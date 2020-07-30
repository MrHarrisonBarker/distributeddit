using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EventBusRabbitMQ;
using EventBusRabbitMQ.Common;
using EventBusRabbitMQ.Events;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PostService
{
    // public interface IEventBusRabbitMQConsumer
    // {
    //     void Consume();
    //     void Disconnect();
    // }

    public class EventBusRabbitMQConsumer
    {
        private readonly IRabbitMQConnection _connection;
        private readonly IPostService _postService;

        public EventBusRabbitMQConsumer(IRabbitMQConnection connection, IPostService postService)
        {
            _connection = connection;
            _postService = postService;
        }

        private async void ReceivedEvent(object sender, BasicDeliverEventArgs e)
        {
            Console.WriteLine("received something");
            if (e.RoutingKey == EventBusConstants.UserPostQueue)
            {
                var message = Encoding.UTF8.GetString(e.Body.Span);
                // var basketCheckoutEvent = JsonConvert.DeserializeObject<BasketCheckoutEvent>(message);

                // EXECUTION : Call Internal Checkout Operation
                // var command = _mapper.Map<CheckoutOrderCommand>(basketCheckoutEvent);
                // var result = await _mediator.Send(command);
                Console.WriteLine($"Received message from the queue -> {message}");
            }
        }

        public void Consume()
        {
            // using (var channel = _connection.CreateModel())
            // {
            var channel = _connection.CreateModel();
            channel.QueueDeclare(queue: EventBusConstants.UserPostQueue, durable: false, exclusive: false,
                autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(channel);

            //Create event when something receive
            consumer.Received += (model, ea) =>
            {
                var message = Encoding.UTF8.GetString(ea.Body.Span);

                Console.WriteLine($"message type {ea.BasicProperties.Type}");

                if (ea.BasicProperties.Type == "rpc")
                {
                    Console.WriteLine($"got rpc call reply to -> {ea.BasicProperties.ReplyTo}");
                    
                    IEnumerable<WeatherForecast> response = null;
                    var props = ea.BasicProperties;
                    var replyProps = channel.CreateBasicProperties();
                    replyProps.CorrelationId = props.CorrelationId;

                    try
                    {
                        response = _postService.Get();
                    }
                    finally
                    {
                        Console.WriteLine("got service response now replying");
                        var responseMessage = response
                            .Select(p => $"{p.Date}, {p.Summary}, {p.TemperatureC}, {p.TemperatureF}").ToArray()
                            .ToString();
                        var responseBytes = Encoding.UTF8.GetBytes(responseMessage);
                        channel.BasicPublish(exchange: "", routingKey: props.ReplyTo,
                            basicProperties: replyProps, body: responseBytes);
                        channel.BasicAck(deliveryTag: ea.DeliveryTag,
                            multiple: false);
                    }
                }
                else
                {
                    Console.WriteLine($"Received message from the queue -> {message}");
                    channel.BasicAck(ea.DeliveryTag, false);
                }
            };

            channel.BasicConsume(EventBusConstants.UserPostQueue, false, consumer);
            // }
        }

        public void Disconnect()
        {
            _connection.Dispose();
        }
    }
}