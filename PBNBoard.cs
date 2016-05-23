using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace BCDD
{
    class PBNField
    {
        public String Key;
        public String Value;
        public String RawField;

        public PBNField() { }
        
        public PBNField(String key, String value)
        {
            this.Key = key;
            this.Value = value;
            this.RawField = String.Format("[{0} \"{1}\"]", this.Key, this.Value);
        }

        public PBNField(String rawData)
        {
            this.RawField = rawData;
        }
    }

    class FieldNotFoundException : Exception
    {
        public FieldNotFoundException() : base() { }
        public FieldNotFoundException(String msg) : base(msg) { }
    }

    class PBNBoard
    {
        public List<PBNField> Fields;

        public PBNBoard(List<string> lines)
        {
            this.Fields = new List<PBNField>();
            Regex linePattern = new Regex(@"\[(.*) ""(.*)""\]");
            foreach (String line in lines)
            {
                PBNField field = new PBNField();
                field.RawField = line;
                Match lineParse = linePattern.Match(line);
                if (lineParse.Success)
                {
                    field.Key = lineParse.Groups[1].Value;
                    field.Value = lineParse.Groups[2].Value;
                }
                this.Fields.Add(field);
            }
        }

        public bool HasField(String key)
        {
            foreach (PBNField field in this.Fields)
            {
                if (key.Equals(field.Key))
                {
                    return true;
                }
            }
            return false;
        }

        public String GetField(String key)
        {
            foreach (PBNField field in this.Fields)
            {
                if (key.Equals(field.Key))
                {
                    return field.Value;
                }
            }
            throw new FieldNotFoundException(key + " field not found");
        }

        public void DeleteField(String key)
        {
            List<PBNField> toRemove = new List<PBNField>();
            foreach (PBNField field in this.Fields)
            {
                if (key.Equals(field.Key))
                {
                    toRemove.Add(field);
                }
            }
            foreach (PBNField remove in toRemove)
            {
                this.Fields.Remove(remove);
            }
        }

        public String GetLayout()
        {
            return this.GetField("Deal");
        }

        public String GetNumber()
        {
            return this.GetField("Board");
        }

        public String GetVulnerable()
        {
            return this.GetField("Vulnerable");
        }

        public String GetDealer()
        {
            return this.GetField("Dealer");
        }

    }
}
