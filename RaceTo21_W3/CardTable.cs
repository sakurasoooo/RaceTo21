using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml.Linq;

namespace RaceTo21
{
    public class CardTable
    {
        static Dictionary<string, string> cardImage = null; // Used to store the card image path.

        public CardTable()
        {

            Console.WriteLine("Setting Up Table...");
        }

        /* Shows the name of each player and introduces them by table position.
         * Is called by Game object.
         * Game object provides list of players.
         * Calls Introduce method on each player object.
         */
        public void ShowPlayers(List<Player> players)
        {
            for (int i = 0; i < players.Count; i++)
            {
                Console.WriteLine("Hello, my name is " + players[i].name + " and I am player #" + (i + 1)); // List is 0-indexed but user-friendly player positions would start with 1...
            }
        }

        /* Gets the user input for number of players.
         * Is called by Game object.
         * Returns number of players to Game object.
         */
        public int GetNumberOfPlayers()
        {
            Console.Write("How many players?(2-8) ");
            string response = Console.ReadLine();
            int numberOfPlayers;
            while (int.TryParse(response, out numberOfPlayers) == false
                || numberOfPlayers < 2 || numberOfPlayers > 8)
            {
                Console.WriteLine("Invalid number of players.");
                Console.Write("How many players?");
                response = Console.ReadLine();
            }
            return numberOfPlayers;
        }
       
        /* Gets the name of a player
         * Is called by Game object
         * Game object provides player number
         * Returns name of a player to Game object
         */
        public string GetPlayerName(int playerNum)
        {
            Console.Write("What is the name of player# " + playerNum + "? ");
            string response = Console.ReadLine();
            while (response.Length < 1)
            {
                Console.WriteLine("Invalid name.");
                Console.Write("What is the name of player# " + playerNum + "? ");
                response = Console.ReadLine();
            }
            return response;
        }
        /// <summary>
        /// Output to console.
        /// Prompts whether the player wants to draw cards.
        /// Returns true if the player wants to draw a card
        /// Returns false if the player does not want to draw cards
        /// Otherwise wait for correct input
        /// </summary>
        /// <param name="player"> A player in the game </param>
        /// <returns></returns>
        public bool OfferACard(Player player, out int num)
        {
            num = 0;
            while (true)
            {
                Console.Write(player.name + ", how many (1-3) cards do you want? (N)O to stay: ");
                string response = Console.ReadLine();
                if (int.TryParse(response, out num))
                {
                    if (num >= 1 && num <= 3) return true;
                }
                else if (response.ToUpper().StartsWith("N"))
                {
                    return false;
                }
                else
                {
                    Console.WriteLine("Please answer (1-3) or (N)O");
                }
            }
        }

