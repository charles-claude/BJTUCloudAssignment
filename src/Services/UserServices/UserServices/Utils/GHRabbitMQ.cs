using UserServices.Models;

namespace GHRabbitMQ
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
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
            _provider = serviceProvider;
            InitRabbitMQ();
        }

        private void InitRabbitMQ()
        {
            var factory = new ConnectionFactory { HostName = "localhost" };

            // create connection
            _connection = factory.CreateConnection();

            // create channel
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare("exchange", ExchangeType.Topic);
            _channel.QueueDeclare("Client", false, false, false, null);
            //_channel.QueueBind("demoClient", "Client", "Client", null);
            _channel.BasicQos(0, 1, false);

            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            using (IServiceScope scope = _provider.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<UserContext>();
                var userList = _context.UserItems.ToList();
            }
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ch, ea) =>
            {
                // received message
                var content = System.Text.Encoding.UTF8.GetString(ea.Body.ToArray());

                // handle the received message
                HandleMessage(content);
                _channel.BasicAck(ea.DeliveryTag, false);
            };

            consumer.Shutdown += OnConsumerShutdown;
            consumer.Registered += OnConsumerRegistered;
            consumer.Unregistered += OnConsumerUnregistered;
            consumer.ConsumerCancelled += OnConsumerConsumerCancelled;

            _channel.BasicConsume("Client", false, consumer);
            return Task.CompletedTask;
        }

        private void HandleMessage(string content)
        {
            // we just print this message
            using (IServiceScope scope = _provider.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<UserContext>();
                _logger.LogInformation($"consumer received {content}");
                var tokenId = content;
                var userList = _context.UserItems.Where(s => s.TokenId == tokenId).ToList();
                if (userList.Count() == 0)
                {
                    Sender.Send("Ticket", "Unknown");
                }
                else
                {
                    var user = userList[0];
                    var test = user.Id.ToString();
                    Sender.Send("Ticket", user.Id.ToString());
                }
            }
        }

        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            _logger.LogInformation($"connection shut down {e.ReplyText}");
        }

        private void OnConsumerConsumerCancelled(object sender, ConsumerEventArgs e)
        {
            _logger.LogInformation($"consumer cancelled");
        }

        private void OnConsumerUnregistered(object sender, ConsumerEventArgs e)
        {
            _logger.LogInformation($"consumer unregistered");
        }

        private void OnConsumerRegistered(object sender, ConsumerEventArgs e)
        {
            _logger.LogInformation($"consumer registered");
        }

        private void OnConsumerShutdown(object sender, ShutdownEventArgs e)
        {
            _logger.LogInformation($"consumer shutdown {e.ReplyText}");
        }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}
