namespace LegacyRenewalApp
{
    public class TenureDiscountPolicy : IDiscountPolicy
    {
        public DiscountPolicyResult Apply(DiscountCalculationContext context)
        {
            decimal discountAmount = 0m;
            string notes = string.Empty;

            if (context.Customer.YearsWithCompany >= 5)
            {
                discountAmount += context.BaseAmount * 0.07m;
                notes += "long-term loyalty discount; ";
            }
            else if (context.Customer.YearsWithCompany >= 2)
            {
                discountAmount += context.BaseAmount * 0.03m;
                notes += "basic loyalty discount; ";
            }

            return new DiscountPolicyResult
            {
                DiscountAmount = discountAmount,
                Notes = notes
            };
        }
    }
}