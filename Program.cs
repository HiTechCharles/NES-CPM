﻿using System;
using System.Diagnostics;
using System.IO;
using System.Speech.Synthesis;

namespace CPM  //Computer Picker for Monopoly, this won't work on apple II or IBM.
{
    internal class Program  
    {
        #region VARIABLES
        public static bool[] Chosen = new bool[8];  //has a player been chosen yet?
        public static string[] PlayerName = new string[8];  //player names
        public static uint seed = (uint)Math.Pow(System.DateTime.Now.TimeOfDay.TotalMilliseconds, 11.0 / 7.0);  //rng seed based on time of day
        public static Random RNG = new Random((int)seed);  //create random number generator
        public static string LogPath = Environment.GetEnvironmentVariable("onedriveconsumer") + "\\documents\\CPM\\";  //path to save log file
        public static string TodayLogPath = LogPath + "Today.txt";  //file name to write log
        public static string FullLogPath = LogPath + "Full.txt";  //file name to write log
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
            Law[15] = "Auction off the first Nunowned property you land on.";
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
            PlayerName[0] = "Arthur";   PlayerName[1] = "Gertrude";
            PlayerName[2] = "Erwin";    PlayerName[3] = "Maude";
            PlayerName[4] = "Carmen";   PlayerName[5] = "Isaac";
            PlayerName[6] = "Penelope"; PlayerName[7] = "Ollie";

            Console.Title = "Computer Picker for NES Monopoly";  //console title
            Console.ForegroundColor = ConsoleColor.White;  //text color for console

            //explain what this program is for
            Console.WriteLine("This program picks computer opponents for");
            Console.WriteLine("the NES version of Monopoly.  All you have");
            Console.WriteLine("to do is supply how many computer players");
            Console.WriteLine("are needed.  The game will determine the");
            Console.WriteLine("CPU players to use.  All output from the");
            Console.WriteLine("program  goes into a log file.");
            
            int NumCPU = GetNumber("How many Computer opponents?  ");  //number of CPU opponents
            Directory.CreateDirectory(LogPath);  //make sure directory exists
            StreamWriter TodayLog = new System.IO.StreamWriter(TodayLogPath);  //open a file for writing
            DateTime end = DateTime.Now;  //get current date & time
            
            //writes all choices into a log file
            TodayLog.WriteLine("     DATE & TIME:  " + end.ToShortDateString() + "  " + end.ToShortTimeString());
            TodayLog.WriteLine("   # CPU PLAYERS:  " + NumCPU.ToString());
                TodayLog.Write("CHOSEN CPU NAMES:  ");
           
            for (int i = 0; i < NumCPU; i++)  //loop once for each player picked
            {
                TodayLog.Write(GetPlayer() + ", ");  //write player names to file
            }
            TodayLog.WriteLine(); 
            TodayLog.WriteLine("GAME RULE CHOSEN:  " + GameRule());
            TodayLog.Close();

            string TodayLogContents = File.ReadAllText(TodayLogPath);
            SpeechSynthesizer MonoSpeak  = new SpeechSynthesizer();
            MonoSpeak.Rate = 4;

            Stopwatch MonoTimer = new Stopwatch();  
            Console.WriteLine(TodayLogContents);
            MonoSpeak.SpeakAsync(TodayLogContents);

            Console.WriteLine("\n======= Press a key to start the game timer =======");
            Console.ReadKey();
            Console.Write("TIMER:  STARTED\t\t");
            MonoTimer.Start();
            Console.ReadKey();
            Console.Write("TIMER:  STOPPED");
            MonoTimer.Stop();
            TimeSpan elapsed = MonoTimer.Elapsed;

            StreamWriter AddToLog = new StreamWriter(TodayLogPath, true);  //open a file for writing
            AddToLog.WriteLine($"    Elapsed Time:  {elapsed.Hours} hours, {elapsed.Minutes} minutes, {elapsed.Seconds} seconds");
            AddToLog.WriteLine("\n--------------------------------------------------\n");
            AddToLog.Close();

            TodayLogContents = File.ReadAllText(TodayLogPath);
            File.AppendAllText(FullLogPath, TodayLogContents);
        }
    
    } //end class
} //end namespace