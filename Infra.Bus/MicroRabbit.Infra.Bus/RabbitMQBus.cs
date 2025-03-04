

using System.Text;
using MediatR;
using MicroRabbit.Domain.Core.Bus;
using MicroRabbit.Domain.Core.Commands;
using MicroRabbit.Domain.Core.Events;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MicroRabbit.Infra.Bus
{
    public sealed class RabbitMQBus(IMediator mediator) : IEventBus
    {
        private readonly IMediator _mediator = mediator;
        private readonly Dictionary<string, List<Type>> _handlers = [];
        private readonly List<Type> _eventTypes = [];

        public void Publish<T>(T @event) where T : Event
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            var eventName = @event.GetType().Name;

            channel.QueueDeclare(queue: "test_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event));
            channel.BasicPublish(exchange: "", eventName, basicProperties: null, body: body);

            Console.WriteLine($" [x] Sent {typeof(T).Name}");
        }

        public Task SendCommand<T>(T command) where T : Command
        {
            return _mediator.Send(command);
        }

        public void Subscribe<T, TH>() where T : Event where TH : IEventHandler<T>
        {
            var eventName = typeof(T).Name;
            var handlerType = typeof(TH);

            if (!_eventTypes.Contains(typeof(T)))
            {
                _eventTypes.Add(typeof(T));
            }

            if (!_handlers.ContainsKey(eventName))
            {
                _handlers.Add(eventName, [handlerType]);
            }

            if (_handlers[eventName].Any(s => s.GetType() == handlerType))
            {
                throw new ArgumentException($"Handler for event {handlerType.Name} already registered for event {eventName}");
            }

            _handlers[eventName].Add(handlerType);
            StartBasicConsume<T>();

        }

        private void StartBasicConsume<T>() where T : Event
        {
            var factory = new ConnectionFactory() { HostName = "localhost", DispatchConsumersAsync = true };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            var eventName = typeof(T).Name;
            channel.QueueDeclare(eventName, durable: false, exclusive: false, autoDelete: false, arguments: null);
            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.Received += Consumer_Received;
            channel.BasicConsume(eventName, true, consumer);
        }

        private async Task Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            var eventName = e.RoutingKey;
            var message = Encoding.UTF8.GetString(e.Body.ToArray());
            try
            {
                await ProcessEvent(eventName, message).ConfigureAwait(false);
                Console.WriteLine($" [x] Received {eventName}");
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task ProcessEvent(string eventName, string message)
        {
            if (_handlers.ContainsKey(eventName))
            {
                var subscriptions = _handlers[eventName];
                foreach (var subscription in subscriptions)
                {
                    var handler = Activator.CreateInstance(subscription);
                    if (handler == null) continue;
                    var eventType = _eventTypes.SingleOrDefault(t => t.Name == eventName);
                    #pragma warning disable CS8604 // Possible null reference argument.
                    var @event = JsonConvert.DeserializeObject(message, eventType);
                    #pragma warning restore CS8604 // Possible null reference argument.
                    var concreteType = typeof(IEventHandler<>).MakeGenericType(eventType);
                    #pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                    #pragma warning disable CS8602 // Dereference of a possibly null reference.
                    await (Task)concreteType.GetMethod("Handle").Invoke(handler, [@event]);
                    #pragma warning restore CS8602 // Dereference of a possibly null reference.
                    #pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                }
            }
        }
    }
}