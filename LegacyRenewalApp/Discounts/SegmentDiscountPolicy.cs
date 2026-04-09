namespace LegacyRenewalApp
{
    public class SegmentDiscountPolicy : IDiscountPolicy
    {
        public DiscountPolicyResult Apply(DiscountCalculationContext context)
        {
            decimal discountAmount = 0m;
            string notes = string.Empty;

            if (context.Customer.Segment == "Silver")
            {
                discountAmount += context.BaseAmount * 0.05m;
                notes += "silver discount; ";
            }
            else if (context.Customer.Segment == "Gold")
            {
                discountAmount += context.BaseAmount * 0.10m;
                notes += "gold discount; ";
            }
            else if (context.Customer.Segment == "Platinum")
            {
                discountAmount += context.BaseAmount * 0.15m;
                notes += "platinum discount; ";
            }
            else if (context.Customer.Segment == "Education" && context.Plan.IsEducationEligible)
            {
                discountAmount += context.BaseAmount * 0.20m;
                notes += "education discount; ";
            }

            return new DiscountPolicyResult
            {
                DiscountAmount = discountAmount,
                Notes = notes
            };
        }
    }
}