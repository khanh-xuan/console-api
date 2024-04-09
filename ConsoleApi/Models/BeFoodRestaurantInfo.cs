using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApi.Models
{
    internal class BeFoodRestaurantInfo
    {
        public int minimum_order_amount { get; set; }
        public string display_minimum_order_amount { get; set; }
        public int min_delivery_time { get; set; }
        public int delivery_time { get; set; }
        public string name { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string restaurant_id { get; set; }
        public string display_address { get; set; }
        public double rating { get; set; }
        public int review_count { get; set; }
        public int city { get; set; }
        public string address { get; set; }
        public string calling_Number { get; set; }
        public string email { get; set; }
        public string phone_no { get; set; }
        public string contact_list { get; set; }
        public double distance { get; set; }
        public string distance_text { get; set; }
        public bool is_closed { get; set; }
        public string status { get; set; }
        public string next_slot_time { get; set; }
        public string end_time { get; set; }
        public string out_of_radius { get; set; }
        public int feedback_status { get; set; }
        public int classify { get; set; }
        public string locale { get; set; }
        public double delivery_radius { get; set; }
        public int merchant_id { get; set; }
        public bool is_pickup_enable { get; set; }
        public double median_price { get; set; }
        public int merchant_category_id { get; set; }
        public string merchant_category_name { get; set; }
    }
}
