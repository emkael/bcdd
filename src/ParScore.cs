using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BCDD
{
    class ParScoreInvalidException : FieldNotFoundException
    {
        public ParScoreInvalidException() : base() { }
        public ParScoreInvalidException(String msg) : base(msg) { }
    }

    class ParScore
    {
        private PBNBoard board;
        private static Regex pbnContractPattern = new Regex(@"(\d)([CDHSN])(X?)\s+([NESW])");
        private static Regex pbnScorePattern = new Regex(@"(NS|EW)\s+(-?\d})");
        private static Regex jfrContractPattern = new Regex(@"^(\d)([CDHSN])(D?)([NESW])(-?\d+)$");

        public ParScore(PBNBoard board)
        {
            this.board = board;
        }

        public ParContract GetPBNParContract()
        {
            String contractField = this.board.GetOptimumResult();
            if ("Pass".Equals(contractField))
            {
                return new ParContract();
            }
            Match contractMatch = ParScore.pbnContractPattern.Match(contractField);
            if (!contractMatch.Success)
            {
                throw new ParScoreInvalidException("Invalid format for OptimumResult field: " + contractField);
            }
            String scoreField = this.board.GetOptimumScore();
            Match scoreMatch = ParScore.pbnScorePattern.Match(scoreField);
            if (!scoreMatch.Success)
            {
                throw new ParScoreInvalidException("Invalid format for OptimumScore field: " + scoreField);
            }
            int score = Int16.Parse(scoreMatch.Groups[2].Value);
            if ("EW".Equals(scoreMatch.Groups[1].Value))
            {
                score = -score;
            }
            ParContract contract = new ParContract(Int16.Parse(contractMatch.Groups[1].Value),
                contractMatch.Groups[2].Value[0],
                contractMatch.Groups[4].Value[0],
                "X".Equals(contractMatch.Groups[3].Value),
                score);
            return contract.Validate();
        }

        public ParContract GetJFRParContract()
        {
            String parString = this.board.GetMinimax();
            Match parMatch = ParScore.jfrContractPattern.Match(parString);
            if (!parMatch.Success)
            {
                throw new ParScoreInvalidException("Invalid format for Minimax field: " + parString);
            }
            if ("0".Equals(parMatch.Groups[4].Value))
            {
                return new ParContract(); // pass-out
            }
            ParContract contract = new ParContract(Int16.Parse(parMatch.Groups[1].Value),
                parMatch.Groups[2].Value[0],
                parMatch.Groups[4].Value[0],
                "D".Equals(parMatch.Groups[3].Value),
                Int16.Parse(parMatch.Groups[5].Value));
            return contract.Validate();
        }

        private bool determineVulnerability(String vulnerability, char declarer)
        {
            vulnerability = vulnerability.ToUpper();
            return "ALL".Equals(vulnerability) || "BOTH".Equals(vulnerability)
                || (!"LOVE".Equals(vulnerability) && !"NONE".Equals(vulnerability) && vulnerability.Contains(declarer));
        }

        private ParContract getHighestMakeableContract(int[,] ddTable, bool forNS = true, bool forEW = true)
        {
            ParContract contract = new ParContract();
            int tricks = 0;
            for (int i = 3; i >= 0; i--)
            {
                if ((i % 2 == 0 && forNS)
                    || (i % 2 == 1 && forEW))
                {
                    for (int j = 0; j < 5; j++)
                    {
                        int level = ddTable[i, j] - 6;
                        if (level > contract.Level
                            || (level == contract.Level && j > Array.IndexOf(BCalcWrapper.DENOMINATIONS, contract.Denomination)))
                        {
                            contract.Level = level;
                            contract.Denomination = BCalcWrapper.DENOMINATIONS[j];
                            contract.Declarer = BCalcWrapper.PLAYERS[i];
                            tricks = ddTable[i, j];
                        }
                    }
                }
            }
            String vulnerability = this.board.GetVulnerable().ToUpper();
            bool vulnerable = this.determineVulnerability(vulnerability, contract.Declarer);
            contract.Score = contract.CalculateScore(tricks, vulnerable);
            return contract;
        }

        public ParContract GetDDTableParContract(int[,] ddTable)
        {
            String dealer = this.board.GetDealer();
            String vulnerability = this.board.GetVulnerable().ToUpper();
            ParContract nsHighest = this.getHighestMakeableContract(ddTable, true, false);
            ParContract ewHighest = this.getHighestMakeableContract(ddTable, false, true);
            bool nsPlaying = ("N".Equals(dealer) || "S".Equals(dealer));
            if (nsHighest == ewHighest)
            {
                return nsPlaying ? nsHighest.Validate() : ewHighest.Validate();
            }
            ParContract highest = nsHighest.Higher(ewHighest) ? nsHighest : ewHighest;
            nsPlaying = ('N'.Equals(highest.Declarer) || 'S'.Equals(highest.Declarer));
            bool defenseVulnerability = this.determineVulnerability(vulnerability, nsPlaying ? 'E' : 'N');
            ParContract highestDefense = highest.GetDefense(ddTable, defenseVulnerability);
            if (highestDefense != null)
            {
                return highestDefense.Validate();
            }
            int denominationIndex = Array.IndexOf(BCalcWrapper.DENOMINATIONS, highest.Denomination);
            int declarerIndex = Array.IndexOf(BCalcWrapper.PLAYERS, highest.Declarer);
            List<int> playerIndexes = new List<int>();
            playerIndexes.Add(declarerIndex);
            playerIndexes.Add((declarerIndex + 2) & 3);
            bool vulnerable = this.determineVulnerability(vulnerability, highest.Declarer);
            int scoreSquared = highest.Score * highest.Score;
            List<ParContract> possibleOptimums = new List<ParContract>();
            for (int i = 0; i < 5; i++)
            {
                foreach (int player in playerIndexes)
                {
                    int level = highest.Level;
                    if (i > denominationIndex)
                    {
                        level--;
                    }
                    while (level > 0)
                    {
                        ParContract contract = new ParContract(level, BCalcWrapper.DENOMINATIONS[i], BCalcWrapper.PLAYERS[player], false, 0);
                        contract.Score = contract.CalculateScore(ddTable[player, i], vulnerable);
                        if (scoreSquared < contract.Score * highest.Score)
                        {
                            possibleOptimums.Add(contract.GetDefense(ddTable, defenseVulnerability) ?? contract);
                        }
                        else
                        {
                            break;
                        }
                        level--;
                    }
                }
            }
            foreach (ParContract contract in possibleOptimums)
            {
                if (Math.Abs(contract.Score) > Math.Abs(highest.Score) || (contract.Score == highest.Score && contract.Higher(highest)))
                {
                    highest = contract;
                }
            }
            return highest.Validate();
        }

        public ParContract GetParContract(int[,] ddTable)
        {
            try
            {
                return this.GetJFRParContract();
            }
            catch (FieldNotFoundException)
            {
                try
                {
                    return this.GetPBNParContract();
                }
                catch (FieldNotFoundException)
                {
                    return this.GetDDTableParContract(ddTable);
                }
            }
        }

    }
}
