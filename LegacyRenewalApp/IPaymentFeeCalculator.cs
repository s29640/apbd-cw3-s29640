namespace LegacyRenewalApp
{
    public interface IPaymentFeeCalculator
    {
        PaymentFeeCalculationResult Calculate(
            string normalizedPaymentMethod,
            decimal subtotalAfterDiscount,
            decimal supportFee);
    }
}