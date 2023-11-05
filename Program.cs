using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using System.IO;

namespace BCDD
{
    class Program
    {
        static List<String> getFiles(string[] args)
        {
            List<String> filenames = new List<String>();
            foreach (String arg in args)
            {
                if (File.Exists(arg))
                {
                    filenames.Add(arg);
                }
            }
            if (filenames.Count == 0)
            {
                OpenFileDialog fd = new OpenFileDialog();
                fd.Multiselect = true;
                fd.Filter = "PBN files (*.pbn)|*.pbn|All files (*.*)|*.*";
                if (fd.ShowDialog() == DialogResult.OK)
                {
                    filenames = new List<String>(fd.FileNames);
                }
            }
            return filenames;
        }

        [STAThread]
        static void Main(string[] args)
        {
            List<String> files = Program.getFiles(args);
            List<BCDDFile> workers = new List<BCDDFile>();
            List<String> errors = new List<String>();
            BCDDFile.filesCounter = files.Count;
            if (files.Count > 0)
            {
                foreach (String filename in files)
                {
                    BCDDFile worker = new BCDDFile(filename);
                    workers.Add(worker);
                    Console.WriteLine("Analyzing " + filename);
                    ThreadPool.QueueUserWorkItem(new WaitCallback(worker.analyze));
                }
                BCDDFile.filesCountdown.WaitOne();
                foreach (BCDDFile w in workers)
                {
                    errors.AddRange(w.errors);
                }
                if (errors.Count > 0) {
                    Console.WriteLine("Following ERRORs occured:");
                    foreach (String error in errors) {
                        Console.WriteLine(error);
                    }
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                }
            }
        }
    }
}
