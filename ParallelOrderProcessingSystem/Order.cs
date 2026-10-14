using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelOrderProcessingSystem;

internal class Order
{
    public int OrderID { get; set; }
    public decimal Price { get; set; }
    public bool IsProcessed { get; set; }
}
