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
        CardTable cardTable; // object in charge of displaying game information
        Deck deck = new Deck(); // deck of cards
        int currentPlayer = 0; // current player on list
        public NextTask nextTask; // keeps track of game state
        private bool cheating = false; // lets you cheat for testing purposes if true
        int targetScore = 50; // the end condition of game
        int rounds = 0; // the number of  rounds of game goes

        public Game(CardTable c)
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            cardTable = c;
            deck.Shuffle();

            deck.ShowAllCards();// Comment out this LINE

            nextTask = NextTask.GetNumberOfPlayers;
            
            //Test
            cardTable.InitializeCardImagePath(deck);// Comment out this LINE
            Console.ResetColor();
        }

        /* Adds a player to the current game
         * Called by DoNextTask() method
         */
        public void AddPlayer(string n)
        {
            players.Add(new Player(n));
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

                    numberOfPlayers = cardTable.GetNumberOfPlayers();
                    nextTask = NextTask.GetNames;
                    break;

                case NextTask.GetNames:

                    for (var count = 1; count <= numberOfPlayers; count++)
                    {
                        var name = cardTable.GetPlayerName(count);
                        AddPlayer(name); // NOTE: player list will start from 0 index even though we use 1 for our count here to make the player numbering more human-friendly
                    }
                    nextTask = NextTask.IntroducePlayers;
                    break;

                case NextTask.IntroducePlayers:

                    cardTable.ShowPlayers(players);
                    nextTask = NextTask.PlayerTurn;
                    break;

                case NextTask.PlayerTurn:

                    cardTable.ShowHands(players);
                    Player player = players[currentPlayer];
                    if (player.status == PlayerStatus.active)
                    {
                        player.RoundStart();// use for final score
                        if (cardTable.OfferACard(player))
                        {
                            Card card = deck.DealTopCard();
                            player.cards.Add(card);
                            int playerScore = ScoreHand(player);
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
                    cardTable.ShowHand(player);
                    nextTask = NextTask.CheckForEnd;
                    break;

                case NextTask.CheckForEnd:
                    Player winner = null;
                    if (!CheckRoundEnd(ref winner))
                    {
                        rounds++; // increase at the end of each ROUND
                        cardTable.ShowHands(players);
                        cardTable.AnnounceRoundWinner(winner);

                        /*
                         * 
                         * Change win condition here
                         * depends on rounds or score
                         * 
                         */
                        DoScoring(winner);
                        cardTable.ShowScoreBoard(players, targetScore);
                        // ask all players if want to continue
                        if (cardTable.AskExitGame())
                        {
                            nextTask = NextTask.GameOver;
                            break;
                        }
                        if (DoFinalScoring())
                        {
                            nextTask = NextTask.GameOver;
                        }else
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

        public int ScoreHand(Player player)
        {
            int score = 0;
            if (cheating == true && player.status == PlayerStatus.active) // cheat
            {
                string response = null;
                while (int.TryParse(response, out score) == false)
                {
                    Console.Write("OK, what should player " + player.name + "'s score be?");
                    response = Console.ReadLine();
                }
                return score;
            }
            else
            {
                score = player.HandScore();
            }
            return score;
        }

        public bool CheckRoundEnd(ref Player winner)
        {
            //Check who has 21
            foreach (var player in players)
            {
                if (player.status == PlayerStatus.win)
                {
                    winner = player;
                    return false; //  the 21 winner
                }

            }

            foreach (var playerA in players)
            {
                if (playerA.status != PlayerStatus.bust)
                {
                    foreach (var playerB in players.Skip(players.LastIndexOf(playerA) + 1)) // skip the front players
                    {
                        if (playerB.status != PlayerStatus.bust && (playerA.status == PlayerStatus.active || playerB.status == PlayerStatus.active))
                            return true; // at least two players is still going!
                    }
                }

                if (playerA.status == PlayerStatus.active)
                {
                    // else all others have busted
                    winner = playerA;
                    return false;
                }

                if (playerA.status == PlayerStatus.stay)
                {
                    if (winner == null)
                    {
                        winner = playerA;
                    }
                    else {
                        winner = CompareScore(winner, playerA);

                    }
                    foreach (var playerB in players)
                    {
                        if (playerB.status != PlayerStatus.bust)
                        {
                            //use turn count for tiebreaker 
                            winner = CompareScore(playerB, winner);
                        }
                    }

                    return false; // all others bust or the highest score
                }


            }

            Player CompareScore(Player p1, Player p2)
            {
                if (p2.HandScore() > p1.HandScore())
                {
                    return p2; // higher
                }
                else if (p1.HandScore() == p2.HandScore())
                {
                    if (p2.activedTurn < p2.activedTurn)
                    {
                        return p2;
                    }

                    if (p2.activedTurn == p1.activedTurn)
                    {
                        if (GetPlayerIndex(p2) < GetPlayerIndex(p1))
                        {
                            return p2;
                        }
                    }
                }

                return p1;
            }

            int GetPlayerIndex(Player player)
            {
                return players.LastIndexOf(player);
            }

            Console.WriteLine("You are cheating!!");
            //all busted
            return false;
        }


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
                        player.AddScore( 21 - player.HandScore());
                    }
                }
            }


        }

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
                    throw new Exception("WInner not found");
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
            deck.BuildDeck();
            deck.Shuffle();
            foreach (Player player in players)
            {
                player.ResetRound();
            }
        }

        // check if a player reach the target score
        private bool DoFinalScoring()
        {
            // ask player if to continue play
            // if there is only one player , let the player win
            // a loop here to ask player if want to continue, if not remove the player from players list
            if (players.Count == 1)
            {
                cardTable.AnnounceFinalWinner(players[0]);
                return true;
            }

            foreach (var player in players)
            {
                if (player.score >= targetScore)
                {
                    cardTable.AnnounceFinalWinner(player);
                    return true;
                }
            }

            return false;
        }


       
    }
}
