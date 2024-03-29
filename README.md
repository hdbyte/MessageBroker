# MessageBroker

[![Nuget Version](https://img.shields.io/nuget/v/HDByte.MessageBroker.svg?style=flat-square)](https://www.nuget.org/packages/HDByte.MessageBroker/)
![Downloads](https://img.shields.io/nuget/dt/HDByte.MessageBroker)
![Lines Of Code](https://tokei.rs/b1/github/hdbyte/messagebroker)

![GitHub issues](https://img.shields.io/github/issues/hdbyte/messagebroker?style=flat-square)

## Quick Introduction
'MessageBroker' is an implementation of the Publish/Subscribe programming pattern and is used to announce and react to events asynchronously. An advantage of this pattern is that the sender and receiver are not coupled together.

By default, all events are processed on a background tasks unless otherwise specified. Events that require UI interaction can be run on the UI thread provided that the attribute ActionThread.UI is applied to the subscribe event.


### Getting Started
BrokerManager.GetBroker() is a static method that always returns the same instance of Broker.
```csharp
HDByte.MessageBroker.Broker Broker = BrokerManager.GetBroker();
```

### Example - Publishing
```csharp
public void PlaceFakeOrder()
{
    var order = new CustomerOrder() { Name = "John Smith", ItemName = "Mousepad", ItemCost = 10.99, ItemQuantity = 3 };
    Broker.Publish(order);
}

public class CustomerOrder
{
    public string Name {get; set; }
    public string ItemName { get; set; }
    public double ItemCost { get; set; }
    public int ItemQuantity { get; set; }

    public double GetTotalCost()
    {
        return ItemCost * ItemQuantity;
    }
}
```

### Example - Subscribing
Using ActionThread.UI because this event will update the UI. Remove this if the event does not need to update the UI.
Using ActionThread.Background will cause the events to run on the background thread. (Be careful of Thread Locking)
Using ActionThread.Task will cause the events to run in a task. (Default)
```csharp
Broker.Subscribe<CustomerOrder>(OnNewOrder, ActionThread.UI);
Broker.Subscribe<Item>(OnNewItem, ActionThread.Background);

private void OnNewOrder(CustomerOrder order)
{
    textBox1.Text = order.Name;
    Debug.WriteLine(order.GetTotalCost());
}

private void OnNewItem(Item item)
{
    Debug.WriteLine(item.Name));
}
```