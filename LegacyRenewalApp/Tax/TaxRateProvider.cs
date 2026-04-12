using System.Collections.Generic;

namespace LegacyRenewalApp
{
    public class TaxRateProvider : ITaxRateProvider
    {
        private static readonly IReadOnlyDictionary<string, decimal> _taxRates =
            new Dictionary<string, decimal>
            {
                ["Poland"] = 0.23m,
                ["Germany"] = 0.19m,
                ["Czech Republic"] = 0.21m,
                ["Norway"] = 0.25m
            };

        public decimal GetTaxRate(string country)
        {
            return _taxRates.GetValueOrDefault(country, 0.20m);
        }
    }
}