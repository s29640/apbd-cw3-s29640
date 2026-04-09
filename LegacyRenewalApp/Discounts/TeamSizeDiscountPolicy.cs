using System.Collections.Generic;

namespace LegacyRenewalApp.Discounts
{
    public class TeamSizeDiscountPolicy : IDiscountPolicy
    {
        public DiscountPolicyResult Apply(DiscountCalculationContext context)
        {
            decimal discountAmount = 0m;
            var notes = new List<string>();

            if (context.SeatCount >= 50)
            {
                discountAmount += context.BaseAmount * 0.12m;
                notes.Add("large team discount");
            }
            else if (context.SeatCount >= 20)
            {
                discountAmount += context.BaseAmount * 0.08m;
                notes.Add("medium team discount");
            }
            else if (context.SeatCount >= 10)
            {
                discountAmount += context.BaseAmount * 0.04m;
                notes.Add("small team discount");
            }

            return new DiscountPolicyResult
            {
                DiscountAmount = discountAmount,
                Notes = notes
            };
        }
    }
}