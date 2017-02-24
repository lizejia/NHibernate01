using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Cfg;
using NUnit.Framework;
using Domain;

namespace NHibernateLiftCycle_PersistentClassTest
{
    //什么是持久化类
    //持久化就是把数据（内存中的对象）保存到可永久保存的存储设备中（磁盘），持久化类就是持久化数据的载体，
    //在应用程序中，实现业务的实体类就是持久化类，如我们用到Product类

    //持久化类的规则
    //•实现一个默认的（即无参数的）构造方法（constructor）
    //•提供一个标识属性（identifier property）（可选）
    //•使用非密封（non-sealed）类以及虚方法（virtual methods） (可选)

    [TestFixture]
    public class PersistentClassTest
    {
        private ISessionFactory sessionFactory;

        private const int id = 8888;

        [SetUp]
        public void Init()
        {
            var cfg = new Configuration().Configure("Config/hibernate.cfg.xml");
            sessionFactory = cfg.BuildSessionFactory();
        }

        [Test]
        public void Save()
        {
            using (ISession session = this.sessionFactory.OpenSession())
            {
                using (ITransaction tran = session.BeginTransaction())
                {
                    try
                    {
                        var product = new Product
                        {
                            ID = id,
                            BuyPrice = 100M,
                            Code = "ABC123",
                            Name = "电脑",
                            QuantityPerUnit = "20x1",
                            SellPrice = 110M,
                            Unit = "台"
                        };

                        session.Save(product);

                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        throw ex;
                    }
                }
            }
        }

        [Test]
        public void Update()
        {
            using (ISession session = sessionFactory.OpenSession())
            {
                using (ITransaction tran = session.BeginTransaction())
                {
                    try
                    {
                        var product = session.Get<Product>(id);
                        product.Name = "电脑Update";
                        session.Update(product);
                        tran.Commit();

                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        throw ex;
                    }
                }
            }
        }

        [Test]
        public void Load()
        {
            using (ISession session = sessionFactory.OpenSession())
            {
                using (ITransaction tran = session.BeginTransaction())
                {
                    try
                    {
                        string name = "";
                        var product = session.Load<Product>(id);
                        Console.WriteLine(name);
                        name = product.Name;
                        Console.WriteLine(name);
                        tran.Commit();

                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        throw ex;
                    }
                }
            }
        }

        [Test]
        public void Delete()
        {
            using (ISession session = sessionFactory.OpenSession())
            {
                using (ITransaction tran = session.BeginTransaction())
                {
                    try
                    {
                        var product = session.Load<Product>(id);
                        session.Delete(product);
                        tran.Commit();

                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        throw ex;
                    }
                }
            }
        }
    }
}
