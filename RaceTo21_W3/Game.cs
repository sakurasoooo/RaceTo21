using System;
using System.Collections.Generic;

using System.Linq;
using System.Numerics;

namespace RaceTo21
{
    public class Game
    {
        int numberOfPlayers; // number of players in current game
        List<Player> players = new List<Player>(); // list of objects containing player data
        //CardTable CardTable; // object in charge of displaying game information
        Deck deck = new Deck(); // deck of cards
        int currentPlayer = 0; // current player on list
        public NextTask nextTask; // keeps track of game state
        private bool cheating = false; // lets you cheat for testing purposes if true
        int targetScore = 50; // the end condition of game
        int turns = 0; // the number of turns of game goes

        public Game()
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            //CardTable = c;
            deck.Shuffle();

            deck.ShowAllCards();// Comment out this LINE. just for test

            nextTask = NextTask.GetNumberOfPlayers;

            CardTable.InitializeCardImagePath(deck);
            Console.ResetColor();

            if (cheating) CardTable.Cheating();
        }

        /* Adds a player to the current game
         * Called by DoNextTask() method
         */
        private void AddPlayer(string n)
        {
            players.Add(new Player(n));
        }

        /// <summary>
        /// Adds a player to the current game
        /// </summary>
        /// <param name="player"></param>
        private void AddPlayer(Player player)
        {
            players.Add(player);
        }

        /// <summary>
        /// Remove the specified player from the list
        /// </summary>
        /// <param name="player"> The player want to remove </param>
        /// <exception cref="Exception"></exception>
        private void RemovePlayer(Player player)
        {
            if (!players.Remove(player))
            {
                throw new Exception("Player is not found when removing from list ");
            }
        }

        /* Figures out what task to do next in game
         * as represented by field nextTask
         * Calls methods required to complete task
         * then sets nextTask.
         */

        public void DoNextTask()
        {
            Console.WriteLine("================================"); // this line should be elsewhere right?
            /*
         * 
         * Changed IFs to Switch for reading more easily
         * 
         */
            switch (nextTask)
            {
                case NextTask.GetNumberOfPlayers:

                    numberOfPlayers = CardTable.GetNumberOfPlayers();
                    nextTask = NextTask.GetNames;
                    break;

                case NextTask.GetNames:

                    for (var count = 1; count <= numberOfPlayers; count++)
                    {
                        var name = CardTable.GetPlayerName(count);
                        AddPlayer(name); // NOTE: player list will start from 0 index even though we use 1 for our count here to make the player numbering more human-friendly
                    }
                    nextTask = NextTask.IntroducePlayers;
                    break;

                case NextTask.IntroducePlayers:

                    CardTable.ShowPlayers(players);
                    nextTask = NextTask.PlayerTurn;
                    break;

                case NextTask.PlayerTurn:
                    turns++; // increase at the start of each turn

                    CardTable.ShowHands(players);
                    Player player = players[currentPlayer];
                    CardTable.ShowProbability(BustProbability(player,1), BustProbability(player, 2), BustProbability(player, 3));
                    if (player.status == PlayerStatus.active)
                    {
                        player.SetTurn(turns);// use for final score
                        if (CardTable.OfferACard(player, out int num))
                        {
                            for (int i = 0; i < num; i++)
                            {
                                Card card = deck.DealTopCard();
                                player.AddCard(card);
                            }

                            CheatScoreHand(player);// enable cheating

                            int playerScore = player.HandScore();
                            if (playerScore > 21)
                            {
                                player.status = PlayerStatus.bust;
                            }
                            else if (playerScore == 21)
                            {
                                player.status = PlayerStatus.win;
                            }
                        }
                        else
                        {
                            player.status = PlayerStatus.stay;
                        }
                    }
                    CardTable.ShowHand(player);
                    nextTask = NextTask.CheckForEnd;
                    break;

                case NextTask.CheckForEnd:
                    if (CheckRoundEnd(out Player winner))
                    {

                        CardTable.ShowHands(players);
                        CardTable.AnnounceRoundWinner(winner);

                        /*
                         * 
                         * Change win condition here
                         * depends on rounds or score
                         * 
                         */
                        DoScoring(winner);
                        CardTable.ShowScoreBoard(players, targetScore);
                        // ask all players if want to continue

                        if (DoFinalScoring())
                        {
                            nextTask = NextTask.GameOver;
                            break;
                        }
                        // ask player if to continue play
                        // if there is only one player , let the player win
                        // a loop here to ask player if want to continue, if not remove the player from players list
                        List<Player> removeList = new List<Player>();
                        foreach (Player p in players)
                        {
                            if (CardTable.AskExitGame(p))
                            {
                                removeList.Add(p);
                            }

                            if (removeList.Count == (players.Count - 1)) break; // end loop if there in only one player
                        }

                        foreach (Player p in removeList)
                        {
                            players.Remove(p);
                        }
                        removeList.Clear();

                        if (DoFinalScoring())
                        {
                            nextTask = NextTask.GameOver;
                            break;
                        }
                        else
                        {
                            // Start New turn
                            // CardTable print new turn msg
                            // shuffle players etc
                            ResetRound(true, winner);
                            nextTask = NextTask.PlayerTurn;
                        }
                    }
                    else
                    {
                        currentPlayer++;
                        if (currentPlayer > players.Count - 1)
                        {
                            currentPlayer = 0; // back to the first player...
                        }
                        nextTask = NextTask.PlayerTurn;
                    }

                    break;

                default:

                    Console.WriteLine("I'm sorry, I don't know what to do now!");
                    nextTask = NextTask.GameOver;
                    break;
            }
        }
        /// <summary>
        /// A method used for cheating, to refill the player's hand with random cards based on player input.
        /// For testing only!
        /// </summary>
        /// <param name="player"></param>
        public void CheatScoreHand(Player player)
        {
            int score = 0;
            if (cheating == true && player.status == PlayerStatus.active) // cheat
            {
                Random rand = new Random();
                player.ClearHand();
                string response = null;
                while (int.TryParse(response, out score) == false)
                {
                    Console.Write("OK, what should player " + player.name + "'s score be?");
                    response = Console.ReadLine();
                }

                while (score > 0)
                {
                    if (score >= 10)
                    {
                        score -= 10;
                        player.AddCard(new Card((CardSuit)rand.Next(0, 4), (CardName)rand.Next(10, 14)));
                    }
                    else
                    {
                        player.AddCard(new Card((CardSuit)rand.Next(0, 4), (CardName)score));
                        score -= score;
                    }
                }
            }

        }

