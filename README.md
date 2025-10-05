### PaymentService Refactor
 This project refactors the PaymentService.cs from the original ClearBank codebase. The purpose of the refactor is to improve:  
 - Adherence to SOLID principals
 - Testability  
 - Readability 

The MakePayment method remains unchanged in signature, as required, but the internal design has been improved to make the code more maintainable and easier to extend. 

### Architecture & Design

**1. Payment Rule**
  - Each payment scheme (Bacs, FasterPayments, Chaps) is handled by a separate a separate class that inherits from the abstract PaymentRule class.
  - Each subclass implements the specific validation rules for its scheme.

**2. IDataStoreFactory & IAccountDataStore**
  - Makes PaymentService independent of the actual data store.
  - Allows mocking the data store in unit tests.

**3. PaymentService Refactor**
  - Keeps the MakePayment method simple and readable.
  - All validation rules and balance updates are centralized in a structured and testable way.

**4. Unit Testable Design**
 - Mocks for IDataStoreFactory, IAccountDataStore, and IPaymentRuleFactory allow full isolation.
 - Tests cover success and failure scenarios for all payment schemes.

 **5. What I would add?**
  - Validation Layer – Separate request validation (e.g., negative or zero amounts, missing account number) from payment processing.
  - Helper Methods for Allowed Schemes – Improve readability when checking account permissions using AllowedPaymentSchemes flags.
  - Boundary Conditions – Test edge cases such as zero, exact balance, and very large amounts.
  - Example Usage Snippet – Show how PaymentService can be used in an application.



