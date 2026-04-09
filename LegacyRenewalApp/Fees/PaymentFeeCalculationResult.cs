using System.Collections.Generic;

namespace LegacyRenewalApp.Fees
{
    public class PaymentFeeCalculationResult
    {
        public decimal PaymentFee { get; set; }
        public IReadOnlyCollection<string> Notes { get; set; } = new List<string>();
    }
}