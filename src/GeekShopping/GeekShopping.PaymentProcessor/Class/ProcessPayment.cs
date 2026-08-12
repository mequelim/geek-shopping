using GeekShopping.PaymentProcessor.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace GeekShopping.PaymentProcessor.Class
{
    /// <summary>
    /// Represents the implementation of the <see cref="GeekShopping.PaymentProcessor.Interface.IProcessorPayment"/> interface for processing payments within the GeekShopping.PaymentProcessor namespace.
    /// </summary>
    public class ProcessPayment :IProcessorPayment
    {
        /// <summary>
        /// Processes a payment and returns a value indicating whether the payment was successful.
        /// </summary>
        /// <returns><c>true</c> if the payment was processed successfully; otherwise, <c>false</c>.</returns>
        public bool PaymentProcessor() => true;
    }
}