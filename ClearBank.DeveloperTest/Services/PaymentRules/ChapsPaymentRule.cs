using ClearBank.DeveloperTest.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClearBank.DeveloperTest.Services.PaymentRules
{
   public class ChapsPaymentRule : PaymentRule
   {
      public override bool CanProcess(Account account, MakePaymentRequest request)
      {
         if (account is null)
         {
            return false;
         }
         else if (!account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Chaps))
         {
            return false;
         }
         else if (account.Status != AccountStatus.Live)
         {
            return false;
         }

         return true;
      }
   }
}
