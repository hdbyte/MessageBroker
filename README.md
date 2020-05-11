# MessageBroker

[![NuGet version (HDByte.MessageBroker)](https://img.shields.io/nuget/v/HDByte.MessageBroker.svg?style=flat-square)](https://www.nuget.org/packages/HDByte.MessageBroker/)

## Getting Started
```csharp
HDByte.MessageBroker broker = HDByte.BrokerManager.GetMessageBroker();
```
All published events are handled by a background thread running in MessageBroker, with a first in first out method.

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