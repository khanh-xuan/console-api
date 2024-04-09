using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApi.Models
{
    internal class BeFoodRestaurantResult
    {
        public int Flag { get; set; }
        public string Message { get; set; }
        public BeFoodRestaurant Data { get; set; }
    }
}
