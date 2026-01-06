using System;
using System.IO;
using H.CLI.Interfaces;

namespace H.CLI
{
    public static class InfrastructureConstants
    {
        public static string BaseOutputDirectoryPath { get; set; }
        public static string CommandLinePromptPrefix = "--> ";

        public static bool CheckOutputDirectoryPath(string givenPath, IDriveInfoWrapper givenPathDriveInfo, string farmsFoldersPath)
        {
            if (string.IsNullOrEmpty(givenPath))
            {
                BaseOutputDirectoryPath = farmsFoldersPath;
                return false;
            }
            else if (givenPathDriveInfo.DriveType == DriveType.Network)
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(Properties.Resources.CannotWriteToNetworkDrive);
                Console.ForegroundColor = ConsoleColor.White;
                BaseOutputDirectoryPath = farmsFoldersPath;
                return false;
            }
            else
            {
                BaseOutputDirectoryPath = givenPath;
                return true;
            }
        }
    }
}

