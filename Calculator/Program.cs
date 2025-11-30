using System;

class Program
{
    static void Main(string[] args)
    {
        while (true)
        {
            double num1 = ReadNumber("First number: ");
            double num2 = ReadNumber("Second number: ");

            char op = ReadOperator();

            try
            {
                double result = Calculate(num1, num2, op);
                Console.WriteLine($"Result: {num1} {op} {num2} = {result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            Console.Write("Do you want to continue? (yes/no): ");
            string answer = Console.ReadLine().ToLower();

            if (answer != "yes")
            {
                Console.WriteLine("Bye Bye!");
                break;
            }

            Console.WriteLine();
        }
    }

    static double ReadNumber(string message)
    {
        while (true)
        {
            Console.Write(message);
            string input = Console.ReadLine();

            if (double.TryParse(input, out double value))
                return value;

            Console.WriteLine("Invalid number, try again.");
        }
    }

    static char ReadOperator()
    {
        while (true)
        {
            Console.Write("Enter operation (+, -, *, /): ");
            string input = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(input))
            {
                char op = input[0];
                if (op == '+' || op == '-' || op == '*' || op == '/')
                    return op;
            }

            Console.WriteLine("Invalid operation, try again.");
        }
    }

    static double Calculate(double a, double b, char op)
    {
        return op switch
        {
            '+' => a + b,
            '-' => a - b,
            '*' => a * b,
            '/' => b != 0 ? a / b : throw new Exception("Cannot divide by zero."),
            _ => throw new Exception("Unknown operation.") // Default case for invalid operations
        };
    }
}
