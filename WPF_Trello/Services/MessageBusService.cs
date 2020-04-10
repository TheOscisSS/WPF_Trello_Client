using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_Trello.Models;
using WPF_Trello.Messages;

namespace WPF_Trello.Services
{
    public class MessageBusService
    {
        private readonly ConcurrentDictionary<MessageSubscriber, Func<IMessage, Task>> _consumer;

        public MessageBusService()
        {
            _consumer = new ConcurrentDictionary<MessageSubscriber, Func<IMessage, Task>>();
        }

        public async Task SendTo<TReceiver>(IMessage message)
        {
            var messageType = message.GetType();
            var receiverType = typeof(TReceiver);

            var receivers = _consumer
                .Where(receiver => receiver.Key.MessageType == messageType
                    && receiver.Key.ReceiverType == receiverType)
                .Select(receiver => receiver.Value(message));

           await Task.WhenAll(receivers);
        } 

        public IDisposable Receive<TMessage>(object receiver, Func<TMessage, Task> handler) where TMessage : IMessage
        {
            var subscriber = new MessageSubscriber(receiver.GetType(), typeof(TMessage), sub => _consumer.TryRemove(sub, out var _));

            _consumer.TryAdd(subscriber, message => handler((TMessage)message));

            return subscriber;
        }

    }
}
