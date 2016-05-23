﻿using System;
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
            foreach (String filename in Program.getFiles(args))
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
