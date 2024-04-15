using System.Reflection.PortableExecutable;

namespace TP_JeuPendu_V2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + @"..\..\..\" + "words.csv";
            string files = File.ReadAllText(path);
            string[] words = files.Split('\n');

            for (int i = 0;i < words.Length;i++)
            {
                words[i] = words[i].Trim();
            }

            Dictionary<string,int> scoreDictionnay = new Dictionary<string,int>();

            foreach (string word in words)
            {
                string[] tab = word.Split(";");
                if (!String.IsNullOrEmpty(tab[0]))
                    scoreDictionnay.Add(tab[0], int.Parse(tab[1]));
            }


            bool continueGame = true;
            while (continueGame)
            {
                Console.Clear();
                bool gameRunning = true;
                int errorsNumber = 0;
                string charToFind = "";
                string[,] hangedTable =
                {
                    {"    "},
                    {"============\r\n\r\n    "},
                    {"|| //    "},
                        {"\r\n\r\n    "},
                    {"||//     "},
                        {"\r\n\r\n    "},
                    {"||/      "},
                        {"\r\n\r\n    "},
                    {"||      "},
                        {"\r\n\r\n    "},
                    {"||      "},
                        {"\r\n\r\n    "},
                    {"||      "},
                        {"\r\n\r\n    "},
                    {"||      "},
                        {"\r\n\r\n    "},
                    {"||\r\n\r\n    "},
                    {"||\r\n\r\n    "},
                    {"||\r\n\r\n=========="}
                };
                string[,] hangedBodyParts =
                {
                    {"|\r\n\r\n    "},
                    {" O\r\n\r\n    "},
                    {" |\r\n\r\n    "},
                    {"/|\r\n\r\n    "},
                    {"/|\\\r\n\r\n    "},
                    {"/ \r\n\r\n    "},
                    {"/ \\\r\n\r\n    "}
                };

                Random random = new Random();
                int randomIndex = random.Next(0, scoreDictionnay.Count);
                string wordChoosen = scoreDictionnay.ElementAt(randomIndex).Key.ToLower();

                Console.Clear();
                List<string> tryedLetter = new List<string>();
                List<string> wordToGuess = new List<string> { };
                for (int i = 0; i < wordChoosen.Length; i++)
                {
                    wordToGuess.Add("_");
                }

                while (gameRunning)
                {
                    DisplayHanged(hangedTable);
                    Console.WriteLine($"Le mot a deviné contient {wordChoosen.Length} caractères :");
                    foreach (string letter in wordToGuess)
                    {
                        Console.Write(letter);
                    }
                    Console.WriteLine();

                    bool incorrectChar = true;
                    while (incorrectChar)
                    {
                        DisplayTryedLetter(tryedLetter);
                        Console.WriteLine("Veuillez choisir un caractère à trouver dans le mot :");
                        charToFind = Console.ReadLine().ToLower();
                        if (charToFind.Length == 1)
                        {
                            incorrectChar = false;
                        }
                        else
                        {
                            Console.WriteLine("Erreur, vous devez ne rentrer qu'un seul caractère.");
                        }
                        foreach (string letter in tryedLetter)
                        {
                            if (letter == charToFind)
                            {
                                incorrectChar = true;
                                Console.WriteLine("Erreur, vous avez déjà essayer cette lettre.");
                            }
                        }
                    }
                    tryedLetter.Add(charToFind);
                    bool isPresentChar = false;
                    for (int i = 0; i < wordChoosen.Length; i++)
                    {
                        if (charToFind[0] == wordChoosen[i])
                        {
                            wordToGuess[i] = charToFind;
                            isPresentChar = true;
                        }
                    }
                    if (!isPresentChar)
                    {
                        if (errorsNumber == 0)
                        {
                            for (int j = 3; j < 8; j += 2)
                            {
                                hangedTable[j, 0] = hangedBodyParts[errorsNumber, 0];
                            }
                            errorsNumber++;
                        }
                        else if (errorsNumber == 1)
                        {
                            hangedTable[9, 0] = hangedBodyParts[errorsNumber, 0];
                            errorsNumber++;
                        }
                        else if (errorsNumber == 2)
                        {
                            for (int j = 11; j < 14; j += 2)
                            {
                                hangedTable[j, 0] = hangedBodyParts[errorsNumber, 0];
                            }
                            errorsNumber++;
                        }
                        else if (errorsNumber == 3 || errorsNumber == 4)
                        {
                            hangedTable[11, 0] = hangedBodyParts[errorsNumber, 0];
                            errorsNumber++;
                        }
                        else
                        {
                            hangedTable[15, 0] = hangedBodyParts[errorsNumber, 0];
                            errorsNumber++;
                        }
                    }

                    for (int i = 0; i <= (wordToGuess.Count - 1); i++)
                    {
                        Console.Write(wordToGuess[i]);
                    }

                    Console.Clear();

                    if (VictoryCondition(errorsNumber, wordToGuess, wordChoosen, tryedLetter.Count))
                    {

                        if (scoreDictionnay[wordChoosen] == 0 || scoreDictionnay[wordChoosen] > tryedLetter.Count)
                        {
                            scoreDictionnay[wordChoosen] = tryedLetter.Count;

                            using (StreamWriter sw = new StreamWriter(path))
                            {

                                foreach(var entry in scoreDictionnay)
                                {
                                    sw.WriteLine($"{entry.Key};{entry.Value}");
                                }

                            }

                            Console.WriteLine("Félicitation, c'est un nouveau record !"); 
                        } else
                        {
                            Console.WriteLine($"Le meilleur score pour le mot \"{wordChoosen}\" est de {scoreDictionnay[wordChoosen]} coups.");
                        }

                        Console.WriteLine();
                        DisplayHanged(hangedTable);
                        gameRunning = false;
                        Console.WriteLine();
                        Console.WriteLine("Voulez vous rejouez ? (y/n)");
                        string gameAgain = Console.ReadLine();

                        if (gameAgain == "n")
                        {
                            continueGame = false;
                        }
                    }

                }

            }


        }
        public static void DisplayHanged(string[,] table)
        {
            for (int i = 0; i < table.GetLength(0); i++)
            {
                Console.Write(table[i, 0]);
            }
            Console.WriteLine();
            Console.WriteLine();
        }
        public static void DisplayTryedLetter(List<string> tryedLetter)
        {
            Console.Write("Vous avez essayé les lettres suivantes : ");
            for (int i = 0; i < tryedLetter.Count; i++)
            {
                if (i == tryedLetter.Count - 1)
                {
                    Console.Write($"{tryedLetter[i]}.\n\n");
                }
                else
                {
                    Console.Write($"{tryedLetter[i]},");
                }
            }
        }
        public static bool VictoryCondition(int errorsNumber, List<string> wordToGuess, string wordChoosen, int score)
        {
            if (errorsNumber == 7)
            {
                Console.Clear();
                Console.WriteLine($"Vous avez perdu ! Le mot à deviner était : {wordChoosen}");
                return true;
            }
            int countCharac = 0;
            int countToWin = wordToGuess.Count;
            foreach (string charac in wordToGuess)
            {
                if (charac != "_")
                {
                    countCharac++;
                }
            }
            if (countCharac == countToWin)
            {
                Console.Clear();
                Console.WriteLine($"Bravo ! Vous avez trouvé le mot : {wordChoosen} en {score} coups.");
                return true;
            }
            return false;
        }
    }
}
