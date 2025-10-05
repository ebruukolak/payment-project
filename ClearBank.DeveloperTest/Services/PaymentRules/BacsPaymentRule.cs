using ClearBank.DeveloperTest.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClearBank.DeveloperTest.Services.PaymentRules
{
   public class BacsPaymentRule : PaymentRule
   {
      public override bool CanProcess(Account account, MakePaymentRequest request)
      {
         if (account is null)
         {
            return false;
         }

         if (!account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Bacs))
         {
            return false;
         }

         return true;
      }
   }
}
