using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Process_Release
{
    public partial class Form1 : Form
    {
        string[] extensions = new[] { ".m3u", ".nfo", ".sfv" }, lines, subFolders;
        Thread backgroundThread;

        public Form1()
        {
            InitializeComponent();
        }

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
            setStatus("Processing...", true);
            try
            {
                backgroundThread = new Thread(
                new ThreadStart(() =>
                {
                    if (subFolders.Length == 0)
                    {
                        DirectoryInfo di = new DirectoryInfo(tbFolder.Text);
                        FileInfo[] files = di.EnumerateFiles().Where(f => extensions.Contains(f.Extension.ToLower())).ToArray();

                        foreach (FileInfo file in files)
                        {
                            ProcessFile(file.FullName);
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
                                ProcessFile(file.FullName);
                            }
                        }
                    }
                    this.BeginInvoke(new Action(() => { setStatus("Idle.", false); }));
                }
                ));
                    backgroundThread.IsBackground = true;
                    backgroundThread.Name = "Process";
                    backgroundThread.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void ProcessFile(string fileName)
        {
            using (StreamWriter w = File.AppendText(tbFolder.Text + @"\log.txt"))
            {
                lines = File.ReadAllLines(fileName, Encoding.GetEncoding(28591));
                File.Delete(fileName);
                File.WriteAllLines(fileName, lines, Encoding.GetEncoding(28591));
                Log("- File " + fileName + " corrected.", w);
            }
        }

        private static void Log(string logMessage, TextWriter w)
        {
            w.Write("\rLog Entry: ");
            w.WriteLine("{0} {1} {2}", DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString(), logMessage);
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
