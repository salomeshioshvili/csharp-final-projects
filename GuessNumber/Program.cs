using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

class Program
{
    const string ScoreFile = "scores.csv";

    static void Main()
    {
        Console.Write("Enter your name: ");
        string playerName = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(playerName))
            playerName = "Unknown";

        while (true)
        {
            Console.WriteLine("\n1) Play");
            Console.WriteLine("2) Show TOP 10");
            Console.WriteLine("3) Exit");
            Console.Write("Choose: ");

            string choice = Console.ReadLine();

            if (choice == "1")
                Play(playerName);
            else if (choice == "2")
                ShowTop10();
            else if (choice == "3")
                break;
            else
                Console.WriteLine("Invalid option.");
        }
    }

    static void Play(string playerName)
    {
        int max = ChooseDifficulty();
        int secret = new Random().Next(1, max + 1); // pick a secret number randomly from 1 through max
        int attempts = 10;

        Console.WriteLine($"I chose a number between 1 and {max}. You have {attempts} attempts.");

        for (int i = 1; i <= attempts; i++)
        {
            int guess = ReadInt($"Attempt {i}: ", 1, max);

            if (guess == secret)
            {
                Console.WriteLine("Correct! You win!");
                int score = 11 - i;   // 1st try = 10 points so 10th try = 1 point
                SaveScore(playerName, score);
                return;
            }
            else if (guess < secret)
            {
                Console.WriteLine("Too low.");
            }
            else
            {
                Console.WriteLine("Too high.");
            }
        }

        Console.WriteLine($"You lost. The number was {secret}.");
        SaveScore(playerName, 0);
    }

    static int ChooseDifficulty()
    {
        while (true)
        {
            Console.WriteLine("Difficulty:");
            Console.WriteLine("1) Easy   (1-15)");
            Console.WriteLine("2) Medium (1-25)");
            Console.WriteLine("3) Hard   (1-50)");
            Console.Write("Choose: ");

            string input = Console.ReadLine();

            if (input == "1") return 15;
            if (input == "2") return 25;
            if (input == "3") return 50;

            Console.WriteLine("Invalid choice.");
        }
    }

    static int ReadInt(string msg, int min, int max)
    {
        while (true)
        {
            Console.Write(msg);
            string input = Console.ReadLine();

            if (int.TryParse(input, out int value))
            {
                if (value >= min && value <= max)
                    return value;
                Console.WriteLine($"Enter a number between {min} and {max}.");
            }
            else
            {
                Console.WriteLine("Invalid number.");
            }
        }
    }

    static void SaveScore(string name, int score)
    {
        try
        {
            Dictionary<string, int> scores = LoadScores();

            if (!scores.ContainsKey(name) || score > scores[name])
                scores[name] = score;

            using (StreamWriter sw = new StreamWriter(ScoreFile, false))
            {
                foreach (var s in scores)
                    sw.WriteLine($"{s.Key},{s.Value}");
            }
        }
        catch
        {
            Console.WriteLine("Error saving score.");
        }
    }

    static Dictionary<string, int> LoadScores()
    {
        var dict = new Dictionary<string, int>();

        if (!File.Exists(ScoreFile))
            return dict;

        try
        {
            foreach (string line in File.ReadAllLines(ScoreFile))
            {
                string[] parts = line.Split(',');
                if (parts.Length == 2 && int.TryParse(parts[1], out int score))
                    dict[parts[0]] = score;
            }
        }
        catch
        {
            Console.WriteLine("Error reading scores.");
        }

        return dict;
    }

    static void ShowTop10()
    {
        var scores = LoadScores();

        if (scores.Count == 0)
        {
            Console.WriteLine("No scores yet.");
            return;
        }

        Console.WriteLine("TOP 10 PLAYERS:");
        int rank = 1;

        foreach (var s in scores.OrderByDescending(s => s.Value).Take(10))
        {
            Console.WriteLine($"{rank}. {s.Key} - {s.Value}");
            rank++;
        }
    }
}
