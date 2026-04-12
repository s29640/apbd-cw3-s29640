using System;
using System.Collections.Generic;

namespace LegacyRenewalApp.Fees
{
    public class PaymentFeeCalculator : IPaymentFeeCalculator
    {
        private static readonly Dictionary<string, (decimal rate, string note)> _config =
            new()
            {
                ["CARD"] = (0.02m, "card payment fee"),
                ["BANK_TRANSFER"] = (0.01m, "bank transfer fee"),
                ["PAYPAL"] = (0.035m, "paypal fee"),
                ["INVOICE"] = (0m, "invoice payment")
            };

        public PaymentFeeCalculationResult Calculate(
            string normalizedPaymentMethod,
            decimal subtotalAfterDiscount,
            decimal supportFee)
        {
            if (!_config.TryGetValue(normalizedPaymentMethod, out var config))
                throw new ArgumentException("Unsupported payment method");

            decimal feeBase = subtotalAfterDiscount + supportFee;
            decimal paymentFee = feeBase * config.rate;

            return new PaymentFeeCalculationResult
            {
                PaymentFee = paymentFee,
                Notes = new List<string> { config.note }
            };
        }
    }
}