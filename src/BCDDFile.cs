using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace BCDD
{
    public class BCDDFile
    {
        public List<String> errors;
        private bool errorsEncountered;
        private static Mutex _errorsPrintLock = new Mutex();
        private String filename;
        private PBNFile file;
        private String shortname;

        public static int filesCounter;
        public static ManualResetEvent filesCountdown = new ManualResetEvent(false);

        private int boardCount;
        private ManualResetEvent boardCountdown;

        public BCDDFile(String filename)
        {
            this.errors = new List<String>();
            this.errorsEncountered = false;
            this.filename = filename;
            this.file = new PBNFile(filename);
            this.shortname = Path.GetFileName(this.filename);
            if (this.file.ParseErrors.Count > 0)
            {
                this.error("PBN format errors encountered");
                foreach (KeyValuePair<int, String> kv in this.file.ParseErrors)
                {
                    this.error(kv.Value, "", kv.Key);
                }
            }
        }

        private void error(String message, String boardNo = "", int lineNo = -1)
        {
            if (!"".Equals(boardNo))
            {
                message = String.Format("[{0}:B#{1}] {2}", this.shortname, boardNo, message);
            }
            else if (lineNo > -1)
            {
                message = String.Format("[{0}:L#{1}] {2}", this.shortname, lineNo, message);
            }
            else
            {
                message = String.Format("[{0}] {1}", this.shortname, message);
            }
            errors.Add(message);
            Console.WriteLine("ERROR: " + message);
        }

        private void info(String boardNo, String ddTable, ParContract contract)
        {
            Console.WriteLine(String.Format("[{0}:{1}] {2} {3}", this.shortname, boardNo, ddTable, contract));
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
                List<String> validationErrors = board.ValidateLayout();
                if (validationErrors.Count > 0)
                {
                    BCDDFile._errorsPrintLock.WaitOne();
                    if (!this.errorsEncountered)
                    {
                        this.error("Deal layout errors encountered");
                    }
                    foreach (String validationError in validationErrors)
                    {
                        this.error(validationError, boardNo);
                    }
                    this.errorsEncountered = true;
                    BCDDFile._errorsPrintLock.ReleaseMutex();
                }
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
