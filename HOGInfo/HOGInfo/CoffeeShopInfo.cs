using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HOGInfo.Model;

namespace HOGInfo
{
    public class CoffeeShopInfo
    {
        #region ============BUILD==============================
        public List<Coffee> CoffeeList;
        public List<Sale> SalesList;

        /// <summary>
        /// Loads in data from the provided files and builds the sales and coffee lists.
        /// </summary>
        /// <param name="pricesLocation"></param>
        /// <param name="salesLocation"></param>
        /// <returns></returns>
        public static CoffeeShopInfo Build(string pricesLocation, string salesLocation)
        {
            CoffeeShopInfo info = new CoffeeShopInfo();

            info.CoffeeList = BuildCoffeeList(pricesLocation);
            info.SalesList = BuildSalesList(salesLocation, info.CoffeeList);

            return info;
        }

        /// <summary>
        /// Builds coffee list using given Csv file location
        /// </summary>
        /// <param name="csvLocation"></param>
        /// <returns></returns>
        public static List<Coffee> BuildCoffeeList(string csvLocation)
        {
            List<Coffee> coffeeList = new List<Coffee>();

            try
            {
                using (StreamReader reader = File.OpenText(csvLocation))
                {
                    //Consume Header
                    reader.ReadLine();

                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] values = line.Split(',');

                        coffeeList.Add(new Coffee
                        {
                            Drink = values[0].Trim(),
                            Price = decimal.Parse(values[1].Trim())
                        });
                    }

                    return coffeeList;
                }
            }
            catch (IOException e)
            {
                throw new IOException("An error occured while trying to read the file:\n"
                    + csvLocation + "Details\n" + e.Message);
            }
            catch (InvalidCastException e)
            {
                throw new InvalidCastException("There is a problem with formatting of the file:\n"
                                    + csvLocation + "Details\n" + e.Message);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Builds coffee list using given Csv file location and valid coffee list.
        /// </summary>
        /// <param name="csvLocation"></param>
        /// <param name="coffeeList"></param>
        /// <returns></returns>
        public static List<Sale> BuildSalesList(string csvLocation, List<Coffee> coffeeList)
        {
            List<Sale> salesList = new List<Sale>();

            try
            {
                using (StreamReader reader = File.OpenText(csvLocation))
                {
                    //Consume Header
                    reader.ReadLine();

                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] values = line.Split(',');

                        Coffee targetCoffee = coffeeList.Find(c => c.Drink == values[0]);
                        if (targetCoffee != null)
                        {
                            salesList.Add(new Sale
                            {
                                Coffee = targetCoffee,
                                Roast = values[1].Trim(),
                                Quantity = int.Parse(values[2].Trim()),
                                Extras = values[3].Trim()
                            });
                        }
                        else //Coffee does not exist
                        {
                            salesList = null;
                            break;
                        }
                    }

                    return salesList;

                }
            }
            catch (IOException e)
            {
                throw new IOException("An error occured while trying to read the file:\n"
                                      + csvLocation + "Details\n" + e.Message);
            }
            catch (InvalidCastException e)
            {
                throw new InvalidCastException("There is a problem with formatting of the file:\n"
                                               + csvLocation + "Details\n" + e.Message);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #endregion

        #region ============Tasks==============================
        /// <summary>
        /// Returns the total earnings from coffee sales.
        /// </summary>
        /// <returns></returns>
        public decimal TotalEarnings()
        {
            var blends = ListCoffeeBlendByPopularity();

            if (blends.Count < 1)
                return 0;
            else
                return ListCoffeeBlendByPopularity()
                    .Sum(s => s.Coffee.Price * s.Sold);
        }

        /// <summary>
        /// Returns a list of coffee blends inclluding the quantity sold
        /// ordered by descending quantity.
        /// </summary>
        /// <returns></returns>
        public List<CoffeeSales> ListCoffeeBlendByPopularity()
        {
            List<CoffeeSales> results = new List<CoffeeSales>();

            if (SalesList.Count < 1)
                return results;

            var coffeeBlends = SalesList.AsEnumerable()
                .Select(s => s.Blend).Distinct();

            foreach (string blend in coffeeBlends)
            {
                var targetSales = SalesList.AsEnumerable()
                    .Where(s => s.Blend == blend)
                    .Select(s => s).ToList();

                var amount = targetSales.Sum(s => s.Quantity);
                
                results.Add(new CoffeeSales
                {
                    Coffee = targetSales.First().Coffee,
                    Roast = targetSales.First().Roast,
                    Sold = amount
                });
            }

            return results.OrderByDescending(s => s.Sold).ToList();
        }

        /// <summary>
        /// Returns a list of extras with nulls including the quantity sold.
        /// Listed in descending order by quantity.
        /// </summary>
        /// <returns></returns>
        public List<ExtraOrder> ListExtraOrders()
        {
            List<ExtraOrder> extraOrders = new List<ExtraOrder>();

            if (SalesList.Count < 1)
                return extraOrders;

            var extras = (SalesList.AsEnumerable()
                    .Select(s => s.Extras)
                    .Distinct())
                .ToList();

            foreach (string extra in extras)
            {
                int quantity = SalesList.AsQueryable()
                    .Where(s => s.Extras == extra)
                    .Sum(s => s.Quantity);

                extraOrders.Add(new ExtraOrder
                {
                    Extra = extra,
                    Quantity = quantity
                });
            }

            return extraOrders.OrderByDescending(e => e.Quantity).ToList();
        }

        /// <summary>
        /// Returns a list of extras without nulls including the quantity sold.
        /// Listed in descending order by quantity.
        /// </summary>
        /// <returns></returns>
        public List<ExtraOrder> ExtraOrdersWithoutNull()
        {
            var extras = ListExtraOrders();
            extras.RemoveAll(i => i.Extra.ToLower() == "null");
            return extras;
        }

        /// <summary>
        /// Returns the percentage value of customers who wanted extras.
        /// </summary>
        /// <returns></returns>
        public decimal ExtrasCustomerPercentage()
        {
            var extras = ListExtraOrders();

            try
            {
                var nullCount = extras.AsEnumerable()
                    .Where(e => e.Extra.ToLower() == "null")
                    .Select(e => e.Quantity).First();

                var totalCount = extras.AsEnumerable().Sum(e => e.Quantity);

                return
                    100 - Math.Round((decimal)nullCount * 100 / totalCount, 2);
            }
            catch (InvalidOperationException e)
            {
                return 100;
            }
        }
        #endregion
    }
}
