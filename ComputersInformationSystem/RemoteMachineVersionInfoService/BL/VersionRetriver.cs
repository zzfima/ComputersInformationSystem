namespace GetFileVersion.BL
{
    public class VersionRetriver : IVersionRetriver
    {
        public string FileName { get; }
        public VersionRetriver(string fileName)
        {
            FileName = fileName;
        }

        public async Task<string> GetVersionByIp(IPAddress iPAddress, string userName, string password)
        {
            var fileName = @$"\\{iPAddress.ToString()}{FileName}";
            string version = string.Empty;

            try
            {
                version = await Task.Run(() => FileVersionInfo.GetVersionInfo(fileName)?.FileVersion ?? "no version");
            }
            catch (Exception)
            {
                var command = $"cmdkey /add:{iPAddress.ToString()} /user:{userName} /pass:{password}";
                ExecuteCommand(command, 5000);
                version = await Task.Run(() => FileVersionInfo.GetVersionInfo(fileName)?.FileVersion ?? "no version");
            }

            return version;
        }

        public static int? ExecuteCommand(string command, int timeout)
        {
            var processInfo = new ProcessStartInfo("cmd.exe", "/C " + command)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                WorkingDirectory = "C:\\",
            };

            var process = Process.Start(processInfo);
            process?.WaitForExit(timeout);
            var exitCode = process?.ExitCode;
            process?.Close();
            return exitCode;
        }
    }
}