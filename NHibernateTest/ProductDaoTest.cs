using Dao;
using Domain;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHibernateTest
{
    [TestFixture]
    public class ProductDaoTest
    {
        private IProductDao productDao;

        [SetUp]
        public void Init()
        {
            productDao = new ProductDao();
        }

        [Test]
        public void SaveTest()
        {
            IList<Product> list = productDao.LoadAll();
            for (int i = 0; i < 10; i++)
            {
                var product = new Product
                {
                    ID = list[list.Count - 1].ID + i + 1,
                    BuyPrice = 10M,
                    Code = "ABC123" + i,
                    Name = "电脑" + i,
                    QuantityPerUnit = "20x1",
                    SellPrice = 11M,
                    Unit = "台" + i
                };
                var obj = productDao.Save(product);
                Assert.NotNull(obj);
            }
        }

        [Test]
        public void UpdateTest()
        {
            var product = this.productDao.LoadAll().FirstOrDefault();
            Assert.NotNull(product);

            product.SellPrice = 12M;

            Assert.AreEqual(12M, product.SellPrice);
        }

        [Test]
        public void DeleteTest()
        {
            var product = this.productDao.LoadAll().FirstOrDefault();
            Assert.NotNull(product);
            var id = product.ID;
            productDao.Delete(product);

            Assert.Null(productDao.Get(id));
        }

        [Test]
        public void GetTest()
        {
            var product = this.productDao.LoadAll().FirstOrDefault();
            Assert.NotNull(product);

            var id = product.ID;
            Assert.NotNull(this.productDao.Get(id));
        }

        [Test]
        public void LoadTest()
        {
            var product = this.productDao.LoadAll().FirstOrDefault();
            Assert.NotNull(product);

            var id = product.ID;
            Assert.NotNull(this.productDao.Get(id));
        }
        [Test]
        public void LoadAllTest()
        {
            var count = this.productDao.LoadAll().Count;
            Assert.True(count > 0);
        }
    }
}
