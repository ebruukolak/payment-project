using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Services;
using ClearBank.DeveloperTest.Services.PaymentRules;
using ClearBank.DeveloperTest.Types;
using Moq;
using System.Security.Principal;
using Xunit;

namespace ClearBank.DeveloperTest.Tests
{
   public class PaymentServiceTests
   {
      private readonly Mock<IDataStoreFactory> _dataStoreFactory;
      private readonly Mock<IAccountDataStore> _accountDataStore;
      private readonly Mock<IPaymentRuleFactory> _paymentRuleFactory;
      private readonly PaymentService _paymentService;

      public PaymentServiceTests()
      {
         _dataStoreFactory = new Mock<IDataStoreFactory>();
         _accountDataStore = new Mock<IAccountDataStore>();
         _paymentRuleFactory = new Mock<IPaymentRuleFactory>();
          
         _dataStoreFactory
            .Setup(f=>f.GetDataStore())
            .Returns(_accountDataStore.Object);

         _paymentService = new PaymentService(_dataStoreFactory.Object, _paymentRuleFactory.Object);
      }

      #region FasterPayments Tests

      [Fact]
      public void MakePayment_FasterPayments_Succeeds_WhenBalanceEnough()
      {
         var account = new Account
         {
            AccountNumber = "FP123",
            Balance = 1000,
            AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments,
            Status = AccountStatus.Live
         };

         var request = new MakePaymentRequest
         {
            DebtorAccountNumber = "FP123",
            PaymentScheme = PaymentScheme.FasterPayments,
            Amount = 500
         };

         _accountDataStore
               .Setup(ds => ds.GetAccount(request.DebtorAccountNumber))
               .Returns(account);

         _paymentRuleFactory
            .Setup(rf => rf.GetPaymentRule(request.PaymentScheme))
            .Returns(new FasterPaymentsRule());

         var result = _paymentService.MakePayment(request);
         _accountDataStore.Verify(ds => ds.UpdateAccount(It.Is<Account>(a => a.Balance == 500)), Times.Once);

         Assert.True(result.Success);
         Assert.Equal(500, account.Balance);
      }

      [Fact]
      public void MakePayment_FasterPayments_Fails_WhenInsufficientFunds()
      {
         var account = new Account
         {
            AccountNumber = "FP124",
            Balance = 100,
            AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments,
            Status = AccountStatus.Live
         };

         var request = new MakePaymentRequest
         {
            DebtorAccountNumber = "FP124",
            PaymentScheme = PaymentScheme.FasterPayments,
            Amount = 200
         };

         _accountDataStore
              .Setup(ds => ds.GetAccount(request.DebtorAccountNumber))
              .Returns(account);

         _paymentRuleFactory
            .Setup(rf => rf.GetPaymentRule(request.PaymentScheme))
            .Returns(new FasterPaymentsRule());

         var result = _paymentService.MakePayment(request);

         _accountDataStore.Verify(ds => ds.UpdateAccount(It.IsAny<Account>()), Times.Never);


         Assert.False(result.Success);
      }

      [Fact]
      public void MakePayment_FasterPayments_Fails_WhenNotAllowed()
      {
         var account = new Account
         {
            AccountNumber = "FP125",
            Balance = 500,
            AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs,
            Status = AccountStatus.Live
         };

         var request = new MakePaymentRequest
         {
            DebtorAccountNumber = "FP125",
            PaymentScheme = PaymentScheme.FasterPayments,
            Amount = 200
         };

         _accountDataStore
              .Setup(ds => ds.GetAccount(request.DebtorAccountNumber))
              .Returns(account);

         _paymentRuleFactory
            .Setup(rf => rf.GetPaymentRule(request.PaymentScheme))
            .Returns(new FasterPaymentsRule());

         var result = _paymentService.MakePayment(request);

         _accountDataStore.Verify(ds => ds.UpdateAccount(It.IsAny<Account>()), Times.Never);


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

         _accountDataStore
             .Setup(ds => ds.GetAccount(request.DebtorAccountNumber))
             .Returns((Account)null);

         _paymentRuleFactory
           .Setup(rf => rf.GetPaymentRule(request.PaymentScheme))
           .Returns(new FasterPaymentsRule());

         var result = _paymentService.MakePayment(request);

         _accountDataStore.Verify(ds => ds.UpdateAccount(It.IsAny<Account>()), Times.Never);


         Assert.False(result.Success);
      }


      #endregion

      #region Bacs Tests

      [Fact]
      public void MakePayment_Bacs_Succeeds_WhenAllowed()
      {
         var account = new Account
         {
            AccountNumber = "BACS123",
            Balance = 500,
            AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs,
            Status = AccountStatus.Live
         };

         var request = new MakePaymentRequest
         {
            DebtorAccountNumber = "BACS123",
            PaymentScheme = PaymentScheme.Bacs,
            Amount = 200
         };

         _accountDataStore
               .Setup(ds => ds.GetAccount(request.DebtorAccountNumber))
               .Returns(account);

         _paymentRuleFactory
            .Setup(rf => rf.GetPaymentRule(request.PaymentScheme))
            .Returns(new BacsPaymentRule());

         var result = _paymentService.MakePayment(request);

         _accountDataStore.Verify(ds => ds.UpdateAccount(It.Is<Account>(a => a.Balance == 300)), Times.Once);


         Assert.True(result.Success);
         Assert.Equal(300, account.Balance);
      }


