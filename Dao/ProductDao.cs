using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Linq;

namespace Dao
{
    public class ProductDao : IProductDao
    {
        private ISessionFactory sessionFactory;

        public ProductDao()
        {
            var cfg = new Configuration().Configure("Config/hibernate.cfg.xml");
            sessionFactory = cfg.BuildSessionFactory();
        }

        public void Delete(Product entity)
        {
            using (ISession session = sessionFactory.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    session.Delete(entity);
                    session.Flush();
                    transaction.Commit();
                }
            }
        }

        public Product Get(object id)
        {
            using (ISession session = sessionFactory.OpenSession())
            {
                return session.Get<Product>(id);
            }
        }

        public Product Load(object id)
        {
            using (ISession session=sessionFactory.OpenSession())
            {
                return session.Load<Product>(id);
            }
        }

        public IList<Product> LoadAll()
        {
            using (ISession session = sessionFactory.OpenSession())
            {
                return session.QueryOver<Product>().List();

            }
        }

        public object Save(Product entity)
        {
            using (ISession session = sessionFactory.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    var id = session.Save(entity);
                    session.Flush();

                    transaction.Commit();
                    return id;
                }
            }
        }

        public void Update(Product entity)
        {
            using (ISession session = sessionFactory.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    session.Update(entity);
                    session.Flush();
                    transaction.Commit();
                }
            }
        }
    }
}
