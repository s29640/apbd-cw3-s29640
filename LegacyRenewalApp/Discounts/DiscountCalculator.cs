using LegacyRenewalApp;
using System;
using System.Collections.Generic;

namespace LegacyRenewalApp.Discounts
{
    public class DiscountCalculator : IDiscountCalculator
    {
        private readonly IEnumerable<IDiscountPolicy> _policies;

        public DiscountCalculator()
            : this(new IDiscountPolicy[]
            {
                new SegmentDiscountPolicy(),
                new TenureDiscountPolicy(),
                new TeamSizeDiscountPolicy(),
                new LoyaltyPointsDiscountPolicy()
            })
        {
        }

        public DiscountCalculator(IEnumerable<IDiscountPolicy> policies)
        {
            _policies = policies ?? throw new ArgumentNullException(nameof(policies));
        }

        public DiscountCalculationResult Calculate(
            Customer customer,
            SubscriptionPlan plan,
            int seatCount,
            bool useLoyaltyPoints,
            decimal baseAmount)
        {
            var context = new DiscountCalculationContext(
                customer,
                plan,
                seatCount,
                useLoyaltyPoints,
                baseAmount);

            decimal totalDiscount = 0m;
            var notes = new List<string>();

            foreach (var policy in _policies)
            {
                var result = policy.Apply(context);
                totalDiscount += result.DiscountAmount;

                foreach (var note in result.Notes)
                {
                    notes.Add(note);
                }
            }

            return new DiscountCalculationResult
            {
                DiscountAmount = totalDiscount,
                Notes = notes
            };
        }
    }
}