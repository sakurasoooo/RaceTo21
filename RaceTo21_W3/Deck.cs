using System;
using System.Collections.Generic;
using System.Linq; // currently only needed if we use alternate shuffle method

namespace RaceTo21
{
    public class Deck
    {
        List<Card> cards = new List<Card>(); // Cache unshuffled cards.

        List<Card> cardsCopy; // cards for the game

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

        /// <summary>
        /// Shuffle the cards
        /// </summary>
        public void Shuffle()
        {
            Console.WriteLine("Shuffling Cards...");

            Random rng = new Random();

            // one-line method that uses Linq:
            cardsCopy = cardsCopy.OrderBy(a => rng.Next()).ToList();

            // multi-line method that uses Array notation on a list!
            // (this should be easier to understand)
            //for (int i = 0; i < cardsCopy.Count; i++)
            //{
            //    Card tmp = cardsCopy[i];
            //    int swapindex = rng.Next(cardsCopy.Count);
            //    cardsCopy[i] = cardsCopy[swapindex];
            //    cardsCopy[swapindex] = tmp;
            //}
        }

        /* Maybe we can make a variation on this that's more useful,
         * but at the moment it's just really to confirm that our 
         * shuffling method(s) worked! And normally we want our card 
         * table to do all of the displaying, don't we?!
         */
        // shoud be commented, useless for the game
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

        /// <summary>
        /// Return The top card of the deck
        /// Pop the last card in card list
        /// </summary>
        /// <returns></returns>
        public Card DealTopCard()
        {
            if (cardsCopy.Count <= 0) {
                // Although unlikely, when the deck is emptied, a new deck of cards is used.
                BuildDeck();
                Shuffle();
            }
            Card card = cardsCopy[cardsCopy.Count - 1];
            cardsCopy.RemoveAt(cardsCopy.Count - 1);
            // Console.WriteLine("I'm giving you " + card);
            return card;
        }

        /// <summary>
        /// Return the copy of cards
        /// </summary>
        /// <returns> the copy of cards </returns>
        public List<Card> GetAllCards()
        {
            return cards.ToList();
        }

        /// <summary>
        /// Create a new unshuffled card deck.
        /// </summary>
        public void BuildDeck()
        {
            cardsCopy = GetAllCards();
        }

        /// <summary>
        /// Return the copy of cardCopys
        /// </summary>
        /// <returns> the copy of cardsCopy </returns>
        public List<Card> Cards()
        {
            return cardsCopy.ToList();
        }
    }
}

