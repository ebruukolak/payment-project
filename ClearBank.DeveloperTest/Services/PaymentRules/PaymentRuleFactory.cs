using ClearBank.DeveloperTest.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClearBank.DeveloperTest.Services.PaymentRules
{
   public class PaymentRuleFactory: IPaymentRuleFactory
   {
      private readonly Dictionary<PaymentScheme, PaymentRule> _rules;

      public PaymentRuleFactory()
      {
         _rules = new Dictionary<PaymentScheme, PaymentRule>
         {
            {PaymentScheme.FasterPayments, new FasterPaymentsRule() },
            {PaymentScheme.Bacs,new BacsPaymentRule() },
            {PaymentScheme.Chaps,new ChapsPaymentRule() },
         };
      }

      public PaymentRule GetPaymentRule(PaymentScheme paymentScheme)
      {
         if (!_rules.TryGetValue(paymentScheme, out PaymentRule paymentRule))
            throw new NotSupportedException($"Payment scheme {paymentScheme} is not supported");
         return paymentRule;
      }
   }
}
