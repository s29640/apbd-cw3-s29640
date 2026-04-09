using System.Collections.Generic;

namespace LegacyRenewalApp.Discounts
{
    public class LoyaltyPointsDiscountPolicy : IDiscountPolicy
    {
        public DiscountPolicyResult Apply(DiscountCalculationContext context)
        {
            decimal discountAmount = 0m;
            var notes = new List<string>();

            if (context.UseLoyaltyPoints && context.Customer.LoyaltyPoints > 0)
            {
                int pointsToUse = context.Customer.LoyaltyPoints > 200
                    ? 200
                    : context.Customer.LoyaltyPoints;

                discountAmount += pointsToUse;
                notes.Add($"loyalty points used: {pointsToUse}");
            }

            return new DiscountPolicyResult
            {
                DiscountAmount = discountAmount,
                Notes = notes
            };
        }
    }
}