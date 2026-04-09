namespace LegacyRenewalApp.Fees
{
    public class SupportFeeCalculator : ISupportFeeCalculator
    {
        public SupportFeeCalculationResult Calculate(
            bool includePremiumSupport,
            string normalizedPlanCode)
        {
            decimal supportFee = 0m;
            string notes = string.Empty;

            if (includePremiumSupport)
            {
                if (normalizedPlanCode == "START")
                {
                    supportFee = 250m;
                }
                else if (normalizedPlanCode == "PRO")
                {
                    supportFee = 400m;
                }
                else if (normalizedPlanCode == "ENTERPRISE")
                {
                    supportFee = 700m;
                }

                notes += "premium support included; ";
            }

            return new SupportFeeCalculationResult
            {
                SupportFee = supportFee,
                Notes = notes
            };
        }
    }
}