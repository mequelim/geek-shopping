using GeekShopping.OrderAPI.Data.DTOs;
using GeekShopping.OrderAPI.Data.Messages;
using GeekShopping.OrderAPI.Model;
using GeekShopping.OrderAPI.RabbitMQ.Sender.Interface;
using GeekShopping.OrderAPI.Repository.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace GeekShopping.OrderAPI.RabbitMQ.Consumer
{
    /// <summary>
    /// A background service that consumes messages from a RabbitMQ queue related to check out operations.
    /// </summary>
    /// <remarks>
    /// This class is responsible for setting up a RabbitMQ consumer that continuously listens to a defined queue for incoming messages.
    /// When a message is received, it processes the data, typically to manage order details.
    /// The processing of the message involves interactions with the <c>IOrderRepository</c>.
    /// </remarks>
    public class RabbitMqCheckoutConsumer(IServiceScopeFactory scopeFactory, IConfiguration configuration, IRabbitMqMessageSender rabbitMqMessageSender) : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory = scopeFactory
                                                              ?? throw new ArgumentNullException(nameof(scopeFactory));
        private readonly IConfiguration _configuration = configuration
                                                         ?? throw new ArgumentNullException(nameof(configuration));
        private IConnection? _connection;
        private IChannel? _channel;

        // Methods:
        /// <summary>
        /// Initializes the RabbitMQ connection and channel and declares the queue to be consumed.
        /// </summary>
        /// <param name="cancellationToken">A token to monitor for cancellation requests during the initialization of the RabbitMQ resources.</param>
        /// <returns>A task that represents the asynchronous initialization operation.</returns>
        private async Task InitializeAsync(CancellationToken cancellationToken)
        {
            IConfigurationSection rabbitConfigs = _configuration.GetSection("RabbitMQ");
            ConnectionFactory factory = new()
            {
                HostName = rabbitConfigs["HostName"]!,
                UserName = rabbitConfigs["UserName"]!,
                Password = rabbitConfigs["Password"]!
            };

            _connection = await factory.CreateConnectionAsync(cancellationToken);
            _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

            await _channel.QueueDeclareAsync(
                queue: rabbitConfigs["QueueName"]!,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: cancellationToken
            );
        }

        /// <summary>
        /// Processes an order based on the provided checkout header data and interacts with the order repository to store the order details.
        /// </summary>
        /// <param name="checkoutHeaderDto">The data transfer object containing checkout header information, including user details, payment information, and cart
        /// details.</param>
        /// <param name="orderRepository">The repository interface used to persist the order details to the database.</param>
        /// <returns>A task that represents the asynchronous operation of processing and storing the order.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="checkoutHeaderDto"/> parameter is null.</exception>
        private async Task ProcessOrder(CheckoutHeaderDto? checkoutHeaderDto, IOrderRepository orderRepository)
        {
            ArgumentNullException.ThrowIfNull(checkoutHeaderDto); // Throw an ArgumentNullException exception if checkoutHeaderDto is null.

            OrderHeader order = new()
            {
                CardNumber = checkoutHeaderDto.CardNumber,
                CouponCode = checkoutHeaderDto.CouponCode,
                Cvv = checkoutHeaderDto.Cvv,
                DiscountTotal = checkoutHeaderDto.DiscountTotal,
                Email = checkoutHeaderDto.Email,
                ExpiryMonthYear = checkoutHeaderDto.ExpiryMonthYear,
                FirstName = checkoutHeaderDto.FirstName,
                IsPaid = false,
                LastName = checkoutHeaderDto.LastName,
                OrderDetails = new List<OrderDetail>(),
                OrderTotalItems = checkoutHeaderDto.CartTotalItems,
                Phone = checkoutHeaderDto.Phone,
                PurchaseAmount = checkoutHeaderDto.PurchaseAmount,
                PurchaseDate = DateTime.SpecifyKind(checkoutHeaderDto.PuchaseDate, DateTimeKind.Utc),
                PurchaseTime = DateTime.SpecifyKind(checkoutHeaderDto.PurchaseTime, DateTimeKind.Utc),
                UserId = checkoutHeaderDto.UserId
            };

            foreach(CartDetailDto details in checkoutHeaderDto.CartDetails)
            {
                OrderDetail detail = new()
                {
                    ProductId = details.ProductId,
                    ProductName = details.Product!.Name,
                    Price = details.Product.Price,
                    Count = details.Count
                };

                order.OrderTotalItems += details.Count;
                order.OrderDetails.Add(detail);
            }

            await orderRepository.AddOrder(order);

            PaymentDto payment = new()
            {
                Name = $"{order.FirstName} ${order.LastName}",
                CardNumber = order.CardNumber,
                Cvv = order.Cvv,
                ExpiryMonthYear = order.ExpiryMonthYear,
                OrderId = order.Id,
                PurchaseAmount = order.PurchaseAmount,
                Email = order.Email
            };

            try
            {
                await rabbitMqMessageSender.SendMessage(payment, "orderPaymentProcessQueue");
            }
            catch(Exception exception)
            {
                Console.WriteLine($"An exception occurred when we tried to post a message on RabbitMQ: {exception}.");
                throw;
            }
        }

        /// <summary>
        /// Executes the background service, initializing the RabbitMQ connection, setting up the message consumer, and processing messages from the configured queue.
        /// </summary>
        /// <param name="stoppingToken">A cancellation token that indicates when the execution should be stopped.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous execution of the service.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the RabbitMQ channel is not properly initialized.</exception>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await InitializeAsync(stoppingToken);
            stoppingToken.ThrowIfCancellationRequested(); // If canceled, throws an exception.

            if(_channel is null) throw new ArgumentNullException(nameof(_channel));

            IConfigurationSection rabbitConfigs = _configuration.GetSection("RabbitMQ");
            AsyncEventingBasicConsumer consumer = new(_channel!);

            consumer.ReceivedAsync += async (_, evt) =>
            {
                try
                {
                    string content = Encoding.UTF8.GetString(evt.Body.ToArray());
                    Console.WriteLine($">>> Received message: {content}");
                    CheckoutHeaderDto? checkoutHeaderDto = JsonSerializer.Deserialize<CheckoutHeaderDto>(content);

                    await using AsyncServiceScope scope = _scopeFactory.CreateAsyncScope();
                    IOrderRepository orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();

                    await ProcessOrder(checkoutHeaderDto, orderRepository);
                    await _channel.BasicAckAsync(evt.DeliveryTag, multiple: false, stoppingToken); // Removes the message from the list.
                }
                catch(Exception exception)
                {
                    Console.WriteLine($">>>>> ERROR to process message: {exception}");
                }
            };

            await _channel.BasicConsumeAsync(rabbitConfigs["QueueName"]!, false, consumer, cancellationToken: stoppingToken);
        }

        /// <summary>
        /// Asynchronously releases the unmanaged resources used by the message consumer and performs cleanup operations.
        /// </summary>
        /// <return>A task that represents the asynchronous disposal operation.</return>
        public async ValueTask DisposeAsync()
        {
            if(_channel is not null) await _channel.DisposeAsync();
            if(_connection is not null) await _connection.DisposeAsync();

            base.Dispose();
        }
    }
}