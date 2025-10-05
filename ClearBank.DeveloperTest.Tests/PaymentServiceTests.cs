using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Services;
using ClearBank.DeveloperTest.Types;
using Xunit;

namespace ClearBank.DeveloperTest.Tests
{
   public class PaymentServiceTests
   {
      private IPaymentService _paymentService;

      public PaymentServiceTests()
      {
         _paymentService = new PaymentService();
         AccountDataStore.Clear();
      }


      #region Bacs Tests

      [Fact]
      public void MakePayment_Bacs_Succeeds_WhenAllowed()
      {
         AccountDataStore.Seed(new Account
         {
            AccountNumber = "BACS123",
            Balance = 500,
            AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs,
            Status = AccountStatus.Live
         });

         var request = new MakePaymentRequest
         {
            DebtorAccountNumber = "BACS123",
            PaymentScheme = PaymentScheme.Bacs,
            Amount = 200
         };

         var result = _paymentService.MakePayment(request);

         Assert.True(result.Success);
         var updated = new AccountDataStore().GetAccount("BACS123");
         Assert.Equal(300, updated.Balance);
      }

      [Fact]
      public void MakePayment_Bacs_Fails_WhenNotAllowed()
      {
         AccountDataStore.Seed(new Account
         {
            AccountNumber = "BACS124",
            Balance = 500,
            AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps,
            Status = AccountStatus.Live
         });

         var request = new MakePaymentRequest
         {
            DebtorAccountNumber = "BACS124",
            PaymentScheme = PaymentScheme.Bacs,
            Amount = 200
         };

         var result = _paymentService.MakePayment(request);

         Assert.False(result.Success);
      }

      #endregion

      #region FasterPayments Tests

      [Fact]
      public void MakePayment_FasterPayments_Succeeds_WhenBalanceEnough()
      {
         AccountDataStore.Seed(new Account
         {
            AccountNumber = "FP123",
            Balance = 1000,
            AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments,
            Status = AccountStatus.Live
         });

         var request = new MakePaymentRequest
         {
            DebtorAccountNumber = "FP123",
            PaymentScheme = PaymentScheme.FasterPayments,
            Amount = 500
         };

         var result = _paymentService.MakePayment(request);

         Assert.True(result.Success);
         var updated = new AccountDataStore().GetAccount("FP123");
         Assert.Equal(500, updated.Balance);
      }

      [Fact]
      public void MakePayment_FasterPayments_Fails_WhenInsufficientFunds()
      {
         AccountDataStore.Seed(new Account
         {
            AccountNumber = "FP124",
            Balance = 100,
            AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments,
            Status = AccountStatus.Live
         });

         var request = new MakePaymentRequest
         {
            DebtorAccountNumber = "FP124",
            PaymentScheme = PaymentScheme.FasterPayments,
            Amount = 200
         };

         var result = _paymentService.MakePayment(request);

         Assert.False(result.Success);
      }

      [Fact]
      public void MakePayment_FasterPayments_Fails_WhenNotAllowed()
      {
         AccountDataStore.Seed(new Account
         {
            AccountNumber = "FP125",
            Balance = 500,
            AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs,
            Status = AccountStatus.Live
         });

         var request = new MakePaymentRequest
         {
            DebtorAccountNumber = "FP125",
            PaymentScheme = PaymentScheme.FasterPayments,
            Amount = 200
         };

         var result = _paymentService.MakePayment(request);

         Assert.False(result.Success);
      }

      [Fact]
      public void MakePayment_FasterPayments_Fails_WhenAccountNotFound()
      {
         var request = new MakePaymentRequest
         {
            DebtorAccountNumber = "FP999",
            PaymentScheme = PaymentScheme.FasterPayments,
            Amount = 100
         };

         var result = _paymentService.MakePayment(request);

         Assert.False(result.Success);
      }


      #endregion

      #region Chaps Tests

      [Fact]
      public void MakePayment_Chaps_Succeeds_WhenAllowedAndLive()
      {
         AccountDataStore.Seed(new Account
         {
            AccountNumber = "CHAPS123",
            Balance = 1000,
            AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps,
            Status = AccountStatus.Live
         });

         var request = new MakePaymentRequest
         {
            DebtorAccountNumber = "CHAPS123",
            PaymentScheme = PaymentScheme.Chaps,
            Amount = 300
         };

         var result = _paymentService.MakePayment(request);

         Assert.True(result.Success);
         var updated = new AccountDataStore().GetAccount("CHAPS123");
         Assert.Equal(700, updated.Balance);
      }

      [Fact]
      public void MakePayment_Chaps_Fails_WhenNotLive()
      {
         AccountDataStore.Seed(new Account
         {
            AccountNumber = "CHAPS124",
            Balance = 500,
            AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps,
            Status = AccountStatus.Disabled
         });

         var request = new MakePaymentRequest
         {
            DebtorAccountNumber = "CHAPS124",
            PaymentScheme = PaymentScheme.Chaps,
            Amount = 200
         };

         var result = _paymentService.MakePayment(request);

         Assert.False(result.Success);
      }

      [Fact]
      public void MakePayment_Chaps_Fails_WhenNotAllowed()
      {
         AccountDataStore.Seed(new Account
         {
            AccountNumber = "CHAPS125",
            Balance = 500,
            AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs,
            Status = AccountStatus.Live
         });

         var request = new MakePaymentRequest
         {
            DebtorAccountNumber = "CHAPS125",
            PaymentScheme = PaymentScheme.Chaps,
            Amount = 200
         };

         var result = _paymentService.MakePayment(request);

         Assert.False(result.Success);
      }

      [Fact]
      public void MakePayment_Chaps_Fails_WhenAccountNotFound()
      {
         var request = new MakePaymentRequest
         {
            DebtorAccountNumber = "CHAPS999",
            PaymentScheme = PaymentScheme.Chaps,
            Amount = 100
         };

         var result = _paymentService.MakePayment(request);

         Assert.False(result.Success);
      }

      #endregion
   }
}

