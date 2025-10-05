using ClearBank.DeveloperTest.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClearBank.DeveloperTest.Services.PaymentRules
{
   public class FasterPaymentsRule : PaymentRule
   {
      public override bool CanProcess(Account account, MakePaymentRequest request)
      {
         if (account is null|| request is null)
         {
            return false;
         }
         else if (!account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.FasterPayments))
         {
            return false;
         }

         return HasSufficientBalance(account, request.Amount);


      }
   }
}
