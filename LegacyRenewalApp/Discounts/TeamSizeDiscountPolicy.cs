using System.Collections.Generic;
using System.Linq;

namespace LegacyRenewalApp.Discounts
{
    public class TeamSizeDiscountPolicy : IDiscountPolicy
    {
        private static readonly (int MinSeats, decimal Rate, string Note)[] Rules =
        {
            (50, 0.12m, "large team discount"),
            (20, 0.08m, "medium team discount"),
            (10, 0.04m, "small team discount")
        };

        public DiscountPolicyResult Apply(DiscountCalculationContext context)
        {
            var rule = Rules.FirstOrDefault(r => context.SeatCount >= r.MinSeats);

            var discountAmount = rule == default
                ? 0m
                : context.BaseAmount * rule.Rate;

            var notes = rule == default
                ? new List<string>()
                : new List<string> { rule.Note };

            return new DiscountPolicyResult
            {
                DiscountAmount = discountAmount,
                Notes = notes
            };
        }
    }
}