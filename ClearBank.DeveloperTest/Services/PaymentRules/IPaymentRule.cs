using ClearBank.DeveloperTest.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClearBank.DeveloperTest.Services.PaymentRules
{
   public abstract class PaymentRule
   {
      public abstract bool CanProcess(Account account, MakePaymentRequest? request = null);

      protected bool HasSufficientBalance(Account account, decimal amount)
      {
         return account.Balance >= amount;
      }
   }
}
