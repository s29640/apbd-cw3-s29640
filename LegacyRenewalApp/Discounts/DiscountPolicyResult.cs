using System.Collections.Generic;

namespace LegacyRenewalApp.Discounts
{
    public class DiscountPolicyResult
    {
        public decimal DiscountAmount { get; set; }
        public IReadOnlyCollection<string> Notes { get; set; } = new List<string>();
    }
}