using ConsoleApi.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace ConsoleApi.Services
{
    internal class Excel
    {
        protected static void SetCaptionStyle(ExcelStyle style)
        {
            style.Fill.PatternType = ExcelFillStyle.Solid;
            style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(184, 204, 228));
            style.Font.Bold = true;
        }

        public static byte[] CreateBeFoodXlsx(string path)
        {
            var properties = new[]
            {
                new PropertyByName<BeFoodRestaurantInfo>("Id", p => p.restaurant_id),
                new PropertyByName<BeFoodRestaurantInfo>("Name", p => p.name),
                new PropertyByName<BeFoodRestaurantInfo>("Address", p => p.address),
                new PropertyByName<BeFoodRestaurantInfo>("ContactList", p => p.contact_list),
            };

            return CreateFileXlsx(properties, path);
        }

        public static byte[] CreateShopeeFoodXlsx(string path)
        {
            var properties = new[]
            {
                new PropertyByName<ShopeeFoodRestaurantInfo>("Id", p => p.restaurant_id),
                new PropertyByName<ShopeeFoodRestaurantInfo>("Name", p => p.name),
                new PropertyByName<ShopeeFoodRestaurantInfo>("Address", p => p.address),
                new PropertyByName<ShopeeFoodRestaurantInfo>("ContactList", p => string.Join(",",p.phones)),
            };

            return CreateFileXlsx(properties, path);
        }

        public static byte[] ExportToXlsx<T>(PropertyByName<T>[] properties, List<T> itemsToExport, int row, ExcelPackage package)
        {
            var worksheet = package.Workbook.Worksheets.FirstOrDefault();
            var manager = new PropertyManager<T>(properties.Where(p => !p.Ignore));

            foreach (var items in itemsToExport)
            {
                manager.CurrentObject = items;
                try
                {
                    manager.WriteToXlsx(worksheet, row++, false);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            package.Save();
            return package.GetAsByteArray();
        }

        private static byte[] CreateFileXlsx<T>(PropertyByName<T>[] properties, string path)
        {
            using (var stream = new MemoryStream())
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var xlPackage = new ExcelPackage(path))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add(typeof(T).Name);
                    var fWorksheet = xlPackage.Workbook.Worksheets.Add("DataForFilters");
                    fWorksheet.Hidden = eWorkSheetHidden.VeryHidden;

                    var manager = new PropertyManager<T>(properties.Where(p => !p.Ignore));
                    manager.CustomWriteCaption(worksheet, SetCaptionStyle);

                    worksheet.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    xlPackage.Save();
                }
                return stream.ToArray();
            }
        }
    }
}