using System;

namespace LegacyRenewalApp
{
    public class PaymentFeeCalculator : IPaymentFeeCalculator
    {
        public PaymentFeeCalculationResult Calculate(
            string normalizedPaymentMethod,
            decimal subtotalAfterDiscount,
            decimal supportFee)
        {
            decimal paymentFee = 0m;
            string notes = string.Empty;
            decimal feeBase = subtotalAfterDiscount + supportFee;

            if (normalizedPaymentMethod == "CARD")
            {
                paymentFee = feeBase * 0.02m;
                notes += "card payment fee; ";
            }
            else if (normalizedPaymentMethod == "BANK_TRANSFER")
            {
                paymentFee = feeBase * 0.01m;
                notes += "bank transfer fee; ";
            }
            else if (normalizedPaymentMethod == "PAYPAL")
            {
                paymentFee = feeBase * 0.035m;
                notes += "paypal fee; ";
            }
            else if (normalizedPaymentMethod == "INVOICE")
            {
                paymentFee = 0m;
                notes += "invoice payment; ";
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