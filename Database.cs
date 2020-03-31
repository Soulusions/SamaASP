using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB;
using MongoDB.Driver;

namespace SamaASP
{
    public static class Database
    {
        public static MongoClient dbClient = new MongoClient("mongodb://127.0.0.1:27017");

        public static List<string> DumpDB()
        {
            var dbList = dbClient.ListDatabases().ToList();
            List<string> dbListStr = new List<string>();

            foreach ( var item in dbList )
            {
                dbListStr.Add( item.ToString() );
            }

            return dbListStr;
        }

        public static List<string> DumpCOLL()
        {
            IMongoDatabase db = dbClient.GetDatabase("test");
            var collList = db.ListCollections().ToList();
            List<string> collListStr = new List<string>();
            
            foreach ( var item in collList )
            {
                collListStr.Add( item.ToString() );
            }

            return collListStr;
        }
    }
}
