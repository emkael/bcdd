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

        public ParScore(PBNBoard board)
        {
            this.board = board;
        }

    }
}
