using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace BCDD
{
    class DDTableInvalidException : FieldNotFoundException
    {
        public DDTableInvalidException() : base() { }
        public DDTableInvalidException(String msg) : base(msg) { }
    }

    class DDTable
    {
        private PBNBoard board;

        private int[,] getEmptyTable()
        {
            int[,] result = new int[4, 5];
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    result[i, j] = -1;
                }
            }
            return result;
        }

        private int[,] validateTable(int[,] table)
        {
            foreach (int t in table)
            {
                if (t > 13 || t < 0)
                {
                    throw new DDTableInvalidException("Invalid number of tricks: " + t.ToString());
                }
            }
            return table;
        }

        public DDTable(PBNBoard board)
        {
            this.board = board;
        }

        private static bool bannerDisplayed = false;
        
        public int[,] GetBCalcTable()
        {
            if (!DDTable.bannerDisplayed)
            {
                Console.WriteLine("Double dummy analysis provided by BCalc.");
                Console.WriteLine("BCalc is awesome, check it out: http://bcalc.w8.pl");
                DDTable.bannerDisplayed = true;
            }
            int[,] result = this.getEmptyTable();
            String deal = this.board.GetLayout();
            IntPtr solver = BCalcWrapper.bcalcDDS_new(Marshal.StringToHGlobalAnsi("PBN"), Marshal.StringToHGlobalAnsi(deal), 0, 0);
            for (int denom = 0; denom < 5; denom++)
            {
                BCalcWrapper.bcalcDDS_setTrumpAndReset(solver, denom);
                for (int player = 0; player < 4; player++)
                {
                    BCalcWrapper.bcalcDDS_setPlayerOnLeadAndReset(solver, BCalcWrapper.bcalc_declarerToLeader(player));
                    result[player, denom] = 13 - BCalcWrapper.bcalcDDS_getTricksToTake(solver);
                    String error = Marshal.PtrToStringAuto(BCalcWrapper.bcalcDDS_getLastError(solver));
                    if (error != null)
                    {
                        throw new DDTableInvalidException("BCalc error: " + error);
                    }
                }
            }
            BCalcWrapper.bcalcDDS_delete(solver);
            return this.validateTable(result);
        }

        public int[,] GetJFRTable()
        {
            int[,] result = this.getEmptyTable();
            String ability = this.board.GetAbility();
            MatchCollection abilities = this.board.ValidateAbility(ability);
            foreach (Match playerAbility in abilities)
            {
                char player = playerAbility.Groups[1].Value[0];
                int playerID = Array.IndexOf(BCalcWrapper.PLAYERS, player);
                int denomID = 4;
                foreach (char tricks in playerAbility.Groups[2].Value.ToCharArray())
                {
                    result[playerID, denomID] = (tricks > '9') ? (tricks - 'A' + 10) : (tricks - '0');
                    denomID--;
                }
            }
            return this.validateTable(result);
        }

        public int[,] GetPBNTable()
        {
            List<String> table = this.board.GetOptimumResultTable();
            List<Match> parsedTable = this.board.ValidateOptimumResultTable(table);
            int[,] result = this.getEmptyTable();
            foreach (Match lineMatch in parsedTable)
            {
                char player = lineMatch.Groups[1].Value[0];
                char denom = lineMatch.Groups[2].Value[0];
                int tricks = Int16.Parse(lineMatch.Groups[3].Value);
                int playerID = Array.IndexOf(BCalcWrapper.PLAYERS, player);
                int denomID = Array.IndexOf(BCalcWrapper.DENOMINATIONS, denom);
                result[playerID, denomID] = tricks;
            }
            return this.validateTable(result);
        }

        public int[,] GetDDTable()
        {
            try
            {
                return this.GetJFRTable();
            }
            catch (FieldNotFoundException)
            {
                try
                {
                    return this.GetPBNTable();
                }
                catch (FieldNotFoundException)
                {
                    return this.GetBCalcTable();
                }
            }
        }

        public static void PrintTable(int[,] ddTable)
        {
            foreach (char header in BCalcWrapper.DENOMINATIONS)
            {
                Console.Write('\t');
                Console.Write(header);
            }
            Console.WriteLine();
            for (int i = 0; i < 4; i++)
            {
                Console.Write(BCalcWrapper.PLAYERS[i]);
                for (int j = 0; j < 5; j++)
                {
                    Console.Write('\t');
                    Console.Write(ddTable[i, j].ToString());
                }
                Console.WriteLine();
            }
        }
    }
}
