using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;
using System.Threading;

namespace Quicktyper
{
    class Hiscores
    {
        // Lista för hiscores
        private List<Player> hiscores = new List<Player>();

        public Hiscores()
        {
            // Laddar fil ifall den finns
            if(File.Exists(@"hiscores.data") == true)
            {
                // Filestream öppnas för läsning av fil
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(@"hiscores.data", FileMode.Open, FileAccess.Read);
                // Loopar igenom filestreamen och sparar spelare i hiscores som objekt i listan hiscores.
                while (stream.Position < stream.Length)
                {
                    Player obj = (Player)formatter.Deserialize(stream);
                    hiscores.Add(obj);
                }
                stream.Close();
            }
        }
        // Lägga till poäng
        public Player addScore(Player player)
        {
            // Objektet player läggs till i listan
            hiscores.Add(player);
            // Sparnings- och serialiseringsfunktion körs
            marshall();
            return player;
        }
        // Återställa hiscores
        public void resetScore()
        {
            while (hiscores.Count > 0)
            {
                hiscores.RemoveAt(0);
            }
        }
        // Hämta hiscores
        public List<Player> getHiscores()
        {
            return hiscores;
        }
        // Funktion för att spara filer i en filestream, och serialisera datan
        private void marshall()
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(@"hiscores.data", FileMode.Create, FileAccess.Write);
            foreach(Player obj in hiscores)
            {
                formatter.Serialize(stream, obj);
            }
            stream.Close();
        }
    }

    [Serializable]
    // Klass för spelare
    public class Player
    {
        // De variabler som tillhör en spelare - namn, tid och svårighetsgrad
        private string name;
        private string time;
        private string difficulty;
        
        // Sätter och hämtar variabler
        public string Name
        {
            set { this.name = value; }
            get { return this.name; }
        }

        public string Time
        {
            set { this.time = value; }
            get { return this.time; }
        }

        public string Difficulty
        {
            set { this.difficulty = value; }
            get { return this.difficulty; }
        }
    }

    // Klass för att jämföra poäng (tid)
    class ScoreComparer : IComparer
    {
        // De två tiderna som ska jämföras
        int IComparer.Compare( Object x, Object y)
        {
            if (x == null)
            {
                return (y == null) ? 0 : 1;
            }
            if(y == null)
            {
                return -1;
            }
            // Player från klassen Player för att få tiden 
            Player p1 = x as Player;
            Player p2 = y as Player;
            
            // Gör om tiden (sträng) till decimal som sedan görs om till decimalfritt tal
            return Convert.ToInt32(decimal.Truncate(decimal.Parse(p1.Time))) - Convert.ToInt32(decimal.Truncate(decimal.Parse(p2.Time)));
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            // Skapar en instans av klassen Hiscores
            Hiscores hiscore = new Hiscores();
            while (true)
            {
                // Lätta meningar
                ArrayList wordsEasy = new ArrayList();
                wordsEasy.Add("Furnance");
                wordsEasy.Add("Banana");
                wordsEasy.Add("Street");
                wordsEasy.Add("Car");
                wordsEasy.Add("Cinema");

                // Medelsvåra meningar
                ArrayList wordsMedium = new ArrayList();
                wordsMedium.Add("He found a leprechaun in his walnut shell.");
                wordsMedium.Add("Two more days and all his problems would be solved.");
                wordsMedium.Add("He was sitting in a trash can with high street class.");
                wordsMedium.Add("He had decided to accept his fate of accepting his fate.");
                wordsMedium.Add("He hated that he loved what she hated about hate.");

                // Svåra meningar
                ArrayList wordsHard = new ArrayList();
                wordsHard.Add("She moved forward only because she trusted that the ending she now was going through must be followed by a new beginning.");
                wordsHard.Add("One small action would change her life, but whether it would be for better or for worse was yet to be determined.");
                wordsHard.Add("He learned the hardest lesson of his life and had the scars, both physical and mental, to prove it.");
                wordsHard.Add("The tattered work gloves speak of the many hours of hard labor he endured throughout his life.");
                wordsHard.Add("When motorists sped in and out of traffic, all she could think of was those in need of a transplant.");

                // Den lista som kommer att användas i spel, easy/medium/hard kommer att tilldelas i denna
                ArrayList words = new ArrayList();

                // Rensar konsollen och visar startalternativ
                Console.Clear();
                Console.WriteLine("Welcome to Quicktyper!");
                Console.WriteLine("1. Play");
                Console.WriteLine("2. Show hiscores");
                Console.WriteLine("3. Rules");
                Console.WriteLine("4. Reset hiscores");
                Console.WriteLine("X. Exit");

                // Skriver ut top 3 snabbaste rekorden (alla svårighetsgrader)
                int i = 1;
                Console.WriteLine("----- Top 3 Hiscores (Fastest/All difficulties) -----");
                ArrayList topThree = new ArrayList(hiscore.getHiscores());
                // Arraylisten sorteras enligt klassen ScoreComparer, för att den snabbaste ska få lägst index
                topThree.Sort(new ScoreComparer());
                int x = 0;
                foreach (Player player in topThree)
                {
                    if(x < 3)
                    {
                        Console.WriteLine("{0}. Player: {1} Time: {2}", i, player.Name, player.Time + " seconds.");
                        x++;
                        i++;
                    }
                }


                // Inmatning från spelare lagras i lowercase
                string inp = Console.ReadLine().ToLower();

                // Switch-stats för att hantera inmatning
                
                // Starta spelet
                switch (inp)
                {
                    case "1":
                        bool running = true;
                        bool correct = false;
                        string diff = "";
                        // Svårighetsgrad väljs
                        while (!correct)
                        {
                            // Konsollen rensas och alternativ för svårighetsgrad visas
                            Console.Clear();
                            Console.WriteLine("Please choose your difficulty setting.");
                            Console.WriteLine("1. Easy");
                            Console.WriteLine("2. Medium");
                            Console.WriteLine("3. Hard");
                            diff = Console.ReadLine();
                            
                            // ArrayListen "words" tilldelas med de meningar som tillhör respektive svårighetsgrad beroende på val
                            if( diff == "1")
                            {
                                words = wordsEasy;
                                correct = true;
                            }
                            if ( diff == "2") 
                            {
                                words = wordsMedium;
                                correct = true;
                            }
                            if ( diff == "3")
                            {
                                words = wordsHard;
                                correct = true;
                            }
                        }

                        // Variabler för att räkna rätt och fel
                        int right = 0;
                        int wrong = 0;
                        // Timer startar
                        Stopwatch sw = new Stopwatch();
                        sw.Start();
                        // Spel-loopen
                        while (running)
                        {
                            correct = false;
                            // Meningarna plockas fram i slumpmässig ordning
                            Random rnd = new Random();
                            int ind = rnd.Next(0, words.Count);
                            // Meningen som ska visas
                            string word = words[ind].ToString();
                            while (!correct)
                            {
                                // Meningen visas och inmatning av ordet begärs
                                Console.Clear();
                                Console.WriteLine(word);
                                string ans = Console.ReadLine();
                                if (ans == word)
                                {
                                    // Vid rätt svar tas meningen bort ur ArrayListen "words" och loopen körs igen
                                    words.RemoveAt(ind);
                                    correct = true;
                                    right++;
                                }
                                if (ans != word)
                                {
                                    wrong++;
                                }
                                if (words.Count < 1)
                                {
                                    // Ifall det är 0 meningar kvar i arraylisten, stoppas spelet
                                    sw.Stop();
                                    running = false;
                                }
                            }
                        }
                        correct = false;
                        while (!correct)
                        {
                            // Spelrunda avklarad, tid visas och spelaren kan inmata sitt namn
                            Console.Clear();
                            // Visar tiden det tog att spela en runda
                            Console.WriteLine("Elapsed time: " + sw.ElapsedMilliseconds / 1000 + " seconds.");
                            // Visar hur många gånger man skrev rätt och fel
                            Console.WriteLine("Rätt: {0} Fel: {1}", right, wrong);
                            Console.Write("Enter your name: ");
                            // Variabler för namn, tid (från stopwatchen, millisekunder omgjort till sekunder), och svårighetsgrad baserat på det index man tidigare valt
                            string name = Console.ReadLine();
                            string time = (sw.ElapsedMilliseconds / 1000).ToString();
                            string[] difficulty = { "Easy", "Medium", "Hard" };
                            // Felhantering, namn måste minst innehålla ett tecken
                            if(name.Length > 0)
                            {
                                // Klassen player används för att skapa ett objekt
                                Player obj = new Player();
                                // Objektet tilldelas det givna namnet, tiden man fick och den svårighetsgrad man spelade på
                                obj.Name = name;
                                obj.Time = time;
                                obj.Difficulty = difficulty[Convert.ToInt32(diff)-1];
                                // Metoden addScore i klassen Hiscore används för att spara rekordet
                                hiscore.addScore(obj);
                                correct = true;
                            }
                            // Felmeddelande
                            else
                            {
                                Console.WriteLine("Name must contain at least 1 character.");
                            }
                        }

                        break;
                    // Hiscores
                    case "2":

                        Console.Clear();
                        // Tre separata listor för att visa rekorden inom olika svårighetsgrader
                        ArrayList scoreEasy = new ArrayList();
                        ArrayList scoreMedium = new ArrayList();
                        ArrayList scoreHard = new ArrayList();
                        // Går igenom alla hiscores och placerar rätt person under rätt svårighetsgrad
                        foreach(Player player in hiscore.getHiscores())
                        {
                            if (player.Difficulty.ToLower() == "easy")
                            {
                                scoreEasy.Add(player);
                            }
                            if (player.Difficulty.ToLower() == "medium")
                            {
                                scoreMedium.Add(player);
                            }
                            if (player.Difficulty.ToLower() == "hard")
                            {
                                scoreHard.Add(player);
                            }
                        }
                        // Sorterar för att visa snabbast med lägst index
                        scoreEasy.Sort(new ScoreComparer());
                        scoreMedium.Sort(new ScoreComparer());
                        scoreHard.Sort(new ScoreComparer());

                        // Utskrift till konsollen
                        i = 1;
                        Console.WriteLine("----- HISCORES (EASY) -----");
                        foreach(Player player in scoreEasy)
                        {
                            Console.WriteLine("{0}. Player: {1} Time: {2} seconds.", i, player.Name, player.Time);
                            i++;
                        }

                        i = 1;
                        Console.WriteLine("\n----- HISCORES (MEDIUM) -----");
                        foreach (Player player in scoreMedium)
                        {
                            Console.WriteLine("{0}. Player: {1} Time: {2} seconds.", i, player.Name, player.Time);
                            i++;
                        }
                        i = 1;
                        Console.WriteLine("\n----- HISCORES (HARD) -----");
                        foreach (Player player in scoreHard)
                        {
                            Console.WriteLine("{0}. Player: {1} Time: {2} seconds.", i, player.Name, player.Time);
                            i++;
                        }
                        Console.WriteLine("------------------------");
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                        break;
                    // Regler
                    case "3":
                        // Visar "regler" för spelet. 
                        Console.Clear();
                        Console.WriteLine("The purpose of this game is to complete a few given sentences as fast as possible. ");
                        Console.WriteLine("The sentences are case-sensitive, and you will be timed to see how fast you complete all sentences.");
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                        break;
                    // Rensa bort befintliga rekord
                    case "4":
                        correct = false;
                        while (!correct)
                        {
                            // Extra steg för att kontrollera att man verkligen vill ta bort rekorden
                            Console.Clear();
                            Console.WriteLine("Are you sure you want to delete all saved hiscores? (Y/N)");
                            string resp = Console.ReadLine().ToLower();
                            if (resp == "y") 
                            {
                                // Metoden resetScore kallas och hiscores nollställs
                                hiscore.resetScore();
                                correct = true;
                            }
                            if (resp == "n")
                            {
                                // Man går tillbaka till huvudmenyn
                                correct = true;
                            }
                        }
                        break;
                    // Avsluta applikationen
                    case "x":
                        Environment.Exit(0);
                        break;
                }
            }
        }
    }
}
