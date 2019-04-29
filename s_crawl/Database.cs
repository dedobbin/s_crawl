using NHibernate;
using NHibernate.Tool.hbm2ddl;
using System.Reflection;
namespace _crawl0
{
    public sealed class Database
    {
        private static Database database = null;
        private static readonly object padlock = new object();
        private ISessionFactory sessionFactory;

        public Database()
        {
            var configuration = new NHibernate.Cfg.Configuration();
            configuration.Configure();
            configuration.AddAssembly(Assembly.GetExecutingAssembly());
            sessionFactory = configuration.BuildSessionFactory();
        }

        public static ISession OpenSession
        {
            get
            {
                lock (padlock)
                {
                    if (database == null)
                    {
                        database = new Database();
                    }
                    return database.sessionFactory.OpenSession() ;
                }
            }
        }
    }
}
