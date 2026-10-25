using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ParallelOrderProcessingSystem;

class MySystem
{
    private volatile bool _isRunning = false;
    public bool IsRunning => _isRunning;

    public ConcurrentQueue<Order> Orders { get; private set; } = new ConcurrentQueue<Order>();

    private List<Thread> threads = new List<Thread>(5);
    private object locker = new object();
    private SemaphoreSlim semaphoreSlim = new SemaphoreSlim(2, 2);

    private int processedOrders = 0;
    public int ProcessedOrders { get => processedOrders; private set => processedOrders = value; }
    public decimal TotalPrice { get; private set; } = 0m;

    public MySystem(StreamWriter writer)
    {
        // 100 ORDERS INIT
        for (int i = 0; i < 100; i++)
        {
            Orders.Enqueue(new Order
            {
                OrderID = i + 1,
                Price = (decimal)RandomNumberGenerator.GetInt32(100, 1001),
                IsProcessed = false
            });
        }

        // THREAD PART
        for (int i = 0; i < 5; i++)
            threads.Add
            (
                new Thread(
                    () =>
                    {
                        while (IsRunning && Orders.TryDequeue(out var nextOrder))
                        {
                            semaphoreSlim.Wait();

                            try
                            {
                                ProcessOrder(nextOrder!);
                                Interlocked.Increment(ref processedOrders);

                                // manual locking version
                                bool lockTaken = false;
                                try
                                {
                                    Monitor.Enter(locker, ref lockTaken);

                                    TotalPrice += nextOrder!.Price;
                                    LogResult(writer, nextOrder);
                                }
                                finally
                                {
                                    if (lockTaken)
                                        Monitor.Exit(locker);
                                }

                                // lock version
                                //lock (locker)
                                //{
                                //    TotalPrice += nextOrder.Price;
                                //    MySystem.LogResult(writer, nextOrder);
                                //}
                            }
                            finally { semaphoreSlim.Release(); }
                        }
                    }
                )
            );
    }

    public static void LogResult(StreamWriter writer, Order order)
    {
        if (order is null)
            return;


        writer.WriteLine(
            $"OrderID: {order.OrderID}, " +
            $"Price: {order.Price}, " +
            $"IsProcessed: {order.IsProcessed}");

        writer.Flush();
    }

    private void ProcessOrder(Order order)
    {
        Thread.Sleep(200);
        order.IsProcessed = true;
    }

    public void Run()
    {
        _isRunning = true;
        try
        {
            foreach (var thread in threads)
            {
                thread.Start();
            }

            foreach (var thread in threads)
                thread.Join();
        }
        finally { _isRunning = false; }
    }


}
