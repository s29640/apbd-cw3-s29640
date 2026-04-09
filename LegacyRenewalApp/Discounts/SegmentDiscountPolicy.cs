using System.Collections.Generic;

namespace LegacyRenewalApp.Discounts
{
    public class SegmentDiscountPolicy : IDiscountPolicy
    {
        private static readonly Dictionary<string, (decimal Rate, string Note)> _segmentDiscounts
            = new Dictionary<string, (decimal, string)>
        {
            { "Silver", (0.05m, "silver discount") },
            { "Gold", (0.10m, "gold discount") },
            { "Platinum", (0.15m, "platinum discount") }
        };

        public DiscountPolicyResult Apply(DiscountCalculationContext context)
        {
            decimal discountAmount = 0m;
            var notes = new List<string>();

            if (_segmentDiscounts.TryGetValue(context.Customer.Segment, out var config))
            {
                discountAmount += context.BaseAmount * config.Rate;
                notes.Add(config.Note);
            }
            else if (context.Customer.Segment == "Education" && context.Plan.IsEducationEligible)
            {
                discountAmount += context.BaseAmount * 0.20m;
                notes.Add("education discount");
            }

            return new DiscountPolicyResult
            {
                DiscountAmount = discountAmount,
                Notes = notes
            };
        }
    }
}