      [Theory]
      [InlineData(AllowedPaymentSchemes.Chaps)]
      [InlineData(AllowedPaymentSchemes.FasterPayments)]
      public void MakePayment_Bacs_Fails_WhenNotAllowed(AllowedPaymentSchemes allowedSchemes)
      {
         var account = new Account
         {
            AccountNumber = "BACS124",
            Balance = 500,
            AllowedPaymentSchemes = allowedSchemes,
            Status = AccountStatus.Live
         };

         var request = new MakePaymentRequest
         {
            DebtorAccountNumber = "BACS124",
            PaymentScheme = PaymentScheme.Bacs,
            Amount = 200
         };

         _accountDataStore.Setup(ds => ds.GetAccount(request.DebtorAccountNumber)).Returns(account);
         _paymentRuleFactory.Setup(rf => rf.GetPaymentRule(request.PaymentScheme)).Returns(new BacsPaymentRule());

         var result = _paymentService.MakePayment(request);

         _accountDataStore.Verify(ds => ds.UpdateAccount(It.IsAny<Account>()), Times.Never);

         Assert.False(result.Success);
         Assert.Equal(500, account.Balance);
      }

      #endregion

      

      #region Chaps Tests

      [Fact]
      public void MakePayment_Chaps_Succeeds_WhenAllowedAndLive()
      {
         var account= new Account
         {
            AccountNumber = "CHAPS123",
            Balance = 1000,
            AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps,
            Status = AccountStatus.Live
         };

         var request = new MakePaymentRequest
         {
            DebtorAccountNumber = "CHAPS123",
            PaymentScheme = PaymentScheme.Chaps,
            Amount = 300
         };

         _accountDataStore
            .Setup(ds => ds.GetAccount(request.DebtorAccountNumber))
            .Returns(account);

         _paymentRuleFactory
           .Setup(rf => rf.GetPaymentRule(request.PaymentScheme))
           .Returns(new ChapsPaymentRule());

         var result = _paymentService.MakePayment(request);
         _accountDataStore.Verify(ds => ds.UpdateAccount(It.Is<Account>(a => a.AccountNumber == account.AccountNumber)), Times.Once);
         _accountDataStore.Verify(ds => ds.UpdateAccount(It.Is<Account>(a => a.Balance == 700)), Times.Once);


         Assert.True(result.Success);
         Assert.Equal(700, account.Balance);
      }

      [Theory]
      [InlineData(AccountStatus.Disabled)]
      [InlineData(AccountStatus.InboundPaymentsOnly)]
      public void MakePayment_Chaps_Fails_WhenNotLive(AccountStatus status)
      {
         var account = new Account
         {
            AccountNumber = "CH124",
            Balance = 500,
            AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps,
            Status = status
         };

         var request = new MakePaymentRequest
         {
            DebtorAccountNumber = "CH124",
            PaymentScheme = PaymentScheme.Chaps,
            Amount = 200
         };

         _accountDataStore.Setup(ds => ds.GetAccount(request.DebtorAccountNumber)).Returns(account);
         _paymentRuleFactory.Setup(rf => rf.GetPaymentRule(request.PaymentScheme)).Returns(new ChapsPaymentRule());

         var result = _paymentService.MakePayment(request);

         _accountDataStore.Verify(ds => ds.UpdateAccount(It.IsAny<Account>()), Times.Never);

         Assert.False(result.Success);
         Assert.Equal(500, account.Balance);
      }
    
      [Fact]
      public void MakePayment_Chaps_Fails_WhenNotAllowed()
      {
         var account = new Account
         {
            AccountNumber = "CHAPS125",
            Balance = 500,
            AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs,
            Status = AccountStatus.Live
         };

         var request = new MakePaymentRequest
         {
            DebtorAccountNumber = "CHAPS125",
            PaymentScheme = PaymentScheme.Chaps,
            Amount = 200
         };

         _accountDataStore
           .Setup(ds => ds.GetAccount(request.DebtorAccountNumber))
           .Returns(account);

         _paymentRuleFactory
           .Setup(rf => rf.GetPaymentRule(request.PaymentScheme))
           .Returns(new ChapsPaymentRule());

         var result = _paymentService.MakePayment(request);
         _accountDataStore.Verify(ds => ds.UpdateAccount(It.IsAny<Account>()), Times.Never);


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

         _accountDataStore
           .Setup(ds => ds.GetAccount(request.DebtorAccountNumber))
           .Returns((Account)null);

         _paymentRuleFactory
           .Setup(rf => rf.GetPaymentRule(request.PaymentScheme))
           .Returns(new ChapsPaymentRule());

         var result = _paymentService.MakePayment(request);
         _accountDataStore.Verify(ds => ds.UpdateAccount(It.IsAny<Account>()), Times.Never);


         Assert.False(result.Success);
      }

      #endregion
   }
}

