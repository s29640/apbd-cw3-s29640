namespace LegacyRenewalApp.Fees
{
    public interface ISupportFeeCalculator
    {
        SupportFeeCalculationResult Calculate(
            bool includePremiumSupport,
            string normalizedPlanCode);
    }
}