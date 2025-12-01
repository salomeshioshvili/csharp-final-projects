using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

class Program
{
    const string UsersFile = "users.json";
    const string LogsFile = "logs.json";

    static List<User> users = new List<User>();
    static List<OperationLog> logs = new List<OperationLog>();
    static int nextUserId = 1;
    static Random random = new Random();

    static void Main()
    {
        LoadUsers();
        LoadLogs();

        while (true)
        {
            Console.WriteLine("\nATM System");
            Console.WriteLine("\n1) Register");
            Console.WriteLine("2) Login");
            Console.WriteLine("3) Exit");
            Console.Write("Choose: ");

            string choice = Console.ReadLine();

            if (choice == "1")
                RegisterUser();
            else if (choice == "2")
                Login();
            else if (choice == "3")
                break;
            else
                Console.WriteLine("Invalid option.");
        }
    }

    static void RegisterUser()
    {
        Console.Write("First name: ");
        string firstName = Console.ReadLine();

        Console.Write("Last name: ");
        string lastName = Console.ReadLine();

        Console.Write("Personal number: ");
        string personalNumber = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(firstName) ||
            string.IsNullOrWhiteSpace(lastName) ||
            string.IsNullOrWhiteSpace(personalNumber))
        {
            Console.WriteLine("All fields are required.");
            return;
        }

        // unique personal number
        foreach (var u in users)
        {
            if (u.PersonalNumber == personalNumber)
            {
                Console.WriteLine("User with this personal number already exists.");
                return;
            }
        }

        string pin = GenerateUniquePin();
        var user = new User
        {
            Id = nextUserId++,
            FirstName = firstName,
            LastName = lastName,
            PersonalNumber = personalNumber,
            Pin = pin,
            Balance = 0
        };

        users.Add(user);
        SaveUsers();

        Console.WriteLine($"\nUser registered successfully!");
        Console.WriteLine($"Your 4-digit PIN is: {pin}");
        Console.WriteLine("Please remember it.");
    }

    static void Login()
    {
        Console.Write("Personal number: ");
        string personalNumber = Console.ReadLine();

        Console.Write("PIN (4 digits): ");
        string pin = Console.ReadLine();

        User found = null;

        foreach (var u in users)
        {
            if (u.PersonalNumber == personalNumber && u.Pin == pin)
            {
                found = u;
                break;
            }
        }

        if (found == null)
        {
            Console.WriteLine("Wrong personal number or PIN.");
            return;
        }

        Console.WriteLine($"\nWelcome, {found.FirstName} {found.LastName}!");
        UserMenu(found);
    }

    static string GenerateUniquePin()
    {
        while (true)
        {
            int number = random.Next(1000, 10000);
            string pin = number.ToString();
            bool exists = false;

            foreach (var u in users)
            {
                if (u.Pin == pin)
                {
                    exists = true;
                    break;
                }
            }

            if (!exists)
                return pin;
        }
    }

    static void UserMenu(User user)
    {
        while (true)
        {
            Console.WriteLine("\nATM Menu");
            Console.WriteLine("\n1) Check balance");
            Console.WriteLine("2) Deposit money");
            Console.WriteLine("3) Withdraw money");
            Console.WriteLine("4) View operation history");
            Console.WriteLine("5) Logout");
            Console.Write("Choose: ");

            string choice = Console.ReadLine();

            if (choice == "1")
                CheckBalance(user);
            else if (choice == "2")
                Deposit(user);
            else if (choice == "3")
                Withdraw(user);
            else if (choice == "4")
                ShowHistory(user);
            else if (choice == "5")
                break;
            else
                Console.WriteLine("Invalid option.");
        }
    }


    static void CheckBalance(User user)
    {
        Console.WriteLine($"Your balance is: {user.Balance} GEL");

        string message = $"User named {user.FirstName} {user.LastName} checked balance on {DateTime.Now:dd.MM.yyyy}. Balance: {user.Balance} GEL";

        LogOperation(user, message);
    }

    static void Deposit(User user)
    {
        Console.Write("Enter amount to deposit: ");
        string input = Console.ReadLine();

        if (!decimal.TryParse(input, out decimal amount) || amount <= 0)
        {
            Console.WriteLine("Invalid amount.");
            return;
        }

        user.Balance += amount;
        SaveUsers();

        Console.WriteLine($"Deposit successful. New balance: {user.Balance} GEL");

        string message = $"User named {user.FirstName} {user.LastName} deposited {amount} GEL on {DateTime.Now:dd.MM.yyyy}. Current balance is {user.Balance} GEL";

        LogOperation(user, message);
    }

    static void Withdraw(User user)
    {
        Console.Write("Enter amount to withdraw: ");
        string input = Console.ReadLine();

        if (!decimal.TryParse(input, out decimal amount) || amount <= 0)
        {
            Console.WriteLine("Invalid amount.");
            return;
        }

        if (amount > user.Balance)
        {
            Console.WriteLine("Not enough balance.");
            return;
        }

        user.Balance -= amount;
        SaveUsers();

        Console.WriteLine($"Withdraw successful. New balance: {user.Balance} GEL");

        string message = $"User named {user.FirstName} {user.LastName} withdrew {amount} GEL on {DateTime.Now:dd.MM.yyyy}. Current balance is {user.Balance} GEL";

        LogOperation(user, message);
    }

    static void ShowHistory(User user)
    {
        Console.WriteLine("\nOperation History:");

        bool found = false;
        foreach (var log in logs)
        {
            if (log.UserId == user.Id)
            {
                Console.WriteLine($"[{log.Time}] {log.Message}");
                found = true;
            }
        }

        if (!found)
            Console.WriteLine("No operations yet.");
    }

    // Logging
    static void LogOperation(User user, string message)
    {
        var log = new OperationLog
        {
            UserId = user.Id,
            Message = message,
            Time = DateTime.Now
        };

        logs.Add(log);
        SaveLogs();
    }

    // JSON
    static void LoadUsers()
    {
        if (!File.Exists(UsersFile))
            return;

        try
        {
            string json = File.ReadAllText(UsersFile);
            var loaded = JsonSerializer.Deserialize<List<User>>(json);

            if (loaded != null)
                users = loaded;
            
            // find highest existing user id so next new user gets unique id
            int maxId = 0; 
            foreach (var u in users)
            {
                if (u.Id > maxId) maxId = u.Id;
            }
            nextUserId = maxId + 1;
        }
        catch
        {
            Console.WriteLine("Error reading users file.");
        }
    }

    static void SaveUsers()
    {
        try
        {
            // pretty print
            var options = new JsonSerializerOptions { WriteIndented = true }; 
            string json = JsonSerializer.Serialize(users, options);
            File.WriteAllText(UsersFile, json);
        }
        catch
        {
            Console.WriteLine("Error saving users file.");
        }
    }

    static void LoadLogs()
    {
        if (!File.Exists(LogsFile))
            return;

        try
        {
            string json = File.ReadAllText(LogsFile);
            var loaded = JsonSerializer.Deserialize<List<OperationLog>>(json);

            if (loaded != null)
                logs = loaded;
        }
        catch
        {
            Console.WriteLine("Error reading logs file.");
        }
    }

    static void SaveLogs()
    {
        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(logs, options);
            File.WriteAllText(LogsFile, json);
        }
        catch
        {
            Console.WriteLine("Error saving logs file.");
        }
    }
}

