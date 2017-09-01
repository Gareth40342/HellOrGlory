using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HOGInfo.Model;

namespace HOGInfo
{
    class Program
    {
        static string SalesLocation = Environment.CurrentDirectory + "\\Data\\";
        static string PricesLocation = Environment.CurrentDirectory + "\\Data\\Prices\\priceList";

        static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("HOG Infomration");

                var files = Directory.GetFiles(SalesLocation);
                Console.WriteLine("\nList of Sales Files from \"" + SalesLocation + "\": \n");
                foreach(string file in files)
                    Console.WriteLine(Path.GetFileName(file));

                Console.Write("\nPLease enter in a sales file name: ");
                string input = Console.ReadLine().Trim();

                try
                {
                    CoffeeShopInfo info = CoffeeShopInfo.Build(PricesLocation, SalesLocation + input);

                    Console.WriteLine($"\n========={input} REPORT===========");

                    Console.WriteLine("\nTOTAL EARNINGS: £" + info.TotalEarnings());

                    var blendList = info.ListCoffeeBlendByPopularity();
                    if (blendList.Count > 0)
                        Console.WriteLine(
                            $"\n\nMOST POPULAR BLEND: {blendList.First().Coffee} - {blendList.First().Roast}");
                    else
                        Console.WriteLine("\n\nMOST POPULAR BLEND: N/A");

                    Console.WriteLine("\n\nCOFFEE BLENDS ORDERED BY MOST SOLD: ");
                    foreach (CoffeeSales coffeeBlend in blendList)
                        Console.WriteLine(
                            $"\nType: {coffeeBlend.Coffee.Drink}\nRoast: {coffeeBlend.Roast}\nQuanity sold: {coffeeBlend.Sold}");

                    Console.WriteLine("\n\nPERCENTAGE OF CUSTOMERS WHO ORDER EXTRAS: %" +
                                      info.ExtrasCustomerPercentage());

                    Console.WriteLine("\n\nEXTRAS ORDERED BY MOST REQUESTED: ");
                    foreach (ExtraOrder extra in info.ExtraOrdersWithoutNull())
                        Console.WriteLine($"\nExtra: {extra.Extra} \nQuantity: {extra.Quantity}");
                }
                catch (Exception e)
                {
                    if (e.InnerException != null)
                    {
                        Console.WriteLine("Error: " + e.InnerException.Message);
                    }
                    else
                    {
                        Console.WriteLine("Error: " + e.Message);
                    }
                }

                Console.WriteLine("\nPress and key to continue...");
                Console.ReadKey(true);
            }

        }

    }
}
