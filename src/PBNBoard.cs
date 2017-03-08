﻿using System;
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

        private bool? hasOptimumResultTable = null;
        private bool? hasAbility = null;

        private static Regex linePattern = new Regex(@"\[(.*) ""(.*)""\]");
        private static Regex abilityPattern = new Regex(@"\b([NESW]):([0-9A-D]{5})\b");
        private static Regex optimumResultTablePattern = new Regex(@"^([NESW])\s+([CDHSN])T?\s+(\d+)$");

        public PBNBoard(List<string> lines)
        {
            this.Fields = new List<PBNField>();
            foreach (String line in lines)
            {
                PBNField field = new PBNField();
                field.RawField = line;
                Match lineParse = PBNBoard.linePattern.Match(line);
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

        public String GetEvent()
        {
            return this.GetField("Event");
        }

        public void WriteEvent(String name)
        {
            for (int i = 0; i < this.Fields.Count; i++)
            {
                if ("Board".Equals(this.Fields[i].Key))
                {
                    this.Fields.Insert(i, new PBNField("Event", name));
                    break;
                }
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

        public MatchCollection ValidateAbility(String ability)
        {
            MatchCollection matches = PBNBoard.abilityPattern.Matches(ability);
            if (matches.Count != 4)
            {
                this.hasAbility = false;
                throw new DDTableInvalidException("Invalid Ability line: " + ability);
            }
            List<String> players = new List<String>();
            foreach (Match match in matches)
            {
                if (players.Contains(match.Groups[1].Value))
                {
                    this.hasAbility = false;
                    throw new DDTableInvalidException("Duplicate entry in Ability: " + match.Groups[0].Value);
                }
                else
                {
                    players.Add(match.Groups[1].Value);
                }
            }
            this.hasAbility = true;
            return matches;
        }

        public String GetAbility()
        {
            return this.GetField("Ability");
        }

        public void DeleteAbility()
        {
            this.DeleteField("Ability");
        }

        public void WriteAbility(int[,] ddTable)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 4; i++)
            {
                sb.Append(BCalcWrapper.PLAYERS[i]);
                sb.Append(':');
                for (int j = 4; j >= 0; j--)
                {
                    sb.Append((char)(ddTable[i, j] > 9 ? 'A' + ddTable[i, j] - 10 : ddTable[i, j] + '0'));
                }
                if (i < 3)
                {
                    sb.Append(' ');
                }
            }
            String abilityStr = sb.ToString();
            this.Fields.Add(new PBNField("Ability", abilityStr));
        }

        public String GetMinimax()
        {
            return this.GetField("Minimax");
        }

        public void DeleteMinimax()
        {
            this.DeleteField("Minimax");
        }

        public void WriteMinimax(ParContract contract)
        {
            String minimax;
            if (contract.Score == 0)
            {
                minimax = "7NS0";
            }
            else
            {
                minimax = String.Format("{0}{1}{2}{3}{4}", contract.Level, contract.Denomination, contract.Doubled ? "D" : "", contract.Declarer, contract.Score);
            }
            this.Fields.Add(new PBNField("Minimax", minimax));
        }

        public String GetOptimumScore()
        {
            return this.GetField("OptimumScore");
        }

        public void DeleteOptimumScore()
        {
            this.DeleteField("OptimumScore");
        }

        public void WriteOptimumScore(ParContract contract)
        {
            this.Fields.Add(new PBNField("OptimumScore", String.Format("NS {0}", contract.Score)));
        }

        public String GetOptimumResult()
        {
            return this.GetField("OptimumResult");
        }

        public List<Match> ValidateOptimumResultTable(List<String> table)
        {
            List<Match> matches = new List<Match>();
            List<String> duplicates = new List<String>();
            foreach (String line in table)
            {
                Match match = PBNBoard.optimumResultTablePattern.Match(line);
                if (!match.Success)
                {
                    this.hasOptimumResultTable = false;
                    throw new DDTableInvalidException("Invalid OptimumResultTable line: " + line);
                }
                String position = match.Groups[1].Value + " - " + match.Groups[2].Value;
                if (duplicates.Contains(position))
                {
                    this.hasOptimumResultTable = false;
                    throw new DDTableInvalidException("Duplicate OptimumResultTable line: " + line);
                }
                else
                {
                    duplicates.Add(position);
                }
                matches.Add(match);
            }
            this.hasOptimumResultTable = true;
            return matches;
        }

        public List<String> GetOptimumResultTable()
        {
            bool fieldFound = false;
            List<String> result = new List<String>();
            foreach (PBNField field in this.Fields)
            {
                if ("OptimumResultTable".Equals(field.Key))
                {
                    fieldFound = true;
                }
                else
                {
                    if (fieldFound)
                    {
                        if (field.Key == null)
                        {
                            result.Add(field.RawField);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            if (!fieldFound)
            {
                this.hasOptimumResultTable = false;
                throw new FieldNotFoundException("OptimumResultTable field not found");
            }
            return result;
        }

        public void DeleteOptimumResultTable()
        {
            bool fieldFound = false;
            List<PBNField> toRemove = new List<PBNField>();
            foreach (PBNField field in this.Fields)
            {
                if ("OptimumResultTable".Equals(field.Key))
                {
                    fieldFound = true;
                    toRemove.Add(field);
                }
                else
                {
                    if (fieldFound)
                    {
                        if (field.Key == null)
                        {
                            toRemove.Add(field);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            foreach (PBNField remove in toRemove)
            {
                this.Fields.Remove(remove);
            }
        }

        public void WriteOptimumResultTable(int[,] ddTable)
        {
            this.Fields.Add(new PBNField("OptimumResultTable", @"Declarer;Denomination\2R;Result\2R"));
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    this.Fields.Add(new PBNField(String.Format("{0} {1}{2} {3}", BCalcWrapper.PLAYERS[i], BCalcWrapper.DENOMINATIONS[j], (BCalcWrapper.DENOMINATIONS[j] == 'N') ? "T" : "", ddTable[i, j])));
                }
            }
        }

        public void SaveParContract(ParContract contract)
        {
            this.DeleteOptimumScore();
            this.WriteOptimumScore(contract); // we're not writing DDS custom fields, just parse them
            this.DeleteMinimax();
            this.WriteMinimax(contract);
        }

        public void SaveDDTable(int[,] ddTable)
        {
            if (this.hasOptimumResultTable == null)
            {
                try
                {
                    List<Match> optimumResultTable = this.ValidateOptimumResultTable(this.GetOptimumResultTable());
                    this.hasOptimumResultTable = true;
                }
                catch (FieldNotFoundException)
                {
                    this.hasOptimumResultTable = false;
                }
            }
            if (this.hasOptimumResultTable == false)
            {
                this.DeleteOptimumResultTable();
                this.WriteOptimumResultTable(ddTable);
            }
            if (this.hasAbility == null)
            {
                try
                {
                    MatchCollection ability = this.ValidateAbility(this.GetAbility());
                    this.hasAbility = true;
                }
                catch (FieldNotFoundException)
                {
                    this.hasAbility = false;
                }
            }
            if (this.hasAbility == false)
            {
                this.DeleteAbility();
                this.WriteAbility(ddTable);
            }
        }
    }
}
