using System;
using System.Collections.Generic;
using System.Linq;
namespace RaceTo21
{
    public class CardTable
    {
        static Dictionary<string, string> cardImage = null;
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
                players[i].Introduce(i+1); // List is 0-indexed but user-friendly player positions would start with 1...
            }
        }

        /* Gets the user input for number of players.
         * Is called by Game object.
         * Returns number of players to Game object.
         */
        public int GetNumberOfPlayers()
        {
            Console.Write("How many players? ");
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

        public bool OfferACard(Player player)
        {
            while (true)
            {
                Console.Write(player.name + ", do you want a card? (Y/N)");
                string response = Console.ReadLine();
                if (response.ToUpper().StartsWith("Y"))
                {
                    return true;
                }
                else if (response.ToUpper().StartsWith("N"))
                {
                    return false;
                }
                else
                {
                    Console.WriteLine("Please answer Y(es) or N(o)!");
                }
            }
        }

        public void ShowHand(Player player)
        {
            if (player.cards.Count > 0)
            {
                /*
                 * 
                 * format card printing with commas
                 * 
                 */
                Console.Write(player.name + " has: ");
                foreach (Card card in player.cards)
                {
                    Console.Write(card);
                    //print comma between two cards
                    if (player.cards.LastIndexOf(card) < player.cards.Count - 1) Console.Write(",");

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

        public void ShowHands(List<Player> players)
        {
            foreach (Player player in players)
            {
                ShowHand(player);
            }
        }

        public void ShowScore(Player player, int targetScore)
        {
            Console.Write(player.name + " score: ");
            Console.WriteLine($"{player.score}/{targetScore}");
        }

        public void ShowScoreBoard(List<Player> players, int targetScore)
        {
            List<Player> orederPlayers = players.OrderBy(player => player.score).ToList();
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("=====Score Board====");
            foreach (Player player in orederPlayers)
            {
                ShowScore(player, targetScore);
            }
            Console.WriteLine("====================");
            Console.ResetColor();
        }

        public bool AskExitGame()
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            bool ans;
            while (true)
            {
                Console.WriteLine("Do ALL players want to continue?");
                Console.WriteLine("Press <Enter> to continue...Press \"q\" to Exit...");
                var key = Console.ReadKey().Key;
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
                // should not go there
                Console.WriteLine("Everyone Lose");
            }
            Console.ResetColor();
            Console.Write("Press <Enter> to exit... ");
            while (Console.ReadKey().Key != ConsoleKey.Enter) { }
        }

        public void InitializeCardImagePath(Deck deck)
        {
            Console.WriteLine("************ Initializing Card image paths....");
           
            cardImage = new Dictionary<string, string>();

            List<Card> cards = deck.GetCards();
            foreach (Card card in cards)
            {
                cardImage.Add(card.short_name, $"card_{card.GetSuitName().ToLower()}_{card.GetShortValueName().ToUpper()}.png");
                Console.WriteLine(card.short_name + " " + cardImage[card.short_name]);
            }
           
        }
    }
}