        /// <summary>
        /// Output to console.
        /// Display the specified player's hand and score.
        /// e.g. 
        /// p1 has NO cards = 0/21 (STAY)
        /// p2 has: Two of Diamonds, Three of Hearts = 5/21
        /// p3 has: A of Spades, Nine of Diamonds = 10/21
        /// </summary>
        /// <param name="player"> A player in the game </param>
        public void ShowHand(Player player)
        {
            if (player.GetCards().Count > 0)
            {
                /*
                 * 
                 * format card printing with commas
                 * 
                 */
                Console.Write(player.name + " has: ");
                foreach (Card card in player.GetCards())
                {
                    Console.Write(card);
                    //print comma between two cards
                    if (player.GetCards().LastIndexOf(card) < player.GetCards().Count - 1) Console.Write(",");

                    Console.Write(" ");
                }
                Console.Write("= " + player.HandScore() + "/21 ");
                if (player.status != PlayerStatus.active)// refactor here
                {
                    Console.Write("(" + player.status.ToString().ToUpper() + ")");
                }
                Console.WriteLine();
            }
            else
            {
                Console.Write(player.name + " has NO cards ");
                Console.Write("= " + player.HandScore() + "/21 ");
                if (player.status != PlayerStatus.active)// refactor here
                {
                    Console.Write("(" + player.status.ToString().ToUpper() + ")");
                }
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Output to console.
        /// Displays all player hands and scores.
        /// The first argument requires all players.
        /// </summary>
        /// <param name="players"> All players in the game </param>
        public void ShowHands(List<Player> players)
        {
            foreach (Player player in players)
            {
                ShowHand(player);
            }
        }

        /// <summary>
        /// Output to console.
        /// Displays the score of the player.
        /// The first argument requires a player, and the second argument requires the goal score of the game.
        /// </summary>
        /// <param name="player"> The player </param>
        /// <param name="targetScore"> The goal score of the game </param>
        public void ShowScore(Player player, int targetScore)
        {
            Console.Write(player.name + " score: ");
            Console.WriteLine($"{player.score}/{targetScore}");
        }


        /// <summary>
        /// Output to console.
        /// Displays the total score of all players.
        /// Playerscores are sorted from highest to lowest.
        /// The first argument requires all players, and the second argument requires the goal score of the game.
        /// </summary>
        /// <param name="players"> All players in the game </param>
        /// <param name="targetScore"> The goal score of the game </param>
        public void ShowScoreBoard(List<Player> players, int targetScore)
        {
            List<Player> orederPlayers = players.OrderBy(player => player.score).Reverse().ToList();
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("=====Score Board====");
            foreach (Player player in orederPlayers)
            {
                ShowScore(player, targetScore);
            }
            Console.WriteLine("====================");
            Console.ResetColor();
        }


        /// <summary>
        /// Output to console.
        /// Prompts the player if want to interrupt the game.
        /// Returns true if the player typed 'q'
        /// Returns false, if the player typed the enter key
        /// Otherwise wait for correct input
        /// </summary>
        /// <returns> The result of player‘s choice </returns>
        public bool AskExitGame(Player player)
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            bool ans;
            while (true)
            {
                Console.WriteLine("Do player " + player.name + " want to continue?");
                Console.WriteLine("Press <Enter> to continue...Press \"q\" to Exit...");
                var key = Console.ReadKey().Key;
                Console.WriteLine("");
                if (key == ConsoleKey.Enter)
                {
                    ans = false;
                    break;
                }
                if (key == ConsoleKey.Q)
                {
                    ans = true;
                    break;
                }
            }

            Console.ResetColor();
            return ans;
        }

        /// <summary>
        /// Output to console.
        /// Prompts The player with the highest score in the end. If argument is null, it will output "Everyone busted!"
        /// </summary>
        /// <param name="player"> The winning player </param>
        public void AnnounceRoundWinner(Player player)
        {
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            if (player != null)
            {
                Console.WriteLine("✿✿✿✿✿✿✿✿✿✿✿✿✿✿✿✿✿✿✿");
                Console.WriteLine(player.name + " wins!");
                Console.WriteLine("✿✿✿✿✿✿✿✿✿✿✿✿✿✿✿✿✿✿✿");
            }
            else
            {
                // should not go there
                Console.WriteLine("Everyone busted!");
            }
            Console.WriteLine("The round ends... ");

            Console.ResetColor();
        }
        /// <summary>
        /// /// Output to console. 
        /// Prompts The player with the highest accumulated score in the end. If argument is null, it will output "everyone lose"
        /// </summary>
        /// <param name="player"> The winning player </param>
        public void AnnounceFinalWinner(Player player)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            if (player != null)
            {
                Console.WriteLine("***************************************");
                Console.WriteLine("⚘⚘⚘⚘⚘⚘⚘⚘⚘⚘⚘⚘⚘⚘⚘⚘⚘⚘⚘⚘⚘⚘⚘⚘⚘⚘⚘⚘⚘⚘⚘⚘⚘⚘⚘⚘⚘⚘");
                Console.WriteLine(player.name + " is the Final winner!!!");
                Console.WriteLine("⚘⚘⚘⚘⚘⚘⚘⚘⚘⚘⚘⚘⚘⚘⚘⚘⚘⚘⚘⚘⚘⚘⚘⚘⚘⚘⚘⚘⚘⚘⚘⚘⚘⚘⚘⚘⚘⚘");
                Console.WriteLine("***************************************");
            }
            else
            {
                // should NOT go there
                Console.WriteLine("Everyone Lose");
            }
            Console.ResetColor();
            Console.Write("Press <Enter> to exit... ");
            while (Console.ReadKey().Key != ConsoleKey.Enter) { }
        }

        /// <summary>
        /// Use the card in the Deck to create the path of the card image and store it as a dictionary.
        /// The path format is “card_cardsuit_cardformattedvalue.png”.
        /// Used for kenny asset.
        /// </summary>
        /// <param name="deck"> The deck used for game </param>
        public void InitializeCardImagePath(Deck deck)
        {
            //Console.WriteLine("************ Initializing Card image paths....");

            cardImage = new Dictionary<string, string>();

            List<Card> cards = deck.GetCards();
            foreach (Card card in cards)
            {
                cardImage.Add(card.ShortName, $"card_{card.GetSuitName().ToLower()}_{card.GetFormattedValueName().ToUpper()}.png");
                //Console.WriteLine(card.ShortName + " " + cardImage[card.ShortName]);
            }

        }

        /// <summary>
        /// Prompts cheating mode is enabled
        /// </summary>
        public void Cheating()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("***************************************");
            Console.WriteLine("************* Cheating MODE ***********");
            Console.WriteLine("***************************************");
            Console.ResetColor();
        }
    }
}