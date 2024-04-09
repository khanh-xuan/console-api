using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ConsoleApi.Models;
using ConsoleApi.Services;
using log4net;
using Newtonsoft.Json;
using OfficeOpenXml;

namespace ConsoleApi
{
    internal class Program
    {
        private static readonly ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static int periodicItem = int.Parse(ConfigurationManager.AppSettings["PeriodicItem"]);
        private static int shopeeFoodRange = int.Parse(ConfigurationManager.AppSettings["ShopeeFoodRange"]);
        private static int maxJsonFileLength = int.Parse(ConfigurationManager.AppSettings["MaxJsonFileLength"]);

        //BeFood
        private static List<BeFoodRestaurant> listBeFoodRestaurant = new List<BeFoodRestaurant>();
        private static List<BeFoodRestaurantInfo> listBeFoodRestaurantInfor = new List<BeFoodRestaurantInfo>();
        private static int BeFoodMinId = int.Parse(ConfigurationManager.AppSettings["BeFoodMinId"]);
        private static int BeFoodMaxId = int.Parse(ConfigurationManager.AppSettings["BeFoodMaxId"]);
        private static string urlBeFoodGetToken = ConfigurationManager.AppSettings["BeFoodGetTokenUrl"];
        private static string urlBeFoodGetRestaurantInfo = ConfigurationManager.AppSettings["BeFoodGetRestaurantInfoUrl"];
        private static string beFoodFolderJson = ConfigurationManager.AppSettings["BeFoodFolderJson"];
        private static string beFoodFolderExcel = ConfigurationManager.AppSettings["BeFoodFolderExcel"];
        //ShopeeFood
        private static List<ShopeeFoodRestaurantInfo> listShopeeFoodRestaurantInfor = new List<ShopeeFoodRestaurantInfo>();
        private static string urlShopeeFoodGetRestaurantInfo = ConfigurationManager.AppSettings["ShopeeGetRestaurantInfoUrl"];
        private static int shopeeMinId = int.Parse(ConfigurationManager.AppSettings["ShopeeFoodMinId"]);
        private static int shopeeMaxId = int.Parse(ConfigurationManager.AppSettings["ShopeeFoodMaxId"]);
        private static string shopeeFoodFolderJson = ConfigurationManager.AppSettings["ShopeeFoodFolderJson"];
        private static string shopeeFoodFolderExcel = ConfigurationManager.AppSettings["ShopeeFoodFolderExcel"];


        private static string beFoodFilePathJson = $"{beFoodFolderJson}\\befood-{BeFoodMinId}-{BeFoodMaxId}-{DateTime.Now.ToString("ddMMyyyyHHmmss")}.json";
        private static string shopeeFoodFilePathJson = $"{shopeeFoodFolderJson}\\shopeefood-{shopeeMinId}-{shopeeMaxId}-{DateTime.Now.ToString("ddMMyyyyHHmmss")}.json";

        static void Main(string[] args)
        {
            Console.WriteLine("--------------Start----------------");
            GetBeFood();
            //GetShopeeFood();
            Console.Read();
        }

