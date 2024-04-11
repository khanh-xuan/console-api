using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace ConsoleApi.Services
{
    internal class ShopeeFood
    {
        private static readonly ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        public static string GetRestaurantInfo(string url, List<double> restaurant_ids)
        {
            try
            {
                HttpWebRequest webrequest = (HttpWebRequest)WebRequest.Create(url);
                webrequest.Method = "POST";
                webrequest.ContentType = "application/json";
                webrequest.Headers.Add("X-Foody-Api-Version", ConfigurationManager.AppSettings["X-Foody-Api-Version"]);
                webrequest.Headers.Add("X-Foody-App-Type", ConfigurationManager.AppSettings["X-Foody-App-Type"]);
                webrequest.Headers.Add("X-Foody-Client-Language", ConfigurationManager.AppSettings["X-Foody-Client-Language"]);
                webrequest.Headers.Add("X-Foody-Client-Type", ConfigurationManager.AppSettings["X-Foody-Client-Type"]);
                webrequest.Headers.Add("X-Foody-Client-Version", ConfigurationManager.AppSettings["X-Foody-Client-Version"]);
                webrequest.Headers.Add("X-Foody-Client-Id", ConfigurationManager.AppSettings["X-Foody-Client-Id"] ?? string.Empty);


                using (var streamWriter = new StreamWriter(webrequest.GetRequestStream()))
                {
                    string json = new JavaScriptSerializer().Serialize(new
                    {
                        restaurant_ids = restaurant_ids
                    });
                    streamWriter.Write(json);
                }

                HttpWebResponse webresponse = (HttpWebResponse)webrequest.GetResponse();
                Encoding enc = Encoding.GetEncoding("utf-8");
                StreamReader responseStream = new StreamReader(webresponse.GetResponseStream(), enc);
                string result = string.Empty;
                result = responseStream.ReadToEnd();
                webresponse.Close();
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error("ShopeeFood GetRestaurantInfo --" + ex.ToString());

            }
            return null;
        }

    }
}
