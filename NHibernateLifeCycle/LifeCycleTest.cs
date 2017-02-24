using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NHibernate;
using NHibernate.Cfg;
using Domain;


namespace NHibernateLifeCycle
{
    [TestFixture]
    public class LifeCycleTest
    {

        //临时态（Transient）：用new创建的对象，它没有持久化，没有纳入Session中，随时可以被垃圾回收，处于此状态的对象叫临时对象。特点：数据库中没有与之对应的记录；
        //持久态（Persistent）：已经持久化，加入到了Session缓存中。通过NHibernate保存的对象或通过Get/Load等方法获取出的对象，其对象没有脱离Session的管理，处于此状态的对象叫持久对象；
        //游离态（Detached）：持久化对象脱离了Session的对象。如Session缓存被清空的对象。特点：已经持久化，但不在Session缓存中。处于此状态的对象叫游离对象；

        private ISessionFactory sessionFactory;

        public LifeCycleTest()
        {
            log4net.Config.XmlConfigurator.Configure();
        }

        [SetUp]
        public void Init()
        {
            var cfg = new Configuration().Configure("Config/hibernate.cfg.xml");
            sessionFactory = cfg.BuildSessionFactory();
        }
        
        /// <summary>
        /// Transient（临时）到Persistent（持久）
        /// </summary>
        [Test]
        public void TransientToPersistent()
        {
            
            using (ISession session = sessionFactory.OpenSession())
            {
                using (ITransaction transaction =session.BeginTransaction())
                {
                    Product product = new Product()
                    {
                        ID = 10000,
                        BuyPrice = 10M,
                        Code = "ABC123",
                        Name = "电脑",
                        QuantityPerUnit = "20x1",
                        SellPrice = 11M,
                        Unit = "台"
                    };

                    try
                    {
                        //Persistent
                        session.Save(product);
                        //保存记录后再修改记录
                        product.Code = "1000MMMM";
                        //提交事务
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        //回滚事务
                        transaction.Rollback();

                        throw ex;
                    }
                }
            }
        }

        /// <summary>
        /// Persistent持久到Detached游离，再到持久
        /// ISession.QueryOver<T>只有List()之后才生成查询Sql，这个是延迟加载，避免不必要的性能开销。
        /// 延迟加载的有效期在Session打开的情况在。
        /// </summary>
        [Test]
        public void PersistentToDetachedToPer()
        {
            //Detached
            Product product = new Product();
            using (ISession session =sessionFactory.OpenSession())
            {
                using (ITransaction tran = session.BeginTransaction())
                {
                    try
                    {
                        //Presistent

                        IQueryOver<Product> cc = session.QueryOver<Product>();
                        Console.WriteLine("开始调用List()");
                        IList<Product> Ipro = cc.List();
                        Console.WriteLine("开始调用FirstOrDefault()");
                        product = Ipro.FirstOrDefault();
                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        throw ex;
                    }
                    
                }
            }
            //Detached
            product.Remark = "Persistent持久到Detached游离，再到持久222";

            using (ISession session = sessionFactory.OpenSession())
            {
                using (ITransaction tran = session.BeginTransaction())
                {
                    try
                    {
                        //Persistent
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

        /// <summary>
        /// Get方法得到持久态(Persistent)
        /// 1、Get方法不管是否提交事务，都可以得到持久态对象
        /// 2、在修改对象属性之前提交事务，不能提交到数据库
        /// 3、在修改对象属性之后提交事务，可以把修改的属性提交到数据库
        /// </summary>
        [Test]
        public void Get()
        {
            using (ISession session = sessionFactory.OpenSession())
            {
                using (ITransaction tran = session.BeginTransaction())
                {
                    try
                    {
                        //Presistent
                        Console.WriteLine("开始调用ISession.Get()");
                        Product product = session.Get<Product>(10000);
                        Console.WriteLine("修改Name为Get");
                        //tran.Commit();
                        product.Name = "Get";
                       
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        throw ex;
                    }

                }
            }
        }



        /// <summary>
        /// Load()
        /// Load与ISession.QueryOver<T>都有延迟加载的特性，Load是在查看其属性的时候才回去生成SELECT,ISession.QueryOver<T>是调用其List()
        /// 当调用Load()方法时，不立刻产生SQL语句，查看其属性后才产生SQL语句，并且查看数据库中不存在对象的属性时会抛出异常。
        /// 原因是调用Load()方法会返回一个“代理类”，这是NHibernate的一个重要的特性——延迟加载
        /// NHibernate的代理对象是由第三方组件“Antlr3.Runtime”提供的
        /// </summary>
        [Test]
        public void Load()
        {
            using (ISession session = sessionFactory.OpenSession())
            {
                using (ITransaction tran = session.BeginTransaction())
                {
                    try
                    {
                        Console.WriteLine("调用 Load()方法");
                        //Persistent
                        var product = session.Load<Product>(10000);
                        
                        //断言为空
                        Assert.NotNull(product);

                        Console.WriteLine("查看其Name属性");
                        //当查看其属性时，则会生成SQL语句
                        string name = product.Name;
                        Assert.NotNull(name);  //断言name不为空

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


        /// <summary>
        /// Delete
        /// 先得到一个持久态（Persistent）对象，然后调用Delete()方法删除该对象，这时该对象变回临时态（Transient）
        /// </summary>
        [Test]
        public void DeleteTest()
        {
            using (ISession session = this.sessionFactory.OpenSession())
            {
                using (ITransaction tran = session.BeginTransaction())
                {
                    //Transient
                    var product = new Product
                    {
                        ID = 1,
                        BuyPrice = 10M,
                        Code = "ABC123",
                        Name = "电脑",
                        QuantityPerUnit = "20x1",
                        SellPrice = 11M,
                        Unit = "台"

                    };

                    try
                    {
                        //Persistent
                        session.Save(product);

                        //Transient
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

        /// <summary>
        /// Update
        /// 先手动打造new一个数据库中存在的游离态（Detached）对象，然后直接调用Update()方法将对象的状态设置为持久态（Persistent）
        /// 为什么可以new出一个游离态对象，因为游离态的特点是，已经持久化，但不在Session缓存中。处于此状态的对象叫游离对象
        /// 也就说看new出来的对象，其数据是否已经存在于数据库中，但是这个对象又还没有被Session管理
        /// </summary>
        [Test]
        public void UpdateTest()
        {
            int id = 9999;

            using (ISession session = this.sessionFactory.OpenSession())
            {
                using (ITransaction tran = session.BeginTransaction())
                {
                    //Transient
                    var product = new Product
                    {
                        ID = id,
                        BuyPrice = 10M,
                        Code = "ABC123",
                        Name = "电脑",
                        QuantityPerUnit = "20x1",
                        SellPrice = 11M,
                        Unit = "台"

                    };

                    try
                    {
                        //Persistent
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

            using (ISession session = this.sessionFactory.OpenSession())
            {
                using (ITransaction tran = session.BeginTransaction())
                {
                    //Detached
                    var product = new Product
                    {
                        ID = id,
                        Code = "ABC456",
                    };

                    try
                    {
                        //Persistent
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
    }
}
