using System.Collections.Generic;

namespace LegacyRenewalApp.Fees
{
    public class SupportFeeCalculationResult
    {
        public decimal SupportFee { get; set; }
        public IReadOnlyCollection<string> Notes { get; set; } = new List<string>();
    }
}