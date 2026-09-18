using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BCDD
{
    class PBNFile
    {
        public List<PBNBoard> Boards;
        public Dictionary<int, String> ParseErrors;

        private String filename;
        private String tmpFileName;

        StreamWriter outputFile;

        public PBNFile(String filename)
        {
            this.filename = filename;
            this.ParseErrors = new Dictionary<int, string>();
            this.Boards = new List<PBNBoard>();
            List<String> contents = File.ReadAllLines(this.filename).Select(l => l.Trim()).ToList();
            if (!contents.Last().Equals(""))
            {
                contents.Add("");
            }
            List<String> lines = new List<String>();
            int lineNo = 0;
            for (int l = 0; l < contents.Count; l++)
            {
                String line = contents[l];
                if (line.Length != 0)
                {
                    lines.Add(line);
                    continue;
                }
                else
                {
                    if (lines.Count > 0) // ignore leading or multiple empty lines
                    {
                        try
                        {
                            this.Boards.Add(new PBNBoard(lines));
                        }
                        catch (Exception ex)
                        {
                            this.ParseErrors[lineNo+1] = ex.Message;
                        }
                        lines = new List<String>();
                        lineNo = l;
                    }
                }
            }
            if (!this.Boards[0].HasField("Event"))
            {
                this.Boards[0].WriteEvent("");
            }
        }

        public void WriteBoard(PBNBoard board)
        {
            if (this.outputFile == null)
            {
                this.tmpFileName = Path.GetTempFileName();
                this.outputFile = new StreamWriter(new FileStream(this.tmpFileName, FileMode.Create), Encoding.UTF8);
            }
            foreach (PBNField field in board.Fields)
            {
                this.outputFile.WriteLine(field.RawField);
            }
            this.outputFile.WriteLine();
        }

        public void Save()
        {
            if (this.outputFile == null)
            {
                throw new IOException("No boards written to PBN file, unable to save it.");
            }
            this.outputFile.Flush();
            this.outputFile.Close();
            File.Delete(this.filename);
            File.Move(this.tmpFileName, this.filename);
        }
    }
}
