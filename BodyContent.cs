string html = "<Div><p>crazy</p><div class='aj'></div></div><p>abcd</p><div><div class='aj'></div>";
List<string> extractedStrings = new List<string>();

int startIndex = 0;
while ((startIndex = html.IndexOf("<div class='aj'></div>", startIndex)) != -1)
{
    int endIndex = html.IndexOf("<div class='aj'></div>", startIndex + 1);

    if (endIndex != -1)
    {
        string extracted = html.Substring(0, startIndex);
        extractedStrings.Add(extracted);

        // Remove the processed portion of the HTML string
        html = html.Substring(endIndex + "<div class='aj'></div>".Length);
    }
    else
    {
        break;
    }
}

// Add the remaining HTML content if any
if (!string.IsNullOrEmpty(html))
{
    extractedStrings.Add(html);
}

// Now you have a list of extracted strings to perform operations on
foreach (string extractedString in extractedStrings)
{
    // Perform your operations on each extracted string
    Console.WriteLine(extractedString);
}
