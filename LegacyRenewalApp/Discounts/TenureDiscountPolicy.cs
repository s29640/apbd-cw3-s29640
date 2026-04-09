using System.Collections.Generic;

namespace LegacyRenewalApp.Discounts
{
    public class TenureDiscountPolicy : IDiscountPolicy
    {
        public DiscountPolicyResult Apply(DiscountCalculationContext context)
        {
            decimal discountAmount = 0m;
            var notes = new List<string>();

            if (context.Customer.YearsWithCompany >= 5)
            {
                discountAmount += context.BaseAmount * 0.07m;
                notes.Add("long-term loyalty discount");
            }
            else if (context.Customer.YearsWithCompany >= 2)
            {
                discountAmount += context.BaseAmount * 0.03m;
                notes.Add("basic loyalty discount");
            }

            return new DiscountPolicyResult
            {
                DiscountAmount = discountAmount,
                Notes = notes
            };
        }
    }
}