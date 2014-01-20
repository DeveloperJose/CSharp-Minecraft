#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Text;
using System.IO;
using System.Windows;
using Microsoft.Xna.Framework.Content;
#endregion

namespace Client
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        public const string DateFormat = "yyyy'-'MM'-'dd";
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                using (var game = new Client())
                    game.Run();
            }
#if !DEBUG
            catch (ContentLoadException e)
            {
                if (!Directory.Exists("Content"))
                    Directory.CreateDirectory("Content");
                if (MessageBox.Show("There has been a content error. Do you want to check for missing files?", "Content Error", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    new ContentDownloadForm(false).ShowDialog();
                }
                else if (MessageBox.Show("Do you want to re-download all the files?", "Content Error", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    new ContentDownloadForm(true).ShowDialog();
                }
                else
                {
                    AskToSaveException(e);
                }
            }
            catch (Exception e)
            {
                AskToSaveException(e);
            }
#endif
            finally { }
        }
        /// <summary>
        /// Asks the user if they want to save the log of an exception.
        /// </summary>
        /// <param name="e">Exception to try to save.</param>
        private static void AskToSaveException(Exception e)
        {
            if (MessageBox.Show("An error has occured. Save error log file?", "HexaClassic Client", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                using (var dialog = new SaveFileDialog())
                {
                    dialog.DefaultExt = ".txt";
                    dialog.FileName = "HC-Client_ " + DateTime.UtcNow.ToString(DateFormat) + "_errorLog.txt";
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        StringBuilder logFile = new StringBuilder();
                        logFile.AppendLine("Version: " + Client.Version);
                        logFile.AppendLine("Exception:" + e.ToString());
                        File.WriteAllText(dialog.FileName, logFile.ToString());
                    }
                }
            }
        }

    }
#endif
}
