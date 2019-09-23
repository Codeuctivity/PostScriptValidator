using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Threading.Tasks;

namespace PostScriptValidator
{
  public class PostScriptValidator : IDisposable
  {
    private string _pathGhostscriptDirectory;

    public string GhostscriptBinPath { get; private set; }
    public int ExitCode { get; private set; }
    public string ErrorMessage { get; private set; }

    private bool _isInitilized;
    private const string c_maskedQuote = "\"";
    public bool Validate (string pathToPsFile)
    {
      IntiPathToGhostscriptBin ();
      var absolutePathToPsFile = Path.GetFullPath (pathToPsFile);

      if (!File.Exists (absolutePathToPsFile))
      {
        throw new FileNotFoundException (absolutePathToPsFile + " not found");
      }

      using (var process = new Process ())
      {
        process.StartInfo.FileName = GhostscriptBinPath;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;

        var startInfo = process.StartInfo;
        //https://stackoverflow.com/questions/258132/validating-a-postscript-without-trying-to-print-it#2981290
        var arguments = new[] { "-sDEVICE=nullpage -dNOPAUSE -dBATCH ", c_maskedQuote, absolutePathToPsFile, c_maskedQuote };
        startInfo.Arguments = string.Concat (arguments);
        process.Start ();

        var outputResult = GetStreamOutput (process.StandardOutput);
        var errorResult = GetStreamOutput (process.StandardError);

        process.WaitForExit ();

        if (process.ExitCode == 0 && string.IsNullOrEmpty (errorResult))
          return true;

        ExitCode = process.ExitCode;
        ErrorMessage = errorResult;
        return false;
      }
    }

    private void IntiPathToGhostscriptBin ()
    {
      if (_isInitilized)
      {
        return;
      }

      _pathGhostscriptDirectory = Path.Combine (Path.GetTempPath (), "Ghostscript" + Guid.NewGuid ());
      Directory.CreateDirectory (_pathGhostscriptDirectory);

      ExtractBinaryFromManifest ("PostScriptValidator.gs9.27.zip");
      GhostscriptBinPath = Path.Combine (_pathGhostscriptDirectory, @"gs9.27\bin", "gswin32c.exe");

      _isInitilized = true;
    }
    private static string GetStreamOutput (StreamReader stream)
    {
      //Read output i<n separate task to avoid deadlocks
      var outputReadTask = Task.Run (() => stream.ReadToEnd ());

      return outputReadTask.Result;
    }

    private void ExtractBinaryFromManifest (string resourceName)
    {
      var pathZipGhostscript = Path.Combine (_pathGhostscriptDirectory, "gs9.27.zip");
      var assembly = Assembly.GetExecutingAssembly ();

      using (var stream = assembly.GetManifestResourceStream (resourceName))
      using (var fileStream = File.Create (pathZipGhostscript))
      {
        stream.Seek (0, SeekOrigin.Begin);
        stream.CopyTo (fileStream);
      }
      ZipFile.ExtractToDirectory (pathZipGhostscript, _pathGhostscriptDirectory);
    }

    public void Dispose () => Directory.Delete (_pathGhostscriptDirectory, true);
  }
}