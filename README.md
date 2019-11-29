# MessageBroker

## Getting Started
```csharp
HDByte.MessageBroker broker = new HDByte.BrokerManager.GetMessageBroker();
```

### Subscribing Examples
```csharp
broker.Subscribe<TestEvent>(TestEventAction, ActionThread.Ui);
broker.Subscribe<TestEvent2>(TestEventAction);
```

### Publishing Examples
```csharp
broker.Publish(new TestEvent() { Name = "Made Up Event" });
```