using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace ConsoleApi.Services
{
    internal class BeFood
    {
        private static readonly ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        public static string GetAccessToken(string url)
        {
            try
            {
                HttpWebRequest webrequest = (HttpWebRequest)WebRequest.Create(url);
                webrequest.Method = "POST";
                webrequest.ContentType = "application/json";
                webrequest.ContentLength = 0;
                HttpWebResponse webresponse = (HttpWebResponse)webrequest.GetResponse();
                Encoding enc = System.Text.Encoding.GetEncoding("utf-8");
                StreamReader responseStream = new StreamReader(webresponse.GetResponseStream(), enc);
                string result = string.Empty;
                result = responseStream.ReadToEnd();
                webresponse.Close();
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error("BeFood GetAccessToken --" + ex.ToString());
            }
            return null;
        }

        public static string GetRestaurantInfo(string url, string token, string id)
        {
            try
            {
                HttpWebRequest webrequest = (HttpWebRequest)WebRequest.Create(url);
                webrequest.Method = "POST";
                webrequest.ContentType = "application/json";
                webrequest.Headers.Add("Authorization", $"Bearer {token}");
                using (var streamWriter = new StreamWriter(webrequest.GetRequestStream()))
                {
                    string json = new JavaScriptSerializer().Serialize(new
                    {
                        restaurant_id = id,
                        latitude = "10.77253621500006",
                        longitude= "106.69798153800008"
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
                _logger.Error("BeFood GetRestaurantInfo --" + ex.ToString());

            }
            return null;
        }

    }
}
