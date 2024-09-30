using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;  // Add Newtonsoft.Json NuGet package

public static class ObjectComparer
{
    // Function to compare two objects and get only the differences (old and new models separately)
    public static (Dictionary<string, object> oldValues, Dictionary<string, object> newValues) CompareObjects(object oldObject, object newObject)
    {
        var oldValues = new Dictionary<string, object>();
        var newValues = new Dictionary<string, object>();
        CompareRecursive(oldObject, newObject, oldValues, newValues, string.Empty);
        return (oldValues, newValues);
    }

    // Recursive function to compare properties of objects
    private static void CompareRecursive(object oldObj, object newObj, Dictionary<string, object> oldValues, Dictionary<string, object> newValues, string propertyName)
    {
        if (oldObj == null && newObj == null)
            return;

        // If one is null or they are different, record the difference
        if (oldObj == null || newObj == null || !oldObj.Equals(newObj))
        {
            oldValues[propertyName] = oldObj;
            newValues[propertyName] = newObj;
            return;
        }

        Type type = oldObj.GetType();

        // Handle collections like arrays and lists
        if (typeof(IEnumerable).IsAssignableFrom(type) && !(oldObj is string))
        {
            CompareCollections(oldObj as IEnumerable, newObj as IEnumerable, oldValues, newValues, propertyName);
        }
        else
        {
            // Handle properties (both simple and complex types)
            foreach (PropertyInfo property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (property.CanRead)
                {
                    var oldValue = property.GetValue(oldObj);
                    var newValue = property.GetValue(newObj);
                    string propertyPath = string.IsNullOrEmpty(propertyName) ? property.Name : $"{propertyName}.{property.Name}";

                    // Only recurse if the property values are different
                    if (oldValue != null && newValue != null && !oldValue.Equals(newValue))
                    {
                        CompareRecursive(oldValue, newValue, oldValues, newValues, propertyPath);
                    }
                }
            }
        }
    }

    // Compare collections
    private static void CompareCollections(IEnumerable oldCollection, IEnumerable newCollection, Dictionary<string, object> oldValues, Dictionary<string, object> newValues, string propertyName)
    {
        var oldEnumerator = oldCollection?.GetEnumerator();
        var newEnumerator = newCollection?.GetEnumerator();

        int index = 0;

        while ((oldEnumerator?.MoveNext() ?? false) | (newEnumerator?.MoveNext() ?? false))
        {
            var oldValue = oldEnumerator?.Current;
            var newValue = newEnumerator?.Current;
            string indexedPropertyName = $"{propertyName}[{index}]";

            CompareRecursive(oldValue, newValue, oldValues, newValues, indexedPropertyName);
            index++;
        }
    }

    // Serialize the old and new models with only differences to a JSON string
    public static string GetDifferencesAsJson((Dictionary<string, object> oldValues, Dictionary<string, object> newValues) models)
    {
        var result = new
        {
            OldValues = models.oldValues,
            NewValues = models.newValues
        };
        
        return JsonConvert.SerializeObject(result, Formatting.Indented);
    }
}

// Example model class
public class SubModel
{
    public int SubId { get; set; }
    public string SubName { get; set; }
}

// Another example model class
public class TestClass
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<SubModel> SubModels { get; set; }
}

public class Program
{
    public static void Main()
    {
        // Old object
        var oldObj = new TestClass 
        { 
            Id = 1, 
            Name = "Old Name", 
            SubModels = new List<SubModel>
            {
                new SubModel { SubId = 1, SubName = "Old Sub 1" },
                new SubModel { SubId = 2, SubName = "Old Sub 2" }
            }
        };

        // New object
        var newObj = new TestClass 
        { 
            Id = 1,  // Same, should be excluded
            Name = "New Name",  // Different
            SubModels = new List<SubModel>
            {
                new SubModel { SubId = 1, SubName = "Old Sub 1" }, // Same, should be excluded
                new SubModel { SubId = 2, SubName = "New Sub 2" }  // Different
            }
        };

        // Compare objects and get differences (old and new values separately)
        var differences = ObjectComparer.CompareObjects(oldObj, newObj);

        // Convert differences (old and new values) to JSON string
        string jsonDifferences = ObjectComparer.GetDifferencesAsJson(differences);
        
        // Output the JSON
        Console.WriteLine(jsonDifferences);
    }
}
