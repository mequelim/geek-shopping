namespace GeekShopping.PaymentProcessor.Interface
{
    /// <summary>
    /// Defines the contract for processing payments within the GeekShopping.PaymentProcessor namespace.
    /// </summary>
    public interface IProcessorPayment
    {
        /// <summary>
        /// Processes a payment and returns a value indicating whether the payment was successful.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the payment was processed successfully; otherwise, <c>false</c>.
        /// </returns>
        bool PaymentProcessor();
    }
}