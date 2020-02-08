namespace PostScriptValidator
{
    internal class CommandResult
    {
        public CommandResult(string standardOutput, string errorMessage, int exitCode)
        {
            ErrorMessage = errorMessage;
            StandardOutput = standardOutput;
            ExitCode = exitCode;
        }

        public string StandardOutput { get; set; }
        public string ErrorMessage { get; set; }
        public int ExitCode { get; set; }
    }
}