        /// <summary>
        /// Checks if round should end.
        /// Return true if round is over, and return the winner. Otherwise return false.
        ///
        /// First check to see if any player has a score of 21. Return true if so.
        /// Then check if at least one player is active and one player is not bust, then don't end the game if so.
        /// Then if the player is in the active state, the other players must be in the bust state.
        /// Finally check all players with stay status.
        /// The player should not be all bust, although will still return true
        /// </summary>
        /// <param name="winner"> The variable used to accept the return value. </param>
        /// <returns></returns>
        private bool CheckRoundEnd(out Player winner)
        {
            winner = null;
            //Check who has 21
            foreach (var player in players)
            {
                if (player.status == PlayerStatus.win)
                {
                    winner = player;
                    return true; //  the 21 winner
                }

            }

            // find the first player who is not bust
            foreach (var playerA in players)
            {
                if (playerA.status != PlayerStatus.bust)
                {
                    foreach (var playerB in players.Skip(players.LastIndexOf(playerA) + 1)) // skip the front players
                    {
                        if (playerB.status != PlayerStatus.bust && (playerA.status == PlayerStatus.active || playerB.status == PlayerStatus.active))
                            return false; // at least two players is still going!
                    }


                    if (playerA.status == PlayerStatus.active)
                    {
                        // playerA is active but all others have busted 
                        winner = playerA;
                        return true;
                    }

                    if (playerA.status == PlayerStatus.stay)
                    {
                        if (winner == null)
                        {
                            winner = playerA;
                        }
                        else
                        {
                            winner = CompareScore(winner, playerA);

                        }
                        foreach (var playerB in players.Skip(players.LastIndexOf(playerA) + 1))
                        {
                            if (playerB.status != PlayerStatus.bust) // playerB can be only in stay status
                            {
                                //use turn count for tiebreaker 
                                winner = CompareScore(playerB, winner);
                            }
                        }

                    }
                    // player A is stay but all others have busted
                    // or The winner among the stay status players
                    return true;
                }

            }

            // Compares two players and returns the winner.
            // The one with the highest score is the winner.
            // If the scores are tied, the player with the fewest active state rounds is the winner.
            Player CompareScore(Player p1, Player p2)
            {
                if (p2.HandScore() > p1.HandScore())
                {
                    return p2; // higher
                }
                else if (p1.HandScore() == p2.HandScore())
                {
                    if (p2.GetCards().Count > p1.GetCards().Count)
                    {
                        return p2;
                    }

                    else if (p2.GetCards().Count == p1.GetCards().Count)
                    {
                        if (p2.activedTurn < p1.activedTurn)
                        {
                            return p2;
                        }
                        // same activedTurn is impossible 
                    }
                }

                return p1;
            }



            Console.WriteLine("You are cheating!!");
            // should not go here
            //throw new Exception("CheckRoundEnd error");
            return true; // all ones bust by cheating
        }

