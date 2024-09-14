using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp.Notifications;

namespace A5_Automation_Tool
{
    internal class Program
    {
        #region MainMethod
        static void Main()
        {
            // Path to the text file on the desktop
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string backupTxtPath = Path.Combine(desktopPath, "Backup Generator.txt");

            // Check if the text file exists
            if (!File.Exists(backupTxtPath))
            {
                Console.WriteLine($"The file '{backupTxtPath}' does not exist.");
                return;
            }
            // Read the file/folder path from the text file
            string targetPath = File.ReadAllText(backupTxtPath).Trim();

            // Check if the file or folder exists
            if (!Directory.Exists(targetPath) && !File.Exists(targetPath))
            {
                Console.WriteLine($"The specified path '{targetPath}' does not exist.");
                return;
            }

            // Create a backup
            try
            {
                string backupPath = CreateBackup(targetPath);
                Console.WriteLine($"Backup created successfully at: {backupPath}");
            }
            catch (Exception exe)
            {
                Console.WriteLine($"An error occurred: {exe.Message}");
            }
          
            // set the Windows Notification with an Image
            ToastContentBuilder notif = new ToastContentBuilder();
            notif.AddText("Backup Generator"); //Title
            notif.AddText("Folders have been backed up"); //Notification
            notif.AddInlineImage(new Uri(Path.GetFullPath("KAITECH Logo.png"))); //Image
            notif.Show();
            Console.ReadLine();
        }
        #endregion

        #region CreateBackupMethod
        static string CreateBackup(string path)
        {
            // Create a 'Backups' folder next to the file or folder
            string backupFolder = Path.Combine(Path.GetDirectoryName(path), "Backups");
            Directory.CreateDirectory(backupFolder);

            // Generate the backup file/folder name with the date and time
            string fileName = Path.GetFileName(path);
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss tt");
            string backupFileName = $"{fileName} [{timestamp}]";
            string backupPath;

            if (Directory.Exists(path))
            {
                // It's a folder
                backupPath = Path.Combine(backupFolder, backupFileName);
                CopyDirectory(path, backupPath);
            }
            else
            {
                // It's a file
                string extension = Path.GetExtension(path);
                backupPath = Path.Combine(backupFolder, backupFileName + extension);
                File.Copy(path, backupPath, true); // True to overwrite if already exists
            }
            return backupPath;
        }
        #endregion

        #region CopyDirectoryMethod      
        static void CopyDirectory(string sourceDir, string destDir)
        {
            // Recursively copy directories and their content
            Directory.CreateDirectory(destDir);

            // Copy each file into the new directory
            foreach (string file in Directory.GetFiles(sourceDir))
            {
                string destFile = Path.Combine(destDir, Path.GetFileName(file));
                File.Copy(file, destFile, true);
            }

            // Recursively copy subdirectories
            foreach (string dir in Directory.GetDirectories(sourceDir))
            {
                string destSubDir = Path.Combine(destDir, Path.GetFileName(dir));
                CopyDirectory(dir, destSubDir);
            }
        }
        #endregion
    }
}
