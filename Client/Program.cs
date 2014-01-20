#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Text;
using System.IO;
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
            finally { }
#if !DEBUG
            catch (Exception e)
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
#endif
        }
    }
#endif
}