        /// <summary>
        /// Update the total score of all players.
        /// Only the winner gets points.
        /// </summary>
        /// <param name="winner"> The winning player </param>
        // Check the if any player reach the target score
        private void DoScoring(Player winner)
        {
            foreach (var player in players)
            {
                if (player == winner)
                {
                    player.AddScore(player.HandScore());
                }
                else
                {
                    if (player.status == PlayerStatus.stay) // someone hit 21
                    {
                        //do nothing
                    }

                    if (player.status == PlayerStatus.bust) // still could win...
                    {
                        player.AddScore(21 - player.HandScore());
                    }
                }
            }


        }
        /// <summary>
        /// Reset game state other than player's score.
        /// Reshuffle the cards and empty the player's hand. Call before round starts;
        /// 
        /// If the first argument is true, the order of the players will be shuffled.
        /// If the second argument is a player, that player will be placed last among all players.
        /// </summary>
        /// <param name="ShufflePlayers"> Whether to disrupt the player </param>
        /// <param name="winner"> The winning player </param>
        /// <exception cref="Exception"> Player not exist in the game </exception>
        private void ResetRound(bool ShufflePlayers = false, Player winner = null)
        {
            /*
             * shuffle the order of players to prevent const tie breaker
             * 
             */

            // draw out the winner player
            if (winner != null)
            {
                if (!players.Remove(winner))
                {
                    throw new Exception("The player does not found");
                }
            }
            // shuffle players
            if (ShufflePlayers)
            {
                Random rng = new Random();

                // one-line method that uses Linq:
                players = players.OrderBy(p => rng.Next()).ToList();
            }

            // add winner to the last
            if (winner != null)
            {
                players.Add(winner);
            }

            currentPlayer = 0;
            turns = 0;
            deck.BuildDeck();
            deck.Shuffle();
            foreach (Player player in players)
            {
                player.ResetRound();
            }
        }

        /// <summary>
        /// Checks if the game is over
        /// Return true if the number of players is less than 2
        /// otherwise Return true if any player's score reaches the target score
        /// otherwise return false
        /// </summary>
        /// <returns> game over state </returns>
        // check if a player reach the target score
        private bool DoFinalScoring()
        {
            if (players.Count <= 0)
            {
                // Should not go here
                throw new Exception("There is no players");
            }

            // if there is only one player , let the player win

            if (players.Count == 1)
            {
                CardTable.AnnounceFinalWinner(players[0]);
                return true;
            }

            foreach (var player in players)
            {
                if (player.score >= targetScore)
                {
                    CardTable.AnnounceFinalWinner(player);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Deprecated.
        /// Get the index of the player in players list.
        /// </summary>
        /// <param name="player"> a player </param>
        /// <returns> the index of the player in players list </returns>
        private int GetPlayerIndex(Player player)
        {
            return players.LastIndexOf(player);
        }

        /// <summary>
        /// Calculate the the % Probability player gets bust after drawing x cards;
        /// </summary>
        /// <param name="player"> The player </param>
        /// <param name="num"> numer of cards want to draw </param>
        /// <returns> the % Probability of bust </returns>
        private float BustProbability(Player player, int num)
        {
            List<List<Card>> combinations = new List<List<Card>>();
            var remainingCards = deck.Cards();
            var n = remainingCards.Count; // the number of cards in deck
            List<int> index = new List<int>();

            // Prevent insufficient number of cards.
            if (num > n) {
                remainingCards.AddRange(deck.GetAllCards());
                n = remainingCards.Count;
            }

            //initialize index list
            for (int i = 0; i < num; i++)
            {
                index.Add(i);
            }

            // find all combinations
            //int numofCombinations = GetNumberofCombinations(n, num);
            for (; ; )
            {
                List<Card> combination = new List<Card>();
                for (int j = 0; j < num; j++)
                {
                    combination.Add(remainingCards[index[j]]);
                }
                combinations.Add(combination);

                if (index[0] == (n - num - 1)) break; // stop when there no more combinations

                index = IncreaseIndex(index,n - 1);
            }


            //  0 0 0 -> (threshold - 2) (threshold - 1) (sthreshold)
            List<int> IncreaseIndex(List<int> indexCopy, int threshold)
            {
                indexCopy[indexCopy.Count - 1]++;
                if (indexCopy[indexCopy.Count - 1] >= threshold)
                {
                    indexCopy = IncreaseIndex(indexCopy.Take(indexCopy.Count - 1).ToList(), threshold);
                    indexCopy.Add(indexCopy[indexCopy.Count - 1] + 1); // add the last element + 1 as last element
                }
                return indexCopy;
            }

            
            //find the number of combinations will not bust
            int safeScore = 21 - player.HandScore(); // must be greater or equal 0
            int safeCombinationNum = 0;
            foreach (var comb in combinations)
            {
                int combScore = 0; // the score of the Combination
                foreach (Card card in comb)
                {
                    combScore += card.CardScore();
                }

                if (combScore <= (safeScore)) safeCombinationNum++;
            }
            
            return (1f - (float)safeCombinationNum / (float)combinations.Count) * 100f;
        }

    }
}
