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

        public ParContract GetParContract(int[,] ddTable)
        {
            try
            {
                return this.GetJFRParContract();
            }
            catch (FieldNotFoundException)
            {
                return this.GetPBNParContract();
            }
        }

    }
}
