namespace GHRabbitMQ
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using PaymentService;
    using PaymentService.Models;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;

    public class ConsumeRabbitMQHostedService : BackgroundService
    {
        private readonly ILogger _logger;
        private IConnection _connection;
        private IModel _channel;
        private readonly IServiceProvider _provider;

        public ConsumeRabbitMQHostedService(ILoggerFactory loggerFactory, IServiceProvider serviceProvider)
        {
            this._logger = loggerFactory.CreateLogger<ConsumeRabbitMQHostedService>();
            InitRabbitMQ();
            _provider = serviceProvider;
        }

        private void InitRabbitMQ()
        {
            var factory = new ConnectionFactory { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare("exchange", ExchangeType.Topic);
            _channel.QueueDeclare("Payment", false, false, false, null);
            _channel.BasicQos(0, 1, false);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ch, ea) =>
            {
                var content = System.Text.Encoding.UTF8.GetString(ea.Body.ToArray());
                HandleMessage(content);
                _channel.BasicAck(ea.DeliveryTag, false);
            };

            _channel.BasicConsume("Payment", false, consumer);
            return Task.CompletedTask;
        }

        private void HandleMessage(string content)
        {
            long userID = long.Parse(content.Substring(0, content.IndexOf("_")));
            long orderID = long.Parse(content.Substring(content.IndexOf("_")+1));
            using (IServiceScope scope = _provider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<PaymentContext>();
                PaymentItem paymentItem = new PaymentItem();
                paymentItem.OrderID = orderID;
                paymentItem.UserID = userID;
                context.payments.Add(paymentItem);
                Sender.Send("Ticket", "OK");
            }
        }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}
