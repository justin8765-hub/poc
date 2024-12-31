using System;
using System.Collections.Generic;
using System.Linq;

public enum ItemType
{
    New,
    Rm,
    Rw
}

public class Item
{
    public int Id { get; set; }
    public ItemType Type { get; set; }
    public int? ParentId { get; set; }

    public override string ToString()
    {
        return $"Item(Id: {Id}, Type: {Type}, ParentId: {ParentId})";
    }
}

public class Program
{
    public static void Main()
    {
        // Sample list of items
        var items = new List<Item>
        {
            new Item { Id = 1, Type = ItemType.New },
            new Item { Id = 2, Type = ItemType.Rm, ParentId = 1 },
            new Item { Id = 3, Type = ItemType.Rw, ParentId = 1 },
            new Item { Id = 4, Type = ItemType.New },
            new Item { Id = 5, Type = ItemType.Rm, ParentId = 3 },
            new Item { Id = 6, Type = ItemType.Rw, ParentId = 4 },
            new Item { Id = 7, Type = ItemType.Rm, ParentId = 6 }
        };

        // Ensure parent-before-children ordering
        var sortedItems = EnsureChildrenAfterParents(items);

        // Output the sorted items
        foreach (var item in sortedItems)
        {
            Console.WriteLine(item);
        }
    }

    private static List<Item> EnsureChildrenAfterParents(List<Item> items)
    {
        // Dictionary for quick lookup of items by ID
        var itemLookup = items.ToDictionary(item => item.Id);

        // List to hold the sorted result
        var sortedList = new List<Item>();

        // HashSet to track visited items
        var visited = new HashSet<int>();

        // Recursive function to process an item and its parent
        void ProcessItem(Item item)
        {
            if (visited.Contains(item.Id))
                return;

            // Process parent first
            if (item.ParentId.HasValue && itemLookup.TryGetValue(item.ParentId.Value, out var parent))
            {
                ProcessItem(parent);
            }

            // Add the current item to the sorted list
            sortedList.Add(item);
            visited.Add(item.Id);
        }

        // Process all items in the input list
        foreach (var item in items)
        {
            ProcessItem(item);
        }

        return sortedList;
    }
}
