using System;
using static System.Formats.Asn1.AsnWriter;

namespace RaceTo21
{
    /*
	 * no special use
	 * 
	 * Avoid cumbersome string input for suit names
	 * 
	 */
   public  enum CardSuit
	{
		Clubs,
		Diamonds,
		Hearts,
		Spades
	}

	public enum CardName
	{
		A=1,
		Two=2,
		Three=3,
		Four=4,
		Five=5,
		Six=6,
		Seven=7,
		Eight=8,
		Nine=9,
		Ten=10,
		Jack=11,
		Queen=12,
		King=13
	}

	public class Card
	{

		CardSuit cardSuit;
		CardName cardName;
		public string short_name { get {
				return ((int)cardName > 1 && (int)cardName < 11 ? ((int)cardName).ToString() : cardName.ToString()[0].ToString()) + cardSuit.ToString()[0].ToString();
			} private set {
				short_name = value;
			} }
		public string long_name
		{
			get
			{
				return cardName + " of " + cardSuit.ToString();

			}
			private set
			{
				long_name = value;
			}
		}

        public Card(string short_name, string long_name)
		{
			this.short_name = short_name;
			this.long_name = long_name;
		}

        public Card(CardSuit cardSuit, CardName cardName)
        {
			this.cardSuit = cardSuit;
			this.cardName = cardName;
        }

        public string CardValue()
		{
			return short_name.Remove(short_name.Length - 1);
		}

		public int CardScore()
		{
			string faceValue = CardValue();
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

        public override string ToString()
        {
			return long_name.ToString();
        }

		public string GetSuitName()
		{
			return cardSuit.ToString();
		}

		public string GetShortValueName()
		{
			switch(cardName)
			{
				case >= CardName.Two and <= CardName.Ten:
					return ((int)cardName).ToString("00");
				default:
					return cardName.ToString()[0].ToString();

			}
		}
    }



	
}

