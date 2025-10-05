using ClearBank.DeveloperTest.Types;
using System.Collections.Generic;

namespace ClearBank.DeveloperTest.Data
{
    public class AccountDataStore
    {
      private static readonly Dictionary<string, Account> _accounts = new();

      public Account GetAccount(string accountNumber)
      {
         _accounts.TryGetValue(accountNumber, out var account);
         return account;
      }

      public void UpdateAccount(Account account)
      {
         _accounts[account.AccountNumber] = account;
      }

      public static void Seed(Account account)
      {
         _accounts[account.AccountNumber] = account;
      }

      public static void Clear()
      {
         _accounts.Clear();
      }

      public static bool Exists(string accountNumber)
      {
         return _accounts.ContainsKey(accountNumber);
      }
   }
}
