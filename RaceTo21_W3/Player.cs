using System;
using System.Collections.Generic;
using System.Linq;

namespace RaceTo21
{
	public class Player
	{
		/// <summary>
		/// The name of the player
		/// </summary>
		public string name { get; private set; }
        /// <summary>
        /// The cards in player's hand, readonly
        /// </summary>
        readonly private List<Card> cards = new List<Card>();
		/// <summary>
		/// The status of the player
		/// </summary>
		public PlayerStatus status = PlayerStatus.active;
		/// <summary>
		/// The score of the player, readonly
		/// </summary>
		public int score { get; private set; } = 0;
        /// <summary>
        /// The number of turns the player remains active. Often used to compare tiebreakers, readonly
        /// </summary>
        public int activedTurn { get; private set; } = 0;

        public Player(string n)
		{
			name = n;
        }

		/// <summary>
		/// return the hand score of the player, according the cards in hand
		/// </summary>
		/// <returns></returns>
		public int HandScore()
		{
			int handScore = 0;
			foreach(Card card in cards)
			{
				handScore += card.CardScore();
			}
			return handScore;
		}

		/// <summary>
		/// remove all the cards in players hand 
		/// </summary>
		public void ClearHand()
		{
			cards.Clear();
		}

		/*
		 * 
		 * Call on the end of each round
		 *	reset the player's cards and turn counter
		 * 
		 */
		public void ResetRound()
		{
			status = PlayerStatus.active;
			ClearHand();
			activedTurn = 0;
		}

		/// <summary>
		/// Add score to total score of the player, (not hand score)
		/// </summary>
		/// <param name="score"> the score player earned </param>
		public void AddScore(int score)
		{
			this.score += score;
		}

        /// <summary>
        /// Call on the start of the each turn of the player,
        /// Increase the activedTurn counter by 1
        /// </summary>
        public void SetTurn( int turn)
		{
			if (status == PlayerStatus.active) activedTurn = turn;
		}

        /// <summary>
        /// Add a card to player's hand,
        /// call when player draw a card
        /// </summary>
        /// <param name="card"> the card player drawn </param>
        public void AddCard(Card card)
		{
			cards.Add(card);
		}
		/// <summary>
		/// Return the copy of cards in player's hand
		/// </summary>
		/// <returns></returns>
		public List<Card> GetCards()
		{
			return cards.ToList();
        }
	}
}

