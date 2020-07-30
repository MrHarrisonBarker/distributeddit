using EventBusRabbitMQ.Events;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Text;
using EventBusRabbitMQ.Common;
using RabbitMQ.Client.Events;

namespace EventBusRabbitMQ.Producer
{

    public class EventBusRabbitMQProducer
    {
        private readonly IRabbitMQConnection _connection;

        public EventBusRabbitMQProducer(IRabbitMQConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        public void PublishTest(string queueName, string msg)
        {
            using (var channel = _connection.CreateModel())
            {
                Console.WriteLine("Starting to publish test msg");

                channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false,
                    arguments: null);

                var body = Encoding.UTF8.GetBytes(msg);

                IBasicProperties properties = channel.CreateBasicProperties();
                properties.Persistent = true;
                properties.DeliveryMode = 2;

                channel.ConfirmSelect();
                channel.BasicPublish(exchange: "", routingKey: queueName, mandatory: true, basicProperties: properties,
                    body: body);
                // channel.WaitForConfirmsOrDie();

                channel.BasicAcks += (sender, eventArgs) =>
                {
                    Console.WriteLine($"Sent RabbitMQ Test msg {eventArgs.DeliveryTag}");
                    //implement ack handle
                };
                channel.ConfirmSelect();
            }
        }

        public void RpcCallBackTest(string queueName, string replyQueue, string msg)
        {
            using (var channel = _connection.CreateModel())
            {
                Console.WriteLine("Starting to publish rpc text callback msg");
                var consumer = new EventingBasicConsumer(channel);

                channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false,
                    arguments: null);

                var body = Encoding.UTF8.GetBytes(msg);

                IBasicProperties properties = channel.CreateBasicProperties();
                properties.DeliveryMode = 2;
                properties.Type = "rpc";
                properties.ReplyTo = replyQueue;

                channel.BasicPublish(exchange: "", routingKey: queueName, mandatory: true, basicProperties: properties,
                    body: body);

                channel.BasicConsume(consumer: consumer, queue: EventBusConstants.PostUserQueue, autoAck: true);

                consumer.Received += (model, ea) =>
                {
                    Console.WriteLine($"Rpc callback reply {ea.Body}");
                };
            }
        }

        public void PublishBasketCheckout(string queueName, BasketCheckoutEvent publishModel)
        {
            using (var channel = _connection.CreateModel())
            {
                channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false,
                    arguments: null);
                var message = JsonConvert.SerializeObject(publishModel);
                var body = Encoding.UTF8.GetBytes(message);

                IBasicProperties properties = channel.CreateBasicProperties();
                properties.Persistent = true;
                properties.DeliveryMode = 2;

                channel.ConfirmSelect();
                channel.BasicPublish(exchange: "", routingKey: queueName, mandatory: true, basicProperties: properties,
                    body: body);
                channel.WaitForConfirmsOrDie();

                channel.BasicAcks += (sender, eventArgs) =>
                {
                    Console.WriteLine("Sent RabbitMQ");
                    //implement ack handle
                };
                channel.ConfirmSelect();
            }
        }
    }
}