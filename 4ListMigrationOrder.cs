using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static void Main()
    {
        Dictionary<string, List<string>> listDependencies = new Dictionary<string, List<string>>
        {
            { "List1", new List<string> { "List2", "List3" } },
            { "List2", new List<string> { "List3" } },
            { "List3", new List<string>() },
            { "List4", new List<string> { "List5" } },
            { "List5", new List<string>() }
        };

        List<string> migrationOrder = GetMigrationOrder(listDependencies);

        Console.WriteLine("Migration Order:");
        foreach (var listName in migrationOrder)
        {
            Console.WriteLine(listName);
        }
    }

    static List<string> GetMigrationOrder(Dictionary<string, List<string>> listDependencies)
    {
        var visited = new HashSet<string>();
        var migrationOrder = new List<string>();

        foreach (var listName in listDependencies.Keys)
        {
            Visit(listName, listDependencies, visited, migrationOrder);
        }

        return migrationOrder;
    }

    static void Visit(string listName, Dictionary<string, List<string>> listDependencies, HashSet<string> visited, List<string> migrationOrder)
    {
        if (!visited.Contains(listName))
        {
            visited.Add(listName);

            foreach (var dependentList in listDependencies[listName])
            {
                Visit(dependentList, listDependencies, visited, migrationOrder);
            }

            migrationOrder.Add(listName);
        }
    }
}
