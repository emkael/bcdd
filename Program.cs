using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace BCDD
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.Multiselect = true;
            if (fd.ShowDialog() == DialogResult.OK)
            {
                foreach (String filename in fd.FileNames)
                {
                    Console.WriteLine("Analyzing " + filename);
                    PBNFile file = new PBNFile(filename);
                }
                Console.WriteLine("Press any key to continue...");
                Console.ReadLine();
            }
        }
    }
}
