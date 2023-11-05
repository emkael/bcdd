using System;
using System.Collections.Generic;
using System.Threading;

namespace BCDD
{
    public class BCDDFile
    {
        public List<String> errors;
        private String filename;

        public static int filesCounter;
        public static ManualResetEvent filesCountdown = new ManualResetEvent(false);

        public BCDDFile(String filename)
        {
            this.errors = new List<String>();
            this.filename = filename;
        }

        private void error(String message, String boardNo = "")
        {
            if (!"".Equals(boardNo))
            {
                message = String.Format("[{0}:{1}] {2}", this.filename, boardNo, message);
            }
            else
            {
                message = String.Format("[{0}] {1}", this.filename, message);
            }
            errors.Add(message);
            Console.WriteLine("ERROR: " + message);
        }

        public void analyze(object state)
        {
            try
            {
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
                            ParScore par = new ParScore(board);
                            ParContract contract = par.GetParContract(ddTable);
                            board.SaveDDTable(ddTable);
                            board.SaveParContract(contract);
                            Console.WriteLine("Board " + boardNo);
                            DDTable.PrintTable(ddTable);
                            Console.WriteLine(contract);
                            Console.WriteLine();
                        }
                        else
                        {
                            this.error("unable to determine DD table for board " + boardNo, boardNo);
                        }
                    }
                    catch (DllNotFoundException)
                    {
                        throw;
                    }
                    catch (Exception e)
                    {
                        this.error(e.Message, boardNo);
                    }
                    file.WriteBoard(board);
                }
                file.Save();
            }
            catch (DllNotFoundException)
            {
                this.error("libbcalcdds.dll could not be loaded - make sure it's present in application directory!");
                return;
            }
            catch (Exception e)
            {
                this.error(e.Message);
            }
            finally
            {
                if (Interlocked.Decrement(ref BCDDFile.filesCounter) == 0)
                {
                    BCDDFile.filesCountdown.Set();
                }
            }
        }
    }
}
