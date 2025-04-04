using System;
using System.Diagnostics.Eventing.Reader;

namespace CPM  //Computer Picker for Monopoly, this won't work on apple II or IBM.
{
    internal class Program  
    {
        public static bool[] Chosen = new bool[8];  //has a player been chosen yet?
        public static string[] PlayerName = new string[8];  //player names
        public static uint seed = (uint)Math.Pow(System.DateTime.Now.TimeOfDay.TotalMilliseconds, 11.0 / 7.0);  //rng seed based on time of day
        public static Random RNG = new Random((int)seed);  //create random number generator

        static void GameRule()  //things you should do during the game
        {
            string[] Law = new string[16];  //stores lines for printing later
            Law[0] = "No trades until computer offers a trade first.";
            Law[1] = "For your first monopoly, you must build from scratch to hotels in one turn.";
            Law[2] = "Always roll to get out of jail.";
            Law[3] = "Always pay to get out of jail";
            Law[4] = "Cannot buy any railroads.";
            Law[5] = "Cannot buy orange or red properties.";
            Law[6] = "Can only build on 1 monopoly.";
            Law[7] = "After getting a monopoly, you must pass go 5 times before building on it.";
            Law[8] = "Play one of the built-in scenarios from the game editor menu.";
            Law[9] = "All Players start with $500.";
            Law[10] = "All players start with $2500.";
            Law[11] = "Must keep at least $500 available at all times.";
            Law[12] = "You may use the rewind feature once during your game.";
            Law[13] = "Play a short game.";
            Law[14] = "Begin a normal game, and use a 60 minute timer.";
            Law[15] = "Auction off the first Nunowned property you land on.";
            int LawNum = RNG.Next(0, 15);
            Console.WriteLine("\n\nThe rule for today's game is:  ");
            Console.WriteLine(Law[LawNum]);
            
        }

        static void GetPlayer()
        {
                int PlayerNum = RNG.Next(0, 8);  //player number picked
                if (Chosen[PlayerNum] == false)  //if player not chosen yet
                {
                    Chosen[PlayerNum] = true;  //set chosen status to true
                    Console.Write(PlayerName[PlayerNum] + ", ");  //display player name
                }
                else
                {
                    GetPlayer();  //this player chosen already, try again
                }

        }  //pick a computer player name

        static int GetNumber(String Prompt)  //get a number from thee user
        {
            string line; int rtn = 0;  //line read and number returned
            Console.Write(Prompt);  //display prompt message before getting input
            line = Console.ReadLine();  //store input

            rtn = int.Parse(line);

            if (rtn < 1 | rtn > 8)
            {
                Console.WriteLine("Number must be between 1 and 8");
                GetNumber(Prompt);
            }
            return rtn;
        }

        static void Main(string[] args)
        {
            //store player names
            PlayerName[0] = "Arthur";
            PlayerName[1] = "Gertrude";
            PlayerName[2] = "Erwin";
            PlayerName[3] = "Maude";
            PlayerName[4] = "Carmen";
            PlayerName[5] = "Isaac";
            PlayerName[6] = "Penelope";
            PlayerName[7] = "Ollie";

            Console.Title = "Computer Picker for NES Monopoly";  //console title
            Console.ForegroundColor = ConsoleColor.White;  //text color for console

            //explain what this program is for
            Console.WriteLine("This program picks computer opponents for");
            Console.WriteLine("the NES version of Monopoly.  All you have");
            Console.WriteLine("to do is supply how many computer players");
            Console.WriteLine("are needed.  The game will determine the");
            Console.WriteLine("order of play.\n");
            
            int NumCPU = GetNumber("How many Computer opponents?  ");  //number of CPU opponents
            
            //prints player names in getplayer function
            Console.WriteLine("\nThe following players have been chosen for your next game:");
            
            for (int i = 0; i < NumCPU; i++)  //loop once for each player picked
            {
                GetPlayer();  //pick an unchosen opponent
            }
            GameRule();

            Console.WriteLine("\nPress a key to exit...");
            Console.ReadKey();  //makes sure output is seen before exiting
        }
    } //end class
} //end namespace
