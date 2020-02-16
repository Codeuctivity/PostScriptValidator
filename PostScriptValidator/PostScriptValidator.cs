using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace PostScriptValidator
{
    /// <summary>
    /// PostScriptValidator wraps calls to ghostscript that tries to parse a given postscript file.
    /// </summary>
    public class PostScriptValidator : IDisposable
    {
        private string pathGhostscriptDirectory;

        /// <summary>
        /// Used Path to ghostscript bin
        /// </summary>
        /// <value>Path to ghostscript bin</value>
        public string GhostscriptBinPath { get; private set; }

        /// <summary>
        /// Last exitcode from ghostscript
        /// </summary>
        /// <value>Is 0 if ghostscript could parse the validated postscript file</value>
        public int ExitCode { get; private set; }

        /// <summary>
        /// Last stderr output from ghostscript
        /// </summary>
        /// <value>Can give you some hints if ghostscript failes to parse the validated postscript file</value>

        public string ErrorMessage { get; private set; }

        /// <summary>
        /// Last stdout output from ghostscript
        /// </summary>
        /// <value>Contains the stdout of the last validation session of ghostscript</value>
        public string StandardOutput { get; private set; }

        private bool isInitilized;
        private bool customGhostscriptlocation;
        private bool disposed;
        private readonly object lockObject = new object();
        private const string c_maskedQuote = "\"";

        /// <summary>
        /// Use this constructor to use the embedded ghostscript binaries on windows, or guess the location on linux
        /// </summary>
        public PostScriptValidator()
        {
        }

        /// <summary>
        /// Use this constructor to use custom ghostscritp bins, e.g. for ubuntu 18.04 /usr/bin/gs
        /// </summary>
        /// <param name="customPathToGhostscriptBin"></param>
        public PostScriptValidator(string customPathToGhostscriptBin)
        {
            customGhostscriptlocation = false;
            isInitilized = true;
        }

        /// <summary>
        /// Validates a ps by trying to parse it using ghostscript
        /// </summary>
        /// <param name="pathToPsFile"></param>
        /// <returns>True for parseable postscript files</returns>
        public bool Validate(string pathToPsFile)
        {
            var absolutePathToPsFile = getAbsoluteFilePath(pathToPsFile);

            //https://stackoverflow.com/questions/258132/validating-a-postscript-without-trying-to-print-it#2981290
            var ghostScriptArguments = new[] { "-sDEVICE=nullpage -dNOPAUSE -dBATCH ", c_maskedQuote, absolutePathToPsFile, c_maskedQuote };

            var ghostScriptCommandResult = GhostScriptCommand(ghostScriptArguments);

            if (ghostScriptCommandResult.ExitCode == 0 && string.IsNullOrEmpty(ghostScriptCommandResult.ErrorMessage))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Embedds fonts in pdf
        /// </summary>
        /// <param name="pathToPdfFile"></param>
        /// <param name="pathToPdfFileWithEmbeddedFonts"></param>
        public void EmbedFonts(string pathToPdfFile, string pathToPdfFileWithEmbeddedFonts)
        {
            var absolutePathToPdfFile = getAbsoluteFilePath(pathToPdfFile);

            // https://stackoverflow.com/questions/258132/validating-a-postscript-without-trying-to-print-it#2981290

            var ghostScriptArguments =
            new[] { " -dSAFER -dNOPLATFONTS -dNOPAUSE -dPDFA -dBATCH -sDEVICE=pdfwrite -dCompatibilityLevel=1.4 -dMaxSubsetPct=100 -dSubsetFonts=true -dEmbedAllFonts=true -sOutputFile=",
                c_maskedQuote,
                pathToPdfFileWithEmbeddedFonts,
                c_maskedQuote,
                " -f ",
                c_maskedQuote,
                absolutePathToPdfFile,
                c_maskedQuote };

            var ghostScriptCommandResult = GhostScriptCommand(ghostScriptArguments);

            if (ghostScriptCommandResult.ExitCode != 0)
            {
                throw new PostScriptValidatorException(ghostScriptCommandResult.ErrorMessage);
            }
        }

        private static string getAbsoluteFilePath(string pathToPdfFile)
        {
            var absolutePathToPsFile = Path.GetFullPath(pathToPdfFile);

            if (!File.Exists(absolutePathToPsFile))
            {
                throw new FileNotFoundException(absolutePathToPsFile + " not found");
            }

            return absolutePathToPsFile;
        }

        private CommandResult GhostScriptCommand(string[] ghostScriptArguments)
        {
            IntiPathToGhostscriptBin();

            using (var process = new Process())
            {
                process.StartInfo.FileName = GhostscriptBinPath;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;

                var startInfo = process.StartInfo;
                startInfo.Arguments = string.Concat(ghostScriptArguments);
                process.Start();

                StandardOutput = GetStreamOutput(process.StandardOutput);
                ErrorMessage = GetStreamOutput(process.StandardError);

                process.WaitForExit();
                ExitCode = process.ExitCode;

                return new CommandResult(StandardOutput, ErrorMessage, process.ExitCode);
            }
        }

        private void IntiPathToGhostscriptBin()
        {
            if (isInitilized)
            {
                return;
            }
            isInitilized = true;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                lock (lockObject)
                {
                    pathGhostscriptDirectory = Path.Combine(Path.GetTempPath(), "Ghostscript" + Guid.NewGuid());
                    Directory.CreateDirectory(pathGhostscriptDirectory);

                    ExtractBinaryFromManifest("PostScriptValidator.gs9.50.zip");
                    GhostscriptBinPath = Path.Combine(pathGhostscriptDirectory, @"gs9.50\bin", "gswin32c.exe");

                    isInitilized = true;
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                GhostscriptBinPath = "/usr/bin/gs";
                customGhostscriptlocation = true;
            }
            else
            {
                throw new NotImplementedException("Sorry, only supporting linux and windows.");
            }
        }

        private static string GetStreamOutput(StreamReader stream)
        {
            //Read output i<n separate task to avoid deadlocks
            var outputReadTask = Task.Run(() => stream.ReadToEnd());

            return outputReadTask.Result;
        }

        private void ExtractBinaryFromManifest(string resourceName)
        {
            var pathZipGhostscript = Path.Combine(pathGhostscriptDirectory, "gs9.50.zip");
            var assembly = Assembly.GetExecutingAssembly();

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            using (var fileStream = File.Create(pathZipGhostscript))
            {
                stream.Seek(0, SeekOrigin.Begin);
                stream.CopyTo(fileStream);
            }
            ZipFile.ExtractToDirectory(pathZipGhostscript, pathGhostscriptDirectory);
        }

        /// <summary>
        /// Disposing ghostscript bins
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposing ghostscript bins
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                if (!customGhostscriptlocation && Directory.Exists(pathGhostscriptDirectory))
                {
                    Directory.Delete(pathGhostscriptDirectory, true);
                }
            }

            disposed = true;
        }
    }
}