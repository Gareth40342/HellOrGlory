using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HOGInfo;
using HOGInfo.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HOGInfoUnitTests
{
    [TestClass]
    public class UnitTest1
    {
        private CoffeeShopInfo _info;
        private Coffee coffee1;
        private Coffee coffee2;
        private Coffee coffee3;
        
        [TestInitialize]
        public void Init()
        {
           _info = new CoffeeShopInfo();
           _info.CoffeeList = new List<Coffee>();
           _info.SalesList = new List<Sale>();
           
            //Build test coffee list
            coffee1 = new Coffee()
            {
                Drink = "Coffee 1",
                Price = 4
            };

            coffee2 = new Coffee()
            {
                Drink = "Coffee 2",
                Price = 2
            };

            coffee3 = new Coffee()
            {
                Drink = "Coffee 3",
                Price = 6
            };

            _info.CoffeeList.Add(coffee1);
            _info.CoffeeList.Add(coffee2);
            _info.CoffeeList.Add(coffee3);

            //Build test sales list
            _info.SalesList.Add(new Sale { Coffee = coffee1, Roast = "Roast 1", Quantity = 5, Extras = "milk" });
            _info.SalesList.Add(new Sale { Coffee = coffee2, Roast = "Roast 3", Quantity = 4, Extras = "null" });
            _info.SalesList.Add(new Sale { Coffee = coffee2, Roast = "Roast 3", Quantity = 3, Extras = "milk" });
            _info.SalesList.Add(new Sale { Coffee = coffee2, Roast = "Roast 1", Quantity = 2, Extras = "soy" });
            _info.SalesList.Add(new Sale { Coffee = coffee3, Roast = "Roast 2", Quantity = 1, Extras = "null" });
        }

        [TestMethod]
        public void ListCoffeeBlendByPopularity()
        {
            //Expected Order: C2 R3 Q7, C1 R1 Q5, C2 R2 Q2, C3 R2 Q1
            Coffee[] expectedCoffeeOrder = {coffee2, coffee1, coffee2, coffee3};
            string[] expectedRoastOrder = {"Roast 3", "Roast 1", "Roast 1", "Roast 2"};
            int[] expectedQuantity = {7, 5, 2, 1};

            
            var blends = _info.ListCoffeeBlendByPopularity();

            for(int i = 0; i < blends.Count; i++)
            {
                if(blends[i].Coffee != expectedCoffeeOrder[i]||
                   blends[i].Roast != expectedRoastOrder[i] ||
                blends[i].Sold != expectedQuantity[i])
                    Assert.Fail();
            }
        }

        [TestMethod]
        public void TotalEarnings()
        {
            decimal expected = 44;
            var actual = _info.TotalEarnings();
            Assert.AreEqual(actual, expected);
        }

        [TestMethod]
        public void ListPopularExtras()
        {
            string[] expectedExtraOrder = {"milk", "null", "soy"};
            int[] expectedAmount = {8, 5, 2};

            var actual = _info.ListExtraOrders();

            for (int i = 0; i < actual.Count; i++)
            {
                if(actual[i].Extra != expectedExtraOrder[i] ||
                    actual[i].Quantity != expectedAmount[i])
                    Assert.Fail();
            }
        }

        [TestMethod]
        public void ListPopularExtrasWithoutNull()
        {
            string[] expectedExtraOrder = { "milk", "soy" };
            int[] expectedAmount = { 8, 2 };

            var actual = _info.ExtraOrdersWithoutNull();

            for (int i = 0; i < actual.Count; i++)
            {
                if (actual[i].Extra != expectedExtraOrder[i] ||
                    actual[i].Quantity != expectedAmount[i])
                    Assert.Fail();
            }
        }

        [TestMethod]
        public void ExtrasCustomerPercentage()
        {
            decimal expected = 66.67M;

            var actual = _info.ExtrasCustomerPercentage();

            Assert.AreEqual(actual, expected);
        }

        [TestMethod]
        public void ExtrasCustomerPercentageOfEmptySalesList() 
        {
            CoffeeShopInfo info = new CoffeeShopInfo();
            info.SalesList = new List<Sale>();

            decimal expected = 100;

            var actual = info.ExtrasCustomerPercentage();

            Assert.AreEqual(actual, expected);
        }

        [TestMethod]
        public void ExtrasCustomerPercentageWithNoNullExtras()
        {
            CoffeeShopInfo info = new CoffeeShopInfo();
            info.SalesList = new List<Sale>();
            info.SalesList.Add(new Sale { Coffee = coffee1, Extras = "milk", Roast = "Roast 1", Quantity = 2 });
            info.SalesList.Add(new Sale { Coffee = coffee3, Extras = "milk", Roast = "Roast 1", Quantity = 2 });

            decimal expected = 100;

            var actual = info.ExtrasCustomerPercentage();

            Assert.AreEqual(actual, expected);
        }

        [TestMethod]
        [ExpectedException(typeof(IOException))]
        public void BuildSalesListsWithInvalidFileLocation()
        {
            string testFile = "jkdljsdlkjskl";
            CoffeeShopInfo.BuildSalesList(testFile, _info.CoffeeList);
        }

        [TestMethod]
        [ExpectedException(typeof(IOException))]
        public void BuildCoffeeListsWithInvalidFileLocation()
        {
            string testFile = "jkdljsdlkjskl";
            CoffeeShopInfo.BuildCoffeeList(testFile);
        }

        [TestMethod]
        public void BuildSalesList()
        {
            string testFile = Environment.CurrentDirectory + "/TestData/validSales";
            var expected = _info.SalesList;
            
            var actual = CoffeeShopInfo.BuildSalesList(testFile, _info.CoffeeList);

            for (int i = 0; i < expected.Count; i++)
            {
                if(actual[i].Coffee.Drink != expected[i].Coffee.Drink ||
                   actual[i].Coffee.Price != expected[i].Coffee.Price ||
                   actual[i].Roast != expected[i].Roast ||
                   actual[i].Extras != expected[i].Extras ||
                   actual[i].Quantity != expected[i].Quantity)
                    Assert.Fail();
            }                                 
        }

        [TestMethod]
        public void BuildCoffeeList()
        {
            string testFile = Environment.CurrentDirectory + "/TestData/validCoffees";
            var expected = _info.CoffeeList;

            var actual = CoffeeShopInfo.BuildCoffeeList(testFile);

            for (int i = 0; i < expected.Count; i++)
            {
                if (actual[i].Drink != expected[i].Drink ||
                    actual[i].Price != expected[i].Price)
                    Assert.Fail();
            }
        }
    }
}
