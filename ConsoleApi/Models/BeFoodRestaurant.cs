using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApi.Models
{
    internal class BeFoodRestaurant
    {
        public object Configuration { get; set; }
        public object Restaurant_Tier { get; set; }
        public BeFoodRestaurantInfo Restaurant_Info { get; set; }
        public string currency_code { get; set; }
        public string currency { get; set; }
        public object categories { get; set; }
        public bool is_favorite { get; set; }
    }
}
