using System;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using System.Collections.Generic;
using System.Linq;

namespace EventSubscription
{
    internal class Program
    {
        // name of the event hub
        private const string eventHubName = "{your event hub name}";
        // connection string to the Event Hubs
        private const string connectionString = "{your event hub connection string}";

        static void Main(string[] args)
        {
            EventHubProducerClient producerClient = null;
            EventDataBatch eventBatch = null;
            var order = default(Order);
            var productSelection = string.Empty;
            var orderNumber = string.Empty;
            var isRun = true;
            var productId = 0;
            var produts = default(List<Tuple<ProductEnum,int>>);

            while (isRun)
            {
                Console.WriteLine("*********** Welcom to Order Process System **************");
                Console.WriteLine("Please option to start order/payment (type 1 or 2)");
                Console.WriteLine("1. Order");
                Console.WriteLine("2. Payment");
                var option = Convert.ToInt32(Console.ReadLine());
                switch (option)
                {
                    case 1:
                        Console.WriteLine("Please select the product you want to Order (Type 1 to 3)");
                        produts = Constants.ProductPriceMap();
                        foreach (var product in produts)
                        {
                            Console.WriteLine($"{(int)product.Item1}. {product.Item1}");
                        }
                        Console.WriteLine("B. Type 'b' to go back <-----");
                        Console.WriteLine("Q. Type 'q' or 'exit' to quit");
                        productSelection = Console.ReadLine();
                        if (productSelection.Equals("b", StringComparison.InvariantCultureIgnoreCase))
                        {
                            continue;
                        }
                        else if (productSelection.Equals("q", StringComparison.InvariantCultureIgnoreCase) ||
                                productSelection.Equals("exit", StringComparison.InvariantCultureIgnoreCase))
                        {
                            break;
                        }
                        break;
                    case 2:
                        Console.WriteLine("Please enter the order number :");
                        Console.WriteLine("B. Type 'b' to go back <-----");
                        Console.WriteLine("Q. Type 'q' or 'exit' to quit");
                        orderNumber = Console.ReadLine();
                        if (orderNumber.Equals("b", StringComparison.InvariantCultureIgnoreCase))
                        {
                            continue;
                        }
                        break;
                    default:
                        break;
                }
               
                if (productSelection.Equals("q", StringComparison.InvariantCultureIgnoreCase) || 
                    productSelection.Equals("exit", StringComparison.InvariantCultureIgnoreCase))
                {
                    break;
                }
                else if (int.TryParse(productSelection,out productId) && productId <= produts.Count)
                {
                    isRun = false;
                }
                else
                {
                    Console.WriteLine("Invalid input, please try again.");
                }
            }
            if (!int.TryParse(productSelection, out productId))
            {
                Console.WriteLine("Invalid selection... Try again");
            }
            else
            {
                if (productId <= produts.Count)
                {
                    var product = (ProductEnum)productId;
                    Console.WriteLine("Please enter the quantity");
                    var quantity = Convert.ToInt32(Console.ReadLine());
                    var random = new Random();
                    order = new Order
                    {
                        OrderId = $"{DateTime.Now.Month}{DateTime.Now.Year}-{random.Next()}",
                        OrderDateTime = DateTime.Now,
                        OrderStatus = "Ordered",
                        PaymentStatus = "Pending",
                        ProductName = product.ToString(),
                        OrderAmount = quantity * produts.FirstOrDefault(p => p.Item1 == product)?.Item2 ?? 0,
                        Quantity = quantity
                    };
                    producerClient = new EventHubProducerClient(connectionString, eventHubName);
                    // Create a batch of events 
                    eventBatch = producerClient.CreateBatchAsync().Result;
                    var orderJsonString = System.Text.Json.JsonSerializer.Serialize(order);
                    if (!eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes(orderJsonString))))
                    {
                        // if it is too large for the batch
                        throw new Exception($"Event {order.OrderId} is too large for the batch and cannot be sent.");
                    }
                }

                try
                {
                    // Use the producer client to send the batch of events to the event hub
                    producerClient.SendAsync(eventBatch);
                    Console.WriteLine($"A batch of {order?.OrderId:numOfEvents} events has been published.");
                }
                finally
                {
                    producerClient.DisposeAsync();
                }
            }
        }
    }
}
