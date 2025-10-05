using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Services.PaymentRules;
using ClearBank.DeveloperTest.Types;
using System.Configuration;

namespace ClearBank.DeveloperTest.Services
{
   public class PaymentService : IPaymentService
   {
      private readonly IDataStoreFactory _dataStoreFactory;
      private readonly IPaymentRuleFactory _paymentRuleFactory;
      public PaymentService(IDataStoreFactory dataStoreFactory, IPaymentRuleFactory paymentRuleFactory)
      {
         _dataStoreFactory = dataStoreFactory;
         _paymentRuleFactory = paymentRuleFactory;
      }
      public MakePaymentResult MakePayment(MakePaymentRequest request)
      {
         var dataStore = _dataStoreFactory.GetDataStore();
         Account account = dataStore.GetAccount(request.DebtorAccountNumber);

         var rule = _paymentRuleFactory.GetPaymentRule(request.PaymentScheme);

         var isProcessSucced = rule.CanProcess(account, request);

         if (isProcessSucced)
         {
            account.Balance -= request.Amount;
            dataStore.UpdateAccount(account);
         }

         return new MakePaymentResult { Success = isProcessSucced };
      }
   }
}
