using System;

namespace RaceTo21
{
    class Program
    {
        static void Main(string[] args)
        {
            //CardTable cardTable = new CardTable();
            Game game = new Game();
            while (game.nextTask != NextTask.GameOver)
            {
                game.DoNextTask();
            }
        }
    }
}

