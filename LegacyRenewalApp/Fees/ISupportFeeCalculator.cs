namespace LegacyRenewalApp
{
    public interface ISupportFeeCalculator
    {
        SupportFeeCalculationResult Calculate(
            bool includePremiumSupport,
            string normalizedPlanCode);
    }
}