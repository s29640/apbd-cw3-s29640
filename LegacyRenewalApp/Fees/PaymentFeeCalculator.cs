using System;
using System.Collections.Generic;

namespace LegacyRenewalApp.Fees
{
    public class PaymentFeeCalculator : IPaymentFeeCalculator
    {
        public PaymentFeeCalculationResult Calculate(
            string normalizedPaymentMethod,
            decimal subtotalAfterDiscount,
            decimal supportFee)
        {
            decimal paymentFee = 0m;
            var notes = new List<string>();
            decimal feeBase = subtotalAfterDiscount + supportFee;

            if (normalizedPaymentMethod == "CARD")
            {
                paymentFee = feeBase * 0.02m;
                notes.Add("card payment fee");
            }
            else if (normalizedPaymentMethod == "BANK_TRANSFER")
            {
                paymentFee = feeBase * 0.01m;
                notes.Add("bank transfer fee");
            }
            else if (normalizedPaymentMethod == "PAYPAL")
            {
                paymentFee = feeBase * 0.035m;
                notes.Add("paypal fee");
            }
            else if (normalizedPaymentMethod == "INVOICE")
            {
                paymentFee = 0m;
                notes.Add("invoice payment");
            }
            else
            {
                throw new ArgumentException("Unsupported payment method");
            }

            return new PaymentFeeCalculationResult
            {
                PaymentFee = paymentFee,
                Notes = notes
            };
        }
    }
}