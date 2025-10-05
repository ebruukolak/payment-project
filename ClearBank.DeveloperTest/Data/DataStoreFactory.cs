using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClearBank.DeveloperTest.Data
{
   public class DataStoreFactory : IDataStoreFactory
   {
      private readonly string _dataStoreType;
      public DataStoreFactory()
      {
         _dataStoreType = ConfigurationManager.AppSettings["DataStoreType"];
      }

      public DataStoreFactory(string dataStoreType)
      {
         _dataStoreType = dataStoreType;
      }

      public IAccountDataStore GetDataStore()
      {
         if (_dataStoreType == "Backup")
         {
            return new BackupAccountDataStore();
         }

         return new AccountDataStore();
      }
   }
}
