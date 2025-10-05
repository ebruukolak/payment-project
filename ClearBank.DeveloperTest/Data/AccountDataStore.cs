using ClearBank.DeveloperTest.Types;
using System.Collections.Generic;

namespace ClearBank.DeveloperTest.Data
{
    public class AccountDataStore: IAccountDataStore
   {

      public Account GetAccount(string accountNumber)
      {
         return new Account();
      }

      public void UpdateAccount(Account account)
      {
      }


   }
}
