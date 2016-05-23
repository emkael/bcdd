using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BCDD
{
    class ParContract
    {
        public int Level = 0;
        public char Denomination;
        public char Declarer;
        public bool Doubled = false;
        public int Score = 0;

        public ParContract() { }

        public ParContract(int level, char denom, char declarer, bool doubled, int score)
        {
            this.Level = level;
            this.Denomination = denom;
            this.Declarer = declarer;
            this.Doubled = doubled;
            this.Score = score;
        }

        public ParContract Validate()
        {
            if (this.Score == 0)
            {
                return this;
            }
            if (this.Level < 1 || this.Level > 7)
            {
                throw new ParScoreInvalidException("Invalid par contract level: " + this.Level.ToString());
            }
            if (!"CDHSN".Contains(this.Denomination))
            {
                throw new ParScoreInvalidException("Invalid par contract denomination: " + this.Denomination);
            }
            if (!"NESW".Contains(this.Declarer))
            {
                throw new ParScoreInvalidException("Invalid par contract declarer: " + this.Declarer);
            }
            return this;
        }

        override public String ToString()
        {
            if (this.Score == 0)
            {
                return "PASS";
            }
            String contract = this.Level.ToString() + this.Denomination;
            String risk = this.Doubled ? "x" : "";
            String declarer = " " + this.Declarer;
            String result = " " + this.Score.ToString("+#;-#;0");
            return contract + risk + declarer + result;
        }

        public override bool Equals(object other)
        {
            ParContract obj = (ParContract)(other);
            return this.Level == obj.Level && this.Denomination == obj.Denomination && this.Score == obj.Score;
        }

        public override int GetHashCode()
        {
            return this.Score + this.Level + 10000 * this.Denomination;
        }

        public int CalculateScore(int tricks, bool vulnerable = false)
        {
            if (this.Level == 0)
            {
                return 0;
            }
            int score = 0;
            if (this.Level + 6 > tricks)
            {
                int undertricks = this.Level + 6 - tricks;
                if (this.Doubled)
                {
                    do
                    {
                        if (undertricks == 1) // first undertrick: 100 non-vul, 200 vul
                        {
                            score -= vulnerable ? 200 : 100;
                        }
                        else
                        {
                            if (undertricks <= 3 && !vulnerable) // second non-vul undertrick: 200
                            {
                                score -= 200;
                            }
                            else // further undertricks: 300
                            {
                                score -= 300;
                            }
                        }
                        undertricks--;
                    }
                    while (undertricks > 0);
                }
                else
                {
                    score = vulnerable ? -100 : -50;
                    score *= undertricks;
                }
            }
            else
            {
                int parTricks = this.Level;
                do
                {
                    if (this.Denomination == 'N' && parTricks == 1) // first non-trump trick: 40
                    {
                        score += 40;
                    }
                    else // other tricks
                    {
                        switch (this.Denomination)
                        {
                            case 'N':
                            case 'S':
                            case 'H':
                                score += 30;
                                break;
                            case 'D':
                            case 'C':
                                score += 20;
                                break;
                        }
                    }
                    parTricks--;
                }
                while (parTricks > 0);
                if (this.Doubled)
                {
                    score *= 2;
                }
                score += (score >= 100) ? (vulnerable ? 500 : 300) : 50; // game premium
                if (this.Level == 7) // grand slam premium
                {
                    score += vulnerable ? 1500 : 1000;
                }
                else if (this.Level == 6) // small slam premium
                {
                    score += vulnerable ? 750 : 500;
                }
                if (this.Doubled)
                {
                    score += 50;
                }
                int overtricks = tricks - this.Level - 6;
                score += this.Doubled
                    ? (vulnerable ? 200 : 100) * overtricks // (re-)double overtricks: 100/200/200/400
                    : overtricks * ((this.Denomination == 'C' || this.Denomination == 'D') ? 20 : 30); // undoubled overtricks
            }
            if (this.Declarer == 'E' || this.Declarer == 'W')
            {
                score = -score;
            }
            return score;
        }

    }
}
