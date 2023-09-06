namespace Infrastructure.EventBus;

public interface IBackgroundQueue<TItem>
{
    public void Enqueue(TItem item);
    public bool TryDequeue(out TItem? value);
}