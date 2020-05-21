using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using WPF_Trello.Events;
using WPF_Trello.Models;

namespace WPF_Trello.Services
{
    public class EventBusService
    {
        private readonly ConcurrentDictionary<EventSubscriber, Func<IEvent, Task>> _subscribers;

        public EventBusService()
        {
            _subscribers = new ConcurrentDictionary<EventSubscriber, Func<IEvent, Task>>();
        }

        public IDisposable Subscribe<TEvent>(Func<TEvent, Task> handler) where TEvent : IEvent
        {
            var subscriber = new EventSubscriber(typeof(TEvent), sub => _subscribers.TryRemove(sub, out var _));

            _subscribers.TryAdd(subscriber, message => handler((TEvent)message));

            return subscriber;
        }

        public async Task Publish<TEvent>(TEvent message) where TEvent : IEvent
        {
            Type messageType = typeof(TEvent);

            var handler = _subscribers
                .Where(sub => sub.Key.MessageType == messageType)
                .Select(sub => sub.Value(message));

            await Task.WhenAll(handler);
        }
    }
}
