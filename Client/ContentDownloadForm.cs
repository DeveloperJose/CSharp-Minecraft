using System;
using System.IO;
using System.Net;
using System.Windows.Forms;
namespace Client
{
    public partial class ContentDownloadForm : Form
    {
        private string[] Filenames = new string[] {
            "calm1.xnb",
            "char.xnb",
            "Crosshair.xnb",
            "MainFont.xnb",
            "terrain.xnb"
        };
        public ContentDownloadForm(bool redownload)
        {
            InitializeComponent();
            for (var i = 0; i < Filenames.Length; i++)
            {
                string currentFile = "Content/" + Filenames[i];
                if (!File.Exists(currentFile) || redownload) // Check to see if we need to download it.
                {
                    using (var client = new WebClient())
                    {
                        client.DownloadFileAsync(new Uri("http://gamemakergm.x10.mx/" + currentFile), // Address
                            currentFile,  // Filename
                            i); // Token

                        client.DownloadProgressChanged += client_DownloadProgressChanged;
                        client.DownloadFileCompleted += client_DownloadFileCompleted;
                    }
                }
            }

        }

        void client_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            int index = (int)e.UserState;
            if ( (index == (Filenames.Length - 1)) && // We reached the end.
                MessageBox.Show("Done! Want to restart the application?", "Content Solver", MessageBoxButtons.YesNo) == DialogResult.Yes)
                Application.Restart();
        }

        void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            int index = (int)e.UserState;
            progressBar.Value = e.ProgressPercentage;
            progressLabel.Text = Filenames[index] + " - Completed: " + e.ProgressPercentage + "%";
        }
    }
}
