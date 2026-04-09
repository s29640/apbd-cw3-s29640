namespace LegacyRenewalApp
{
    public class LoyaltyPointsDiscountPolicy : IDiscountPolicy
    {
        public DiscountPolicyResult Apply(DiscountCalculationContext context)
        {
            decimal discountAmount = 0m;
            string notes = string.Empty;

            if (context.UseLoyaltyPoints && context.Customer.LoyaltyPoints > 0)
            {
                int pointsToUse = context.Customer.LoyaltyPoints > 200
                    ? 200
                    : context.Customer.LoyaltyPoints;

                discountAmount += pointsToUse;
                notes += $"loyalty points used: {pointsToUse}; ";
            }

            return new DiscountPolicyResult
            {
                DiscountAmount = discountAmount,
                Notes = notes
            };
        }
    }
}