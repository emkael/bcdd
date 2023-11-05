using System;
using System.Collections.Generic;
using System.Threading;

namespace BCDD
{
    public class BCDDFile
    {
        public List<String> errors;
        private String filename;
        private PBNFile file;

        public static int filesCounter;
        public static ManualResetEvent filesCountdown = new ManualResetEvent(false);

        private int boardCount;
        private ManualResetEvent boardCountdown;

        public BCDDFile(String filename)
        {
            this.errors = new List<String>();
            this.filename = filename;
            this.file = new PBNFile(filename);
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

        private void info(String boardNo, String ddTable, ParContract contract)
        {
            Console.WriteLine(String.Format("[{0}:{1}] {2} {3}", this.filename, boardNo, ddTable, contract));
        }

        private void processBoard(object state)
        {
            PBNBoard board = state as PBNBoard;
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
                    this.info(boardNo, DDTable.ShortFormat(ddTable), contract);
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
            finally
            {
                if (Interlocked.Decrement(ref this.boardCount) == 0)
                {
                    this.boardCountdown.Set();
                }
            }
        }

        public void analyze(object state)
        {
            try
            {
                this.boardCount = this.file.Boards.Count;
                this.boardCountdown = new ManualResetEvent(false);
                foreach (PBNBoard board in this.file.Boards)
                {
                    ThreadPool.QueueUserWorkItem(new WaitCallback(this.processBoard), board);
                }
                boardCountdown.WaitOne();
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
                foreach (PBNBoard board in this.file.Boards) {
                    this.file.WriteBoard(board);
                }
                Console.WriteLine(String.Format("Saving file {0}", this.filename));
                this.file.Save();
                if (Interlocked.Decrement(ref BCDDFile.filesCounter) == 0)
                {
                    BCDDFile.filesCountdown.Set();
                }
            }
        }
    }
}
