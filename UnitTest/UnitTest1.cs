using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Domain;
using Dao;
namespace UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            IProductDao productDao = new ProductDao();
            var product = new Product
            {
                ID = 1,
                BuyPrice = 10M,
                Code = "ABC123",
                Name = "电脑" + 1,
                QuantityPerUnit = "20x1",
                SellPrice = 11M,
                Unit = "台" + 1
            };
            var obj = productDao.Save(product);
        }
    }
}
