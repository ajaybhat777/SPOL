using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;

public static class ObjectComparer
{
    public static (Dictionary<string, object> oldValues, Dictionary<string, object> newValues) CompareObjects(object oldObject, object newObject)
    {
        var oldValues = new Dictionary<string, object>();
        var newValues = new Dictionary<string, object>();
        CompareRecursive(oldObject, newObject, oldValues, newValues, string.Empty);
        return (oldValues, newValues);
    }

    private static void CompareRecursive(object oldObj, object newObj, Dictionary<string, object> oldValues, Dictionary<string, object> newValues, string propertyName)
    {
        // If both are null, or they are the same instance, skip
        if (oldObj == null && newObj == null)
            return;

        if (oldObj != null && newObj != null && oldObj.Equals(newObj))
            return;

        // If they differ or one is null, add to result
        if (IsSimpleType(oldObj) || IsSimpleType(newObj))
        {
            if (!Equals(oldObj, newObj))
            {
                oldValues[propertyName] = oldObj;
                newValues[propertyName] = newObj;
            }
            return;
        }

        Type type = oldObj?.GetType() ?? newObj?.GetType();

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
                    var oldValue = oldObj != null ? property.GetValue(oldObj) : null;
                    var newValue = newObj != null ? property.GetValue(newObj) : null;
                    string propertyPath = string.IsNullOrEmpty(propertyName) ? property.Name : $"{propertyName}.{property.Name}";

                    // Only recurse if the property values are different
                    if (!Equals(oldValue, newValue))
                    {
                        CompareRecursive(oldValue, newValue, oldValues, newValues, propertyPath);
                    }
                }
            }
        }
    }

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

    private static bool IsSimpleType(object obj)
    {
        if (obj == null) return true; // Treat null as a simple type for comparison
        Type type = obj.GetType();
        return type.IsPrimitive || type.IsValueType || type == typeof(string) || type == typeof(decimal);
    }

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

public class SubModel
{
    public int SubId { get; set; }
    public string SubName { get; set; }
}

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

        var differences = ObjectComparer.CompareObjects(oldObj, newObj);
        string jsonDifferences = ObjectComparer.GetDifferencesAsJson(differences);
        Console.WriteLine(jsonDifferences);
    }
}
