﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BCDD
{
    class PBNFile
    {
        public List<PBNBoard> Boards;

        private String filename;
        private String tmpFileName;

        StreamWriter outputFile;

        public PBNFile(String filename)
        {
            this.filename = filename;
            this.Boards = new List<PBNBoard>();
            String[] contents = File.ReadAllLines(this.filename).Select(l => l.Trim()).ToArray();
            List<String> lines = new List<String>();
            foreach (String line in contents)
            {
                if (line.Length == 0)
                {
                    if (lines.Count > 0)
                    {
                        this.Boards.Add(new PBNBoard(lines));
                        lines = new List<String>();
                    }
                }
                else
                {
                    lines.Add(line);
                }
            }
            if (lines.Count > 0)
            {
                this.Boards.Add(new PBNBoard(lines));
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