        static void GetBeFood()
        {
            var thread = new Thread(() =>
            {
                try
                {
                    Console.WriteLine($"----------GetBeFood---------Start: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");
                    //create file json
                    var initJson = listBeFoodRestaurant.Cast<object>().ToArray();
                    File.WriteAllText(beFoodFilePathJson, JsonConvert.SerializeObject(initJson));

                    //create file excel
                    string filePathExcel = $"{beFoodFolderExcel}\\befood-{BeFoodMinId}-{BeFoodMaxId}-{DateTime.Now.ToString("ddMMyyyyHHmmss")}.xlsx";
                    Excel.CreateBeFoodXlsx(filePathExcel);

                    int success = 0;
                    string accessToken = BeFood.GetAccessToken(urlBeFoodGetToken);
                    if (accessToken != null)
                    {
                        Console.WriteLine($"GetAccessToken Success : {accessToken}");
                        var token = JsonConvert.DeserializeObject<AccessToken>(accessToken);
                        Console.WriteLine("----------GetRestaurantInfo------------");
                        //Random rnd = new Random();

                        for (int i = BeFoodMinId; i <= BeFoodMaxId; i++)
                        {
                            //int sleep = rnd.Next(1000, 2000);
                            //Console.WriteLine($"Sleep {sleep}s + {i}");
                            //Thread.Sleep(sleep);
                            string restaurant = BeFood.GetRestaurantInfo(urlBeFoodGetRestaurantInfo, token.Access_Token, i.ToString());
                            if (restaurant != null)
                            {
                                Console.WriteLine("----------Get RestaurantInfor ID : " + i + "Success");
                                var restaurantJson = JsonConvert.DeserializeObject<BeFoodRestaurantResult>(restaurant);
                                if (restaurantJson.Data != null)
                                {
                                    listBeFoodRestaurant.Add(restaurantJson.Data);
                                    listBeFoodRestaurantInfor.Add(restaurantJson.Data.Restaurant_Info);
                                    success++;
                                }
                            }
                            else
                            {
                                Console.WriteLine("----------Get RestaurantInfor ID : " + i + "Fail");
                                accessToken = BeFood.GetAccessToken(urlBeFoodGetToken);
                                if (accessToken != null)
                                {
                                    Console.WriteLine($"Refresh AccessToken Success : {accessToken}");
                                    token = JsonConvert.DeserializeObject<AccessToken>(accessToken);
                                    restaurant = BeFood.GetRestaurantInfo(urlBeFoodGetRestaurantInfo, token.Access_Token, i.ToString());
                                    if (restaurant != null)
                                    {
                                        Console.WriteLine("----------Get RestaurantInfor ID : " + i + "Success");
                                        var restaurantJson = JsonConvert.DeserializeObject<BeFoodRestaurantResult>(restaurant);
                                        if (restaurantJson.Data != null)
                                        {
                                            listBeFoodRestaurant.Add(restaurantJson.Data);
                                            listBeFoodRestaurantInfor.Add(restaurantJson.Data.Restaurant_Info);
                                            success++;
                                        }
                                    }
                                }
                            }

                            if (i % periodicItem == 0)
                            {
                                // write data to json file
                                CreateBeFoodJson(beFoodFilePathJson, i);
                                // write data to excel file
                                CreateBeFoodExcel(filePathExcel);
                            }
                        }

                    }
                    Console.WriteLine("GetRestaurantInfo Success! Count: " + success);
                    Console.WriteLine($"----------GetBeFood---------End: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");

                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message);
                }
            });

            thread.Start();
        }

