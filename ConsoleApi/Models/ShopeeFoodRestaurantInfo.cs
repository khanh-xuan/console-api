using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApi.Models
{
    internal class ShopeeFoodRestaurantInfo
    {
        public int total_order { get; set; }
        public object rating { get; set; }
        public int city_id { get; set; }
        public List<string> phones { get; set; }
        public int restaurant_id { get; set; }
        public string restaurant_url { get; set; }
        public int brand_id { get; set; }
        public bool is_open { get; set; }
        public int contract_type { get; set; }
        public int id { get; set; }
        public string location_url { get; set; }
        public bool has_contract { get; set; }
        public bool is_quality_merchant { get; set; }
        public object label { get; set; }
        public int merchant_time { get; set; }
        public List<string> categories { get; set; }
        public List<string> cuisines { get; set; }
        public int service_type { get; set; }
        public string url_rewrite_name { get; set; }
        public bool is_foody_delivery { get; set; }
        public object rush_hour { get; set; }
        public int limit_distance { get; set; }
        public string image_name { get; set; }
        public int restaurant_status { get; set; }
        public object campaigns { get; set; }
        public string address { get; set; }
        public string name_en { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public int display_order { get; set; }
        public int delivery_id { get; set; }
        public int district_id { get; set; }
        public bool is_pickup { get; set; }
    }
}
