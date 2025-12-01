using System;

// Data model for ATM user
public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PersonalNumber { get; set; } // unique
    public string Pin { get; set; } // 4-digit, unique
    public decimal Balance { get; set; }
}
