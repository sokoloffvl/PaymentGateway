using System.Collections.Concurrent;

namespace Infrastructure.EventBus;

public class ConflatingQueue<TItem, TKey> : IBackgroundQueue<TItem>, IDisposable
{
    private readonly Func<TItem, TKey> getKey;

    private readonly BlockingCollection<TKey> keyQueue = new(new ConcurrentQueue<TKey>());
    private readonly Dictionary<TKey, TItem> itemsByKey = new();
    private bool isDisposed;

    public ConflatingQueue(Func<TItem, TKey> getKey)
    {
        this.getKey = getKey;
    }

    public void Enqueue(TItem item)
    {
        var key = getKey(item);
        bool added;
        lock (itemsByKey)
        {
            if (itemsByKey.ContainsKey(key))
                added = false;
            else
            {
                added = true;
                itemsByKey[key] = item;
            }
        }

        if (added)
        {
            keyQueue.Add(key);
        }
    }

    public bool TryDequeue(out TItem? value)
    {
        value = default(TItem);
        if (!keyQueue.TryTake(out var key))
            return false;

        lock (itemsByKey)
        {
            if (!itemsByKey.TryGetValue(key, out var item))
                return false;

            value = item;
            itemsByKey.Remove(key);

            return true;
        }
    }

    public void Dispose()
    {
        keyQueue.Dispose();
    }
}