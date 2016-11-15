using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Process_Release
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        public static void ProcessFile(string fileName, string folderForLogFile)
        {
            using (StreamWriter w = File.AppendText(folderForLogFile + @"\log.txt"))
            {
                string[] lines = File.ReadAllLines(fileName, Encoding.GetEncoding(28591));
                File.Delete(fileName);
                File.WriteAllLines(fileName, lines, Encoding.GetEncoding(28591));
                Log("- File " + fileName + " corrected.", w);
            }
        }

        public static void Log(string logMessage, TextWriter w)
        {
            w.Write("\rLog Entry: ");
            w.WriteLine("{0} {1} {2}", DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString(), logMessage);
        }
    }
}
