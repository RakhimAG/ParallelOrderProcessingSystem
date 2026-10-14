using ParallelOrderProcessingSystem;
using System.Collections.Concurrent;
using System.Security.Cryptography;


using var writer = new StreamWriter("results.txt", append: true);

var system = new MySystem(writer);

// FINAL EXPECTED STATISTICS
int expectedProcessedOrder = system.Orders.Count;
decimal totalPriceExpected = system.Orders.Sum(o => o.Price);


system.Run();
Console.WriteLine($"Processed orders : {system.ProcessedOrders}, Expected => {expectedProcessedOrder}");
Console.WriteLine($"Total price of orders : {system.TotalPrice}, Expected => {totalPriceExpected}");