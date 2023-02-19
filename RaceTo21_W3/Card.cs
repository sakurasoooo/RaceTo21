using System;

namespace RaceTo21
{
    /*
	 * no special use
	 * 
	 * Makes it easier for me to type suit names instead of strings
	 * In order to keep the original code structure, I did not change all string checks to enum
	 * 
	 */
    public enum CardSuit
    {
        Clubs,
        Diamonds,
        Hearts,
        Spades
    }
    /*
	 * no special use
	 * 
	 * Makes it easier for me to type card values instead of strings
	 * In order to keep the original code structure, I did not change all string checks to enum
	 * 
	 */
    public enum CardName
    {
        A = 1,
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8,
        Nine = 9,
        Ten = 10,
        Jack = 11,
        Queen = 12,
        King = 13
    }

    public class Card
    {

        CardSuit cardSuit;
        CardName cardName;
        /// <summary>
        /// The id the card eg. 10H, JS.
        ///
        /// If the card value is between 2-10, concat the initial letter of the card suit and the card value.
        /// Otherwise concat the first letter of the card suit and the first letter of the card value.
        /// </summary>
        public string ShortName
        {
            get
            {
                return ((int)cardName > 1 && (int)cardName < 11 ? ((int)cardName).ToString() : cardName.ToString()[0].ToString()) + cardSuit.ToString()[0].ToString();
            }
            private set
            {
                ShortName = value;
            }
        }

        /// <summary>
        /// The full name of the card eg. 10 of Hearts, Jack of Spades. 
        ///
        /// Concat card value , "of" and card suit
        /// </summary>
        public string LongName
        {
            get
            {
                return cardName + " of " + cardSuit.ToString();

            }
            private set
            {
                LongName = value;
            }
        }


        /// <summary>
        /// Card constructor
        /// </summary>
        /// <param name="cardSuit"> Card suit </param>
        /// <param name="cardName"> Card value </param>
        public Card(CardSuit cardSuit, CardName cardName)
        {
            this.cardSuit = cardSuit;
            this.cardName = cardName;
        }

        /// <summary>
        /// Return the short card value, e.g. A, 2, 10, J, Q, K. 
        /// Used for CardScore()
        /// </summary>
        /// <returns> the short card value </returns>
        private string GetCardShortValue()
        {
            return ShortName.Remove(ShortName.Length - 1);
        }

        /// <summary>
        /// Calculate the score of the card, the score of A is 1,
        /// and the score of 2-10 is equal to the card value.
        /// Jack, Queen and King have a score equal to 10
        /// </summary>
        /// <returns> The score of the card </returns>
        public int CardScore()
        {
            string faceValue = GetCardShortValue();
            switch (faceValue)
            {
                case "K":
                case "Q":
                case "J":
                    return 10;
                case "A":
                    return 1;
                default:
                    return int.Parse(faceValue);
            }
        }

        /// <summary>
        /// Same to LongName
        /// </summary>
        /// <returns> The full name of the card </returns>
        public override string ToString()
        {
            return LongName.ToString();
        }

        /// <summary>
        /// Return the suit of card,
        /// 
        /// cardSuit accessor
        /// </summary>
        /// <returns> The suit name of the card </returns>
        public string GetSuitName()
        {
            return cardSuit.ToString();
        }

        /// <summary>
        /// Used for creating card image path. Return the short value of the card, and format single digits as two digits. e.g. "4" -> "04"
        /// </summary>
        /// <returns> formatted short card value </returns>
        public string GetFormattedValueName()
        {
            switch (cardName)
            {
                case >= CardName.Two and <= CardName.Ten:
                    return ((int)cardName).ToString("00");
                default:
                    return cardName.ToString()[0].ToString();

            }
        }
    }




}

