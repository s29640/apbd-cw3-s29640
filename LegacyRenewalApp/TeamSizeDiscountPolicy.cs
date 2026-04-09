namespace LegacyRenewalApp
{
    public class TeamSizeDiscountPolicy : IDiscountPolicy
    {
        public DiscountPolicyResult Apply(DiscountCalculationContext context)
        {
            decimal discountAmount = 0m;
            string notes = string.Empty;

            if (context.SeatCount >= 50)
            {
                discountAmount += context.BaseAmount * 0.12m;
                notes += "large team discount; ";
            }
            else if (context.SeatCount >= 20)
            {
                discountAmount += context.BaseAmount * 0.08m;
                notes += "medium team discount; ";
            }
            else if (context.SeatCount >= 10)
            {
                discountAmount += context.BaseAmount * 0.04m;
                notes += "small team discount; ";
            }

            return new DiscountPolicyResult
            {
                DiscountAmount = discountAmount,
                Notes = notes
            };
        }
    }
}