using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

class Program
{
    static void Main()
    {
        // Your large input string
        string input = "Your large string with <div>abc</div> placeholders.";

        // Your list with order and values
        List<Replacement> replacements = new List<Replacement>
        {
            new Replacement { Order = 1, Value = "Replacement1" },
            new Replacement { Order = 2, Value = "Replacement2" },
            // Add more replacements as needed
        };

        // Iterate through the list and replace placeholders in the input string
        foreach (var replacement in replacements)
        {
            string placeholder = $"<div>abc</div>"; // Define your placeholder here
            string replacementText = $"<div>{replacement.Value}</div>";
            input = ReplaceFirst(input, placeholder, replacementText);
        }

        // Print the modified input string
        Console.WriteLine(input);
    }

    static string ReplaceFirst(string input, string search, string replace)
    {
        int pos = input.IndexOf(search);
        if (pos < 0)
            return input;
        return input.Substring(0, pos) + replace + input.Substring(pos + search.Length);
    }
}

class Replacement
{
    public int Order { get; set; }
    public string Value { get; set; }
}
