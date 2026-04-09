namespace LegacyRenewalApp.Fees
{
    public interface IPaymentFeeCalculator
    {
        PaymentFeeCalculationResult Calculate(
            string normalizedPaymentMethod,
            decimal subtotalAfterDiscount,
            decimal supportFee);
    }
}