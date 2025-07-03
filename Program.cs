using System;
using System.Diagnostics;
using System.IO;
using System.Speech.Synthesis;

namespace CPM  //Computer Picker for Monopoly, this won't work on apple II or IBM.
{
    internal class Program  
    {
        #region VARIABLES
        public static bool[] Chosen = new bool[8];  //has a player been chosen yet?
        public static string[] PlayerName = { "Arthur", "Gertrude", "Erwin", "Maude", "Carmen", "Isaac", "Penelope", "Ollie" };  //player names
        public static uint seed = (uint)Math.Pow(System.DateTime.Now.TimeOfDay.TotalMilliseconds, 11.0 / 7.0);  //rng seed based on time of day
        public static Random RNG = new Random((int)seed);  //create random number generator
        public static string LogPath = Environment.GetEnvironmentVariable("onedriveconsumer") + "\\documents\\CPM\\";  //path to save log file
        public static string TodayLogPath = LogPath + "Last Run.csv";  //file name to write log
        public static string FullLogPath = LogPath + "All Time.csv";  //file name to write log
        public static String[] ItemsToWrite = new String[7];
        public static String[] HeadersToWrite = { "         DATE:", "         TIME:", "# CPU PLAYERS:", " PLAYER NAMES:", "    GAME RULE:", " ELAPSED TIME:", " TOTAL ASSETS:" };
        #endregion

        static string GameRule()  //things you should do during the game
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
            Law[8] = "Play one of the built-in scenarios from the game editor menu.  (1 to 4 players)";
            Law[9] = "All Players start with $500.";
            Law[10] = "All players start with $2500.";
            Law[11] = "Must keep at least $500 available at all times.";
            Law[12] = "You may use the rewind feature once during your game.";
            Law[13] = "Play a short game.";
            Law[14] = "Begin a normal game, and use a 60 minute timer.";
            Law[15] = "Auction off the first unowned property you land on.";
            int LawNum = RNG.Next(0, 15);  //pick a number
            return Law[LawNum];  //return chosen name
        }

        static String GetPlayer()  //pick players for the game
        {
            int PlayerNum;  //random player number gets chosen.  

            do  //do until valid input
            {
                PlayerNum = RNG.Next(0, 8);  //player number picked
                if (Chosen[PlayerNum] == false)  //if player not chosen yet
                {
                    Chosen[PlayerNum] = true;  //set chosen status to true
                    return PlayerName[PlayerNum];   //return player name
                }
            } while (true);
        }  //pick a computer player name

        static double GetNumber(String Prompt, int Low, int High)  //get a number from thee user
        {
            string line; double rtn;  //line read and number returned
            while (true)  //loop until valid input
            {
                Console.Write(Prompt);  //display prompt message before getting input
                line = Console.ReadLine();  //store input

                if (double.TryParse(line, out rtn))  //if string can convert to double
                {
                    // Successfully parsed, exit the loop
                    break;
                }
                else
                {
                    // Invalid input, prompt the user again
                    System.Media.SystemSounds.Asterisk.Play();  //play a sound 
                }
            }

            if (rtn < Low || rtn > High)  //bounds check
            {
                System.Media.SystemSounds.Asterisk.Play();
                GetNumber(Prompt, Low, High);
            }

            return rtn;  //return number, all checks passed
        }

