using System.Collections.Generic;

namespace LegacyRenewalApp.Fees
{
    public class SupportFeeCalculator : ISupportFeeCalculator
    {
        private static readonly IReadOnlyDictionary<string, decimal> _supportFees =
            new Dictionary<string, decimal>
            {
                ["START"] = 250m,
                ["PRO"] = 400m,
                ["ENTERPRISE"] = 700m
            };

        public SupportFeeCalculationResult Calculate(
            bool includePremiumSupport,
            string normalizedPlanCode)
        {
            if (!includePremiumSupport)
            {
                return new SupportFeeCalculationResult
                {
                    SupportFee = 0m,
                    Notes = new List<string>()
                };
            }

            var supportFee = _supportFees.GetValueOrDefault(normalizedPlanCode, 0m);

            return new SupportFeeCalculationResult
            {
                SupportFee = supportFee,
                Notes = new List<string> { "premium support included" }
            };
        }
    }
}