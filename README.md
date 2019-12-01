# MessageBroker

## Getting Started
```csharp
HDByte.MessageBroker broker = HDByte.BrokerManager.GetMessageBroker();
```

### Subscribing Examples
```csharp
broker.Subscribe<NewCustomerEvent>(OnNewCustomer, ActionThread.Ui);
broker.Subscribe<NewOrderEvent>(OnNewOrder);

private void OnNewCustomer(NewCustomer e)
{
    // Do Something with e.Name
}

private void OnNewOrder(NewOrder e)
{
    // Do Something
}
```

### Publishing Examples
```csharp
broker.Publish(new NewCustomerEvent() { Name = "Fake Name" });
```