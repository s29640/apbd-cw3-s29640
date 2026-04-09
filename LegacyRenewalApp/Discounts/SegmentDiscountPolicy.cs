using System.Collections.Generic;

namespace LegacyRenewalApp.Discounts
{
    public class SegmentDiscountPolicy : IDiscountPolicy
    {
        public DiscountPolicyResult Apply(DiscountCalculationContext context)
        {
            decimal discountAmount = 0m;
            var notes = new List<string>();

            if (context.Customer.Segment == "Silver")
            {
                discountAmount += context.BaseAmount * 0.05m;
                notes.Add("silver discount");
            }
            else if (context.Customer.Segment == "Gold")
            {
                discountAmount += context.BaseAmount * 0.10m;
                notes.Add("gold discount");
            }
            else if (context.Customer.Segment == "Platinum")
            {
                discountAmount += context.BaseAmount * 0.15m;
                notes.Add("platinum discount");
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