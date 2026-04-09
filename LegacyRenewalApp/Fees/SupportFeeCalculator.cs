using System.Collections.Generic;

namespace LegacyRenewalApp.Fees
{
    public class SupportFeeCalculator : ISupportFeeCalculator
    {
        public SupportFeeCalculationResult Calculate(
            bool includePremiumSupport,
            string normalizedPlanCode)
        {
            decimal supportFee = 0m;
            var notes = new List<string>();

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

                notes.Add("premium support included");
            }

            return new SupportFeeCalculationResult
            {
                SupportFee = supportFee,
                Notes = notes
            };
        }
    }
}