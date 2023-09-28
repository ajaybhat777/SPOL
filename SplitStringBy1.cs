using System;

class Program
{
    static void Main()
    {
        string input = "1aaaabbbbccccdddd1hhhh";
        string[] parts = input.Split('1');
        string[] result = new string[parts.Length * 2];

        int index = 0;
        foreach (string part in parts)
        {
            result[index++] = part;
            if (!string.IsNullOrEmpty(part))
            {
                result[index++] = "1";
            }
        }

        // Now, 'result' contains the split parts and '1' as separate values in an array
        foreach (string value in result)
        {
            Console.WriteLine(value);
        }
    }
}
