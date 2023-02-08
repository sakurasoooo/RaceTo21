using System;
using System.Collections.Generic;

namespace RaceTo21
{
	public class Player
	{
		public string name;
		public List<Card> cards = new List<Card>();
		public PlayerStatus status = PlayerStatus.active;
		public int score { get; private set; }
		public int activedTurn { get; private set; } = 0;

        public Player(string n)
		{
			name = n;
        }

		/* Introduces player by name
		 * Called by CardTable object
		 */
		public void Introduce(int playerNum)
		{
			Console.WriteLine("Hello, my name is " + name + " and I am player #" + playerNum);
		}

		public int HandScore()
		{
			int handScore = 0;
			foreach(Card card in cards)
			{
				handScore += card.CardScore();
			}
			return handScore;
		}

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

		/*
		 * 
		 * score setter
		 * 
		 */
		public void AddScore(int score)
		{
			this.score += score;
		}

		public void RoundStart()
		{
			if (status == PlayerStatus.active) activedTurn++;
		}
	}
}

