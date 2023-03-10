using System;
using System.Collections.Generic;
using System.Linq; // currently only needed if we use alternate shuffle method

namespace RaceTo21
{
    public class Deck
    {
        List<Card> cards = new List<Card>();

        List<Card> cardsCopy;

        public Deck()
        {
            Console.WriteLine("*********** Building deck...");
            CardSuit[] suits = { CardSuit.Clubs, CardSuit.Diamonds, CardSuit.Hearts, CardSuit.Spades };

            for (int cardVal = 1; cardVal <= 13; cardVal++)
            {
                foreach (var cardSuit in suits)
                {
                    cards.Add(new Card(cardSuit, (CardName)cardVal));

                }
            }

            BuildDeck();
        }


        public void Shuffle()
        {
            Console.WriteLine("Shuffling Cards...");

            Random rng = new Random();

            // one-line method that uses Linq:
            // cards = cards.OrderBy(a => rng.Next()).ToList();

            // multi-line method that uses Array notation on a list!
            // (this should be easier to understand)
            for (int i = 0; i < cardsCopy.Count; i++)
            {
                Card tmp = cardsCopy[i];
                int swapindex = rng.Next(cardsCopy.Count);
                cardsCopy[i] = cardsCopy[swapindex];
                cardsCopy[swapindex] = tmp;
            }
        }

        /* Maybe we can make a variation on this that's more useful,
         * but at the moment it's just really to confirm that our 
         * shuffling method(s) worked! And normally we want our card 
         * table to do all of the displaying, don't we?!
         */

        public void ShowAllCards()
        {
            for (int i = 0; i < cardsCopy.Count; i++)
            {
                Console.Write(i + ":" + cardsCopy[i]); // a list property can look like an Array!
                if (i < cardsCopy.Count - 1)
                {
                    Console.Write(" ");
                }
                else
                {
                    Console.WriteLine("");
                }
            }
        }

        public Card DealTopCard()
        {
            Card card = cardsCopy[cardsCopy.Count - 1];
            cardsCopy.RemoveAt(cardsCopy.Count - 1);
            // Console.WriteLine("I'm giving you " + card);
            return card;
        }

        public List<Card> GetCards()
        {
            return cards.ToList();
        }

        public void BuildDeck()
        {
            cardsCopy = GetCards();
        }
    }
}

