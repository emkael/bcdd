using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BCDD
{
    class InvalidLayoutException : Exception
    {
        public InvalidLayoutException() : base() { }
        public InvalidLayoutException(String msg) : base(msg) { }
    }

    class BoardLayout
    {
        public Dictionary<String, Dictionary<String, List<String>>> Hands;

        public BoardLayout() { }
        public BoardLayout(String pbnLayout)
        {
            this.parsePBNLayout(pbnLayout);
        }

        public void parsePBNLayout(string pbnLayout)
        {
            this.Hands = new Dictionary<String, Dictionary<String, List<String>>>();

            // split first player designation from hands
            string[] deal = pbnLayout.Split(':');
            if (deal.Length < 2)
            {
                throw new InvalidLayoutException(String.Format("First hand undefined in PBN layout: {0}", pbnLayout));
            }
            if (deal[0].Length != 1 || !BCalcWrapper.PLAYERS.Contains(deal[0][0]))
            {
                throw new InvalidLayoutException(String.Format("Invalid first hand designation: {0}", deal[0]));
            }

            // determine order of players for hands
            int handLetterIndex = Array.IndexOf(BCalcWrapper.PLAYERS, deal[0][0]);
            char[] handLetters = new char[4];
            Array.Copy(BCalcWrapper.PLAYERS, handLetterIndex, handLetters, 0, 4 - handLetterIndex);
            Array.Copy(BCalcWrapper.PLAYERS, 0, handLetters, 4 - handLetterIndex, handLetterIndex);

            // split cards into hands
            string[] playerHands = deal[1].Split(' ');
            if (playerHands.Length != 4)
            {
                throw new InvalidLayoutException(String.Format("Deal does not contain four hands: {0}", deal[1]));
            }

            // split each hand into suits
            for (int playerIndex = 0; playerIndex < playerHands.Length; playerIndex++)
            { 
                Dictionary<String, List<String>> hand = new Dictionary<string, List<String>>();
                string[] cards = playerHands[playerIndex].Split('.');
                if (cards.Length != 4)
                {
                    throw new InvalidLayoutException(String.Format("Hand does not contain four suits: {0}", playerHands[playerIndex]));
                }
                for (int suitIndex = 0; suitIndex < cards.Length; suitIndex++)
                {
                    List<String> currentHand = new List<String>();
                    foreach (String card in Regex.Split(cards[suitIndex], string.Empty))
                    {
                        if (card.Length > 0)
                        {
                            currentHand.Add(card);
                        }
                    }
                    hand[(BCalcWrapper.DENOMINATIONS[3 - suitIndex]).ToString()] = currentHand;
                }
                this.Hands[(handLetters[playerIndex]).ToString()] = hand;
            }
        }
    }
}
