public class BulkObservableCollection<T> : ObservableCollection<T>
{
    public void AddRange(IEnumerable<T> items)
    {
        if (items == null) throw new ArgumentNullException(nameof(items));

        foreach (var item in items)
        {
            Items.Add(item); // Directly modifying the underlying collection
        }
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }
}

var myCollection = new BulkObservableCollection<MyItem>();
myCollection.AddRange(Enumerable.Range(1, 10000).Select(i => new MyItem { Name = $"Item {i}" }));