        static void Main()
        {
            Console.Title = "Computer Picker for NES Monopoly";  //console title
            Console.ForegroundColor = ConsoleColor.White;  //text color for console

            #region display program purpose
            //explain what this program is for
            Console.WriteLine("This program picks computer opponents for");
            Console.WriteLine("the NES version of Monopoly.  All you have");
            Console.WriteLine("to do is supply how many computer players");
            Console.WriteLine("are needed.  The game will determine the");
            Console.WriteLine("CPU players to use.  All output from the");
            Console.WriteLine("program  goes into a log file.\n \n");
            #endregion

            #region write data to array
            DateTime DT = DateTime.Now;
            ItemsToWrite[0] = DT.ToShortDateString();
            ItemsToWrite[1] = DT.ToShortTimeString();
            Double NumCPU = GetNumber("How many Computer opponents?  ", 1, 7);  //number of CPU opponents
            ItemsToWrite[2] = NumCPU.ToString();

            for (int i = 0; i < NumCPU; i++)  //loop once for each player picked
            {
                ItemsToWrite[3] += GetPlayer() + " - ";  //write player names to array
            }
            ItemsToWrite[4] = GameRule();
            WriteTodayLog(true);
            SpeakAndDisplay(true);
            #endregion

            TimeSpan elapsed = GameTimer(3 * NumCPU);
            ItemsToWrite[5] = elapsed.Hours + "H " + elapsed.Minutes + "M " + elapsed.Seconds + "S";

            double Assets = GetNumber("\n \nHow much was your Total assets?\n(0 if you went bankrupt.)  $", 0, 99999);
            ItemsToWrite[6] = "$" + Assets.ToString("n2");

            WriteTodayLog(false);
            WriteFullLog();
        }

        static TimeSpan GameTimer(double DisplayInterval)  //time how long the game takes
        {
            bool KeyPressed = false;  //timer stops on keypress
            TimeSpan interval = TimeSpan.FromMinutes(DisplayInterval);  //display timer every so often
            TimeSpan lastDisplayed = TimeSpan.Zero;  //last time the elapsed time was shown
            Stopwatch MonoTimer = new Stopwatch();
            SpeechSynthesizer MonoSpeak = new SpeechSynthesizer
            {
                Rate = 3 
            }; 

            Console.WriteLine("\n=============== PRESS A KEY TO START THE  TIMER ===============");
            Console.WriteLine("Elapsed time will be displayed and spoken every " + DisplayInterval.ToString() + " minutes.");
            Console.ReadKey(false);  //press key to start 
            Console.Write("TIMER:  STARTED\t\tELAPSED TIME:  ");
            MonoSpeak.SpeakAsync("Timer Started!"); 
            MonoTimer.Start(); // Start the timer

            while (!KeyPressed)
            {
                if (MonoTimer.Elapsed - lastDisplayed >= interval)
                {
                    Console.Write(MonoTimer.Elapsed.TotalMinutes.ToString("f0") + " "); 
                    MonoSpeak.SpeakAsync($"Elapsed time: {MonoTimer.Elapsed.TotalMinutes:F0} minutes");
                    lastDisplayed = MonoTimer.Elapsed;
                }

                if (Console.KeyAvailable)
                {
                    Console.ReadKey(true); // Capture key without echoing
                    KeyPressed = true;
                }
            }

            Console.Write("\t\tTIMER:  STOPPED");
            MonoSpeak.SpeakAsync("TIMER:  STOPPED");
            MonoTimer.Stop();
            return MonoTimer.Elapsed;
        }

        static void WriteTodayLog(bool Partial = true)
        {
            int Index;

            if ( Partial == true )
            { 
                Index = 4;
            }
            else
            {
                Index = 6;
            }

            StreamWriter TodayLog = new System.IO.StreamWriter(TodayLogPath);  //open a file for writing
            for (int i = 0;i <=Index; i++)
            {
                TodayLog.WriteLine(HeadersToWrite[i] + "  " + ItemsToWrite[i]);
            }
            TodayLog.WriteLine("\n==================================================\n");
            TodayLog.Close();
        }

        static void SpeakAndDisplay(bool Partial = true)
        {
            var MonoSpeak = new SpeechSynthesizer();
            MonoSpeak.Rate = 4;
            Console.WriteLine();

            int Index;

            if (Partial == true)
            {
                Index = 4;
            }
            else
            {
                Index = 6;
            }

            for (int i = 0; i <= Index; i++)
            {
                Console.WriteLine(HeadersToWrite[i] + "  " + ItemsToWrite[i]);
                MonoSpeak.SpeakAsync(HeadersToWrite[i] + ".  " + ItemsToWrite[i]);
            }
        }

        static void WriteFullLog()        
        {
            StreamWriter FullLog = new System.IO.StreamWriter(FullLogPath, true);  //open a file for writing
            FullLog.WriteLine(string.Join(",", ItemsToWrite));
            FullLog.Close();
        }

    } //end class
} //end namespace