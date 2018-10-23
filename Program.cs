using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace GitBranchCleanup
{
    class Program
    {
        static void Main(string[] args)
        {
            var output = ExecuteCommandWithOutput("cmd.exe", "/c git branch -v", @"C:\source\mlswautomation\");

            foreach (var branchEntry in output.Split('\n'))
            {
                if (branchEntry.Length > 0)
                {
                    var branchName = branchEntry.Trim().Substring(0,
                        branchEntry.Trim().IndexOf(" ", StringComparison.InvariantCultureIgnoreCase)).Trim();

                    var match = Regex.Match(branchEntry, @"behind \d+\]");

                    if (match.Success)
                    {
                        var behindCount = branchEntry.Substring(match.Index + 7, branchEntry.IndexOf("]") - match.Index - 7);

                        if (int.Parse(behindCount) > 500)
                        {
                            // Console.WriteLine($"Branch name: {branchName}, Behind count: {behindCount}");
                            var command = "git branch -D " + branchName;
                            Console.WriteLine(command);
                        }
                    }
                }
            }

            Console.ReadLine();
        }

        private static string ExecuteCommandWithOutput(string executable, string parameters, string workingDir)
        {
            Process cmd = new Process();
            cmd.StartInfo.FileName = executable;
            cmd.StartInfo.Arguments = parameters;
            cmd.StartInfo.WorkingDirectory = workingDir;
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();

            var output = cmd.StandardOutput.ReadToEnd();

            cmd.StandardInput.Close();
            cmd.WaitForExit();

            return output;
        }
    }
}
