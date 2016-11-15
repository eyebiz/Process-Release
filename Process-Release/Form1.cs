using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Process_Release
{
    public partial class Form1 : Form
    {
        string[] extensions = new[] { ".m3u", ".nfo", ".sfv" }, subFolders;
        Thread backgroundThread;

        public Form1()
        {
            InitializeComponent();
        }

        public delegate void UpdateUI();

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbdRelease = new FolderBrowserDialog();
            tsProgress.Value = 0;
            try
            {
                fbdRelease.Description = "Choose Folder";
                fbdRelease.SelectedPath = @"D:\Torrents";

                if (fbdRelease.ShowDialog() == DialogResult.OK)
                {
                    tbFolder.Text = fbdRelease.SelectedPath;
                    subFolders = Directory.GetDirectories(fbdRelease.SelectedPath);
                    lbNbrOfSubfolders.Text = "Number of subfolders: " + subFolders.Length.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            fbdRelease.Dispose();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnProcess_Click(object sender, EventArgs e)
        {
            Invoke(new UpdateUI(() => setStatus("Processing...", true)));
            try
            {
                if (subFolders.Length == 0)
                {
                    DirectoryInfo di = new DirectoryInfo(tbFolder.Text);
                    FileInfo[] files = di.EnumerateFiles().Where(f => extensions.Contains(f.Extension.ToLower())).ToArray();
                        

                    foreach (FileInfo file in files)
                    {
                        Program.ProcessFile(file.FullName, tbFolder.Text);

                        //backgroundThread = new Thread(() => Program.ProcessFile(file.FullName, tbFolder.Text));
                        //backgroundThread.Start();
                    }
                }
                else
                {
                    foreach (string folder in subFolders)
                    {
                        DirectoryInfo di = new DirectoryInfo(folder);
                        FileInfo[] files = di.EnumerateFiles().Where(f => extensions.Contains(f.Extension.ToLower())).ToArray();
                        // MessageBox.Show(files[0].ToString() + files[1].ToString() + files[2].ToString());

                        foreach (FileInfo file in files)
                        {
                            Program.ProcessFile(file.FullName, tbFolder.Text);

                            //backgroundThread = new Thread(() => Program.ProcessFile(file.FullName, tbFolder.Text));
                            //backgroundThread.Start();
                        }
                    }
                }
                Invoke(new UpdateUI(() => setStatus("Idle.", false)));

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }



        private void setStatus(string status, bool disableUI)
        {
            tsStatus.Text = status;
            tsStatus.Invalidate();

            if (disableUI)
            {
                btnProcess.Enabled = false;
                btnExit.Enabled = false;
                btnBrowse.Enabled = false;
                tsProgress.Style = ProgressBarStyle.Marquee;
            }
            else
            {
                btnProcess.Enabled = true;
                btnExit.Enabled = true;
                btnBrowse.Enabled = true;
                tsProgress.Style = ProgressBarStyle.Blocks;
            }
            Application.DoEvents();
        }
    }
}
