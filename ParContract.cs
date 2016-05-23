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

    }
}