        static void GetShopeeFood()
        {
            Console.WriteLine($"----------GetShopeeFood---------Start: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");
            var thread = new Thread(() =>
            {
                try
                {
                    //create file json
                    var initJson = listShopeeFoodRestaurantInfor.Cast<object>().ToArray();
                    File.WriteAllText(shopeeFoodFilePathJson, JsonConvert.SerializeObject(initJson));

                    //create file excel
                    string filePathExcel = $"{shopeeFoodFolderExcel}\\shopeefood-{shopeeMinId}-{shopeeMaxId}-{DateTime.Now.ToString("ddMMyyyyHHmmss")}.xlsx";
                    Excel.CreateBeFoodXlsx(filePathExcel);
                    int success = 0;
                    for (int i = shopeeMinId; i <= shopeeMaxId; i += shopeeFoodRange)
                    {
                        var payload = new List<int>(i);
                        for (int j = 0; j < shopeeFoodRange; j++)
                        {
                            payload.Add(j + i);
                        }
                        var restaurant = ShopeeFood.GetRestaurantInfo(urlShopeeFoodGetRestaurantInfo, payload);
                        if (restaurant != null)
                        {
                            var restaurantJson = JsonConvert.DeserializeObject<ShopeeFoodRestaurantResult>(restaurant);
                            if (restaurantJson.reply != null && restaurantJson.reply.delivery_infos.Count > 0)
                            {
                                listShopeeFoodRestaurantInfor.AddRange(restaurantJson.reply.delivery_infos);

                                // write data to json file
                                CreateShopeeFoodJson(shopeeFoodFilePathJson,i);
                                // write data to excel file
                                CreateShopeeFoodExcel(filePathExcel);
                                listShopeeFoodRestaurantInfor = new List<ShopeeFoodRestaurantInfo>();
                                success += restaurantJson.reply.delivery_infos.Count;
                            }
                        }
                    }
                    Console.WriteLine("GetRestaurantInfo Success! Count: " + success);
                    Console.WriteLine($"----------GetShopeeFood---------End: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message);
                }
            });
            thread.Start();
        }
        static void CreateBeFoodExcel(string filePathExcel)
        {
            // write data to excel file
            var package = new ExcelPackage(filePathExcel);
            var worksheet = package.Workbook.Worksheets.FirstOrDefault();
            if (worksheet == null)
                throw new Exception("No worksheet found");
            int rowCount = worksheet.Dimension.End.Row + 1;
            var properties = new[]
            {
                new PropertyByName<BeFoodRestaurantInfo>("Id", p => p.restaurant_id),
                new PropertyByName<BeFoodRestaurantInfo>("Name", p => p.name),
                new PropertyByName<BeFoodRestaurantInfo>("Address", p => p.address),
                new PropertyByName<BeFoodRestaurantInfo>("ContactList", p => p.contact_list),
            };
            var bytes = Excel.ExportToXlsx(properties, listBeFoodRestaurantInfor, rowCount, package);
            File.WriteAllBytes(filePathExcel, bytes);
            listBeFoodRestaurantInfor = new List<BeFoodRestaurantInfo>();
        }
        static void CreateBeFoodJson(string filePathJson, int currentId)
        {
            long length = new FileInfo(filePathJson).Length;
            if (length >= maxJsonFileLength * 1024)
            {
                beFoodFilePathJson = $"{beFoodFolderJson}\\befood-{currentId}-{BeFoodMaxId}-{DateTime.Now.ToString("ddMMyyyyHHmmss")}.json";
                var dataJson = listBeFoodRestaurant.Cast<object>().ToArray();
                File.WriteAllText(beFoodFilePathJson, JsonConvert.SerializeObject(dataJson));
                listBeFoodRestaurant = new List<BeFoodRestaurant>();
            }
            else
            {
                string json = File.ReadAllText(filePathJson, Encoding.UTF8);
                List<BeFoodRestaurant> jsonData = JsonConvert.DeserializeObject<List<BeFoodRestaurant>>(json);
                jsonData.AddRange(listBeFoodRestaurant);
                listBeFoodRestaurant = new List<BeFoodRestaurant>();
                var dataJson = jsonData.Cast<object>().ToArray();
                File.WriteAllText(filePathJson, JsonConvert.SerializeObject(dataJson));
            }

        }

        static void CreateShopeeFoodExcel(string filePathExcel)
        {
            // write data to excel file
            var package = new ExcelPackage(filePathExcel);
            var worksheet = package.Workbook.Worksheets.FirstOrDefault();
            if (worksheet == null)
                throw new Exception("No worksheet found");
            int rowCount = worksheet.Dimension.End.Row + 1;
            var properties = new[]
            {
                new PropertyByName<ShopeeFoodRestaurantInfo>("Id", p => p.restaurant_id),
                new PropertyByName<ShopeeFoodRestaurantInfo>("Name", p => p.name),
                new PropertyByName<ShopeeFoodRestaurantInfo>("Address", p => p.address),
                new PropertyByName<ShopeeFoodRestaurantInfo>("ContactList", p => string.Join(",",p.phones)),
            };
            var bytes = Excel.ExportToXlsx(properties, listShopeeFoodRestaurantInfor, rowCount, package);
            File.WriteAllBytes(filePathExcel, bytes);
            listBeFoodRestaurantInfor = new List<BeFoodRestaurantInfo>();
        }
        static void CreateShopeeFoodJson(string filePathJson, int currentId)
        {
            long length = new FileInfo(filePathJson).Length;
            if (length >= maxJsonFileLength * 1024)
            {
                shopeeFoodFilePathJson = $"{shopeeFoodFolderJson}\\shopeefood-{currentId}-{shopeeMaxId}-{DateTime.Now.ToString("ddMMyyyyHHmmss")}.json";
                var dataJson = listShopeeFoodRestaurantInfor.Cast<object>().ToArray();
                File.WriteAllText(shopeeFoodFilePathJson, JsonConvert.SerializeObject(dataJson));
            }
            else
            {
                string json = File.ReadAllText(filePathJson, Encoding.UTF8);
                List<ShopeeFoodRestaurantInfo> jsonData = JsonConvert.DeserializeObject<List<ShopeeFoodRestaurantInfo>>(json);
                jsonData.AddRange(listShopeeFoodRestaurantInfor);
                var dataJson = jsonData.Cast<object>().ToArray();
                File.WriteAllText(filePathJson, JsonConvert.SerializeObject(dataJson));
            }
        }
    }
}
