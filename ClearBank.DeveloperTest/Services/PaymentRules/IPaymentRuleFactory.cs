using ClearBank.DeveloperTest.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClearBank.DeveloperTest.Services.PaymentRules
{
   public interface IPaymentRuleFactory
   {
      PaymentRule GetPaymentRule(PaymentScheme paymentScheme);
   }
}
