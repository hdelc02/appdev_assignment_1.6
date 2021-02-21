using System;
using System.Collections.Generic;
using System.Text;

namespace Hangman
{
    class Engine
    {
        static void Main(string[] args)
        {
            var game = new Hangman();
            while(!game.GameOver)
            {
                Console.WriteLine(game.GetState());
                try
                {
                    game.Update(Console.ReadLine());
                }
                catch { }
                Console.Clear();
            }
            Console.WriteLine(game.GetState());
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
