using System;
using System.Collections.Generic;
using System.Diagnostics.PerformanceData;
using System.Linq;
using System.Text.Json;
using System.Windows;
using client.Controller;

namespace client.Models
{
    public class CalculatedProduct : Product
    {
        public int Count { get; set; }

        public void SetCount()
        {
            Count = GetCount();
        }
    
}
}