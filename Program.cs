using System;
using System.Collections.Generic;
using System.Text;
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
            List<String> errors = new List<String>();
            if (files.Count > 0)
            {
                foreach (String filename in files)
                {
                    try
                    {
                        Console.WriteLine("Analyzing " + filename);
                        PBNFile file = new PBNFile(filename);
                        foreach (PBNBoard board in file.Boards)
                        {
                            DDTable table = new DDTable(board);
                            String boardNo;
                            try
                            {
                                boardNo = board.GetNumber();
                            }
                            catch (FieldNotFoundException)
                            {
                                boardNo = "?";
                            }
                            try
                            {
                                int[,] ddTable = table.GetDDTable();
                                if (ddTable != null)
                                {
                                    Console.WriteLine("Board " + boardNo);
                                    DDTable.PrintTable(ddTable);
                                    ParScore par = new ParScore(board);
                                    ParContract contract = par.GetParContract(ddTable);
                                    Console.WriteLine(contract);
                                    Console.WriteLine();
                                    board.SaveDDTable(ddTable);
                                    board.SaveParContract(contract);
                                    file.WriteBoard(board);
                                }
                                else
                                {
                                    String error = "ERROR: unable to determine DD table for board " + boardNo;
                                    errors.Add(String.Format("[{0}] {1}", filename, error));
                                    Console.WriteLine(error);
                                }
                            }
                            catch (DllNotFoundException)
                            {
                                throw;
                            }
                            catch (Exception e)
                            {
                                errors.Add(String.Format("[{0}:{1}] {2}", filename, boardNo, e.Message));
                                Console.WriteLine(e.Message);
                            }
                        }
                        file.Save();
                    }
                    catch (DllNotFoundException)
                    {
                        Console.WriteLine("libbcalcdds.dll could not be loaded - make sure it's present in application directory!");
                        break;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("ERROR: " + e.Message);
                    }
                }
                if (errors.Count > 0) {
                    Console.WriteLine("Following ERRORs occured:");
                    foreach (String error in errors) {
                        Console.WriteLine(error);
                    }
                }
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }
    }
}
