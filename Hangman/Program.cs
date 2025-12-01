using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

class Program
{
    static string xmlFile = "hangman_scores.xml";

    static List<string> words = new List<string>
    {
        "apple", "banana", "orange", "grape", "kiwi",
        "strawberry", "pineapple", "blueberry", "peach", "watermelon"
    };

    static void Main()
    {
        Console.Write("Enter your name: ");
        string name = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(name)) name = "Player";

        while (true)
        {
            Console.WriteLine("\n1) Play");
            Console.WriteLine("2) Show TOP 10");
            Console.WriteLine("3) Exit");

            Console.Write("Choose: ");
            string choice = Console.ReadLine();

            if (choice == "1") Play(name);
            else if (choice == "2") ShowTop10();
            else if (choice == "3") break;
            else Console.WriteLine("Invalid choice.");
        }
    }


    static void Play(string playerName)
    {
        Random rand = new Random();
        string word = words[rand.Next(words.Count)];
        char[] revealed = new string('_', word.Length).ToCharArray();

        int wrong = 0;
        int maxWrong = 6;

        Console.WriteLine("\nThe word has " + word.Length + " letters.");
        Console.WriteLine(new string(revealed));

        while (wrong < maxWrong && new string(revealed).Contains("_"))
        {
            Console.Write("Enter a letter: ");
            string input = Console.ReadLine().ToLower();

            if (string.IsNullOrWhiteSpace(input) || input.Length != 1)
            {
                Console.WriteLine("Please enter one letter.");
                continue;
            }

            char letter = input[0];

            if (word.Contains(letter))
            {
                Console.WriteLine("Correct");

                for (int i = 0; i < word.Length; i++)
                {
                    if (word[i] == letter)
                        revealed[i] = letter;
                }
            }
            else
            {
                wrong++;
                Console.WriteLine("Wrong! Remaining tries: " + (maxWrong - wrong));
            }

            Console.WriteLine(new string(revealed));
        }

        // win by writing full word correctly
        if (!new string(revealed).Contains("_"))
        {
            Console.Write("Guess the full word: ");
            string guess = Console.ReadLine();

            if (guess == word)
            {
                Console.WriteLine("You won!");
                int score = (maxWrong - wrong) * 10;
                SaveScore(playerName, score);
                return;
            }
            else
            {
                Console.WriteLine("Wrong! You lose.");
                SaveScore(playerName, 0);
                return;
            }
        }

        Console.WriteLine("You lost! The word was: " + word);
        SaveScore(playerName, 0);
    }


    static void SaveScore(string name, int score) // saving to XML
    {
        XDocument doc;

        if (!File.Exists(xmlFile))
        {
            doc = new XDocument(new XElement("Scores"));
        }
        else
        {
            doc = XDocument.Load(xmlFile);
        }

        XElement existing = null;

        foreach (var player in doc.Root.Elements("Player"))
        {
            if (player.Element("Name").Value == name)
            {
                existing = player;
                break;
            }
        }

        if (existing == null)
        {
            doc.Root.Add(new XElement("Player",
                new XElement("Name", name),
                new XElement("BestScore", score)));
        }
        else
        {
            int old = int.Parse(existing.Element("BestScore").Value);
            if (score > old)
                existing.Element("BestScore").Value = score.ToString();
        }

        doc.Save(xmlFile);
    }


    static void ShowTop10()
    {
        if (!File.Exists(xmlFile))
        {
            Console.WriteLine("No scores yet.");
            return;
        }

        XDocument doc = XDocument.Load(xmlFile);

        List<(string Name, int Score)> list = new List<(string, int)>();

        foreach (var p in doc.Root.Elements("Player"))
        {
            string name = p.Element("Name").Value;
            int score = int.Parse(p.Element("BestScore").Value);
            list.Add((name, score));
        }

        list.Sort((a, b) => b.Score.CompareTo(a.Score)); // different way from GuessNumber

        Console.WriteLine("\nTOP 10 PLAYERS:");
        for (int i = 0; i < list.Count && i < 10; i++)
        {
            Console.WriteLine($"{i + 1}. {list[i].Name} - {list[i].Score}");
        }
    }
}
