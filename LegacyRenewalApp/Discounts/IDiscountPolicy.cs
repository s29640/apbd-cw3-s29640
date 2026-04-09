namespace LegacyRenewalApp.Discounts
{
    public interface IDiscountPolicy
    {
        DiscountPolicyResult Apply(DiscountCalculationContext context);
    }
}