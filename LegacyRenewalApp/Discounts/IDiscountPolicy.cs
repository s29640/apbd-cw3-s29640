namespace LegacyRenewalApp
{
    public interface IDiscountPolicy
    {
        DiscountPolicyResult Apply(DiscountCalculationContext context);
    }
}