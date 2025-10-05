using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClearBank.DeveloperTest.Data
{
   public interface IDataStoreFactory
   {
      IAccountDataStore GetDataStore();

   }
}
