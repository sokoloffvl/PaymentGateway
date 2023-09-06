using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Infrastructure.EventBus;
using NUnit.Framework;

namespace Application.UnitTests;

public class ConflatingQueueTests
{
    
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void GivenEnqueuedItem_WhenDequeue_ThenReturnsThatItem()
    {
        //Arrange
        var queue = new ConflatingQueue<string, string>(k => k);
        var item = Guid.NewGuid().ToString();
        queue.Enqueue(item);
        //Act
        var isDequeued = queue.TryDequeue(out var dequeuedItem);
        //Assert
        isDequeued.Should().BeTrue();
        dequeuedItem.Should().Be(item);
    }
    
    [Test]
    public void GivenEnqueuedItem_WhenTheSameItemEnqueuedTwice_ThenDequeuesItOnlyOnce()
    {
        //Arrange
        var queue = new ConflatingQueue<string, string>(k => k);
        var item = Guid.NewGuid().ToString();
        queue.Enqueue(item);
        queue.Enqueue(item);
        //Act
        var isDequeuedFirstTime = queue.TryDequeue(out var firstDequeuedItem);
        var isDequeuedSecondTime = queue.TryDequeue(out var secondDequeuedItem);
        //Assert
        isDequeuedFirstTime.Should().BeTrue();
        firstDequeuedItem.Should().Be(item);
        isDequeuedSecondTime.Should().BeFalse();
        secondDequeuedItem.Should().BeNull();
    }
    
    [Test]
    public void GivenEmptyQueue_WhenDequeueCalled_ThenDequeuesNothing()
    {
        //Arrange
        var queue = new ConflatingQueue<string, string>(k => k);
        //Act
        var isDequeued = queue.TryDequeue(out var dequeuedItem);
        //Assert
        isDequeued.Should().BeFalse();
        dequeuedItem.Should().BeNull();
    }
    
    [Test]
    public void GivenQueue_WhenMultipleConcurrentEnqueueCalls_ThenEnqueuesOneItemOnly()
    {
        //Arrange
        var queue = new ConflatingQueue<string, string>(k => k);
        var item = Guid.NewGuid().ToString();
        //Act
        Parallel.ForEach(Enumerable.Range(1, 10), 
            _ => queue.Enqueue(item));
        var isDequeuedFirstTime = queue.TryDequeue(out var firstDequeuedItem);
        var isDequeuedSecondTime = queue.TryDequeue(out var secondDequeuedItem);
        //Assert
        isDequeuedFirstTime.Should().BeTrue();
        firstDequeuedItem.Should().Be(item);
        isDequeuedSecondTime.Should().BeFalse();
        secondDequeuedItem.Should().BeNull();
    }
}