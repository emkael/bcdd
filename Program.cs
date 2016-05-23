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
                                Console.WriteLine("ERROR: unable to determine DD table for board " + boardNo);
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            Console.WriteLine(e.StackTrace);
                        }
                    }
                    file.Save();
                }
                Console.WriteLine("Press any key to continue...");
                Console.ReadLine();
            }
        }
    }
}
