using System;

namespace LegacyRenewalApp
{
    public class SubscriptionRenewalService
    {
        public RenewalInvoice CreateRenewalInvoice(
            int customerId,
            string planCode,
            int seatCount,
            string paymentMethod,
            bool includePremiumSupport,
            bool useLoyaltyPoints)
        {
            ValidateInput(customerId, planCode, seatCount, paymentMethod);

            string normalizedPlanCode = Normalize(planCode);
            string normalizedPaymentMethod = Normalize(paymentMethod);

            var customer = GetCustomer(customerId);
            var plan = GetPlan(normalizedPlanCode);

            EnsureCustomerIsActive(customer);

            decimal baseAmount = CalculateBaseAmount(plan, seatCount);

            string notes = string.Empty;
            decimal discountAmount = CalculateDiscountAmount(
                customer,
                plan,
                seatCount,
                useLoyaltyPoints,
                baseAmount,
                ref notes);

            decimal subtotalAfterDiscount = ApplyMinimumDiscountedSubtotal(
                baseAmount - discountAmount,
                ref notes);

            decimal supportFee = CalculateSupportFee(
                includePremiumSupport,
                normalizedPlanCode,
                ref notes);

            decimal paymentFee = CalculatePaymentFee(
                normalizedPaymentMethod,
                subtotalAfterDiscount,
                supportFee,
                ref notes);

            decimal taxRate = GetTaxRate(customer.Country);
            decimal taxBase = subtotalAfterDiscount + supportFee + paymentFee;
            decimal taxAmount = taxBase * taxRate;
            decimal finalAmount = taxBase + taxAmount;

            finalAmount = ApplyMinimumInvoiceAmount(finalAmount, ref notes);

            var invoice = BuildInvoice(
                customerId,
                seatCount,
                normalizedPlanCode,
                normalizedPaymentMethod,
                customer,
                baseAmount,
                discountAmount,
                supportFee,
                paymentFee,
                taxAmount,
                finalAmount,
                notes);

            SaveInvoice(invoice);
            SendInvoiceEmail(customer, normalizedPlanCode, invoice);

            return invoice;
        }

        private static void ValidateInput(
            int customerId,
            string planCode,
            int seatCount,
            string paymentMethod)
        {
            if (customerId <= 0)
            {
                throw new ArgumentException("Customer id must be positive");
            }

            if (string.IsNullOrWhiteSpace(planCode))
            {
                throw new ArgumentException("Plan code is required");
            }

            if (seatCount <= 0)
            {
                throw new ArgumentException("Seat count must be positive");
            }

            if (string.IsNullOrWhiteSpace(paymentMethod))
            {
                throw new ArgumentException("Payment method is required");
            }
        }

        private static string Normalize(string value)
        {
            return value.Trim().ToUpperInvariant();
        }

        private static Customer GetCustomer(int customerId)
        {
            var customerRepository = new CustomerRepository();
            return customerRepository.GetById(customerId);
        }

        private static SubscriptionPlan GetPlan(string normalizedPlanCode)
        {
            var planRepository = new SubscriptionPlanRepository();
            return planRepository.GetByCode(normalizedPlanCode);
        }

        private static void EnsureCustomerIsActive(Customer customer)
        {
            if (!customer.IsActive)
            {
                throw new InvalidOperationException("Inactive customers cannot renew subscriptions");
            }
        }

        private static decimal CalculateBaseAmount(SubscriptionPlan plan, int seatCount)
        {
            return (plan.MonthlyPricePerSeat * seatCount * 12m) + plan.SetupFee;
        }

        private static decimal CalculateDiscountAmount(
            Customer customer,
            SubscriptionPlan plan,
            int seatCount,
            bool useLoyaltyPoints,
            decimal baseAmount,
            ref string notes)
        {
            decimal discountAmount = 0m;

            if (customer.Segment == "Silver")
            {
                discountAmount += baseAmount * 0.05m;
                notes += "silver discount; ";
            }
            else if (customer.Segment == "Gold")
            {
                discountAmount += baseAmount * 0.10m;
                notes += "gold discount; ";
            }
            else if (customer.Segment == "Platinum")
            {
                discountAmount += baseAmount * 0.15m;
                notes += "platinum discount; ";
            }
            else if (customer.Segment == "Education" && plan.IsEducationEligible)
            {
                discountAmount += baseAmount * 0.20m;
                notes += "education discount; ";
            }

            if (customer.YearsWithCompany >= 5)
            {
                discountAmount += baseAmount * 0.07m;
                notes += "long-term loyalty discount; ";
            }
            else if (customer.YearsWithCompany >= 2)
            {
                discountAmount += baseAmount * 0.03m;
                notes += "basic loyalty discount; ";
            }

            if (seatCount >= 50)
            {
                discountAmount += baseAmount * 0.12m;
                notes += "large team discount; ";
            }
            else if (seatCount >= 20)
            {
                discountAmount += baseAmount * 0.08m;
                notes += "medium team discount; ";
            }
            else if (seatCount >= 10)
            {
                discountAmount += baseAmount * 0.04m;
                notes += "small team discount; ";
            }

            if (useLoyaltyPoints && customer.LoyaltyPoints > 0)
            {
                int pointsToUse = customer.LoyaltyPoints > 200 ? 200 : customer.LoyaltyPoints;
                discountAmount += pointsToUse;
                notes += $"loyalty points used: {pointsToUse}; ";
            }

            return discountAmount;
        }

        private static decimal ApplyMinimumDiscountedSubtotal(decimal subtotalAfterDiscount, ref string notes)
        {
            if (subtotalAfterDiscount < 300m)
            {
                subtotalAfterDiscount = 300m;
                notes += "minimum discounted subtotal applied; ";
            }

            return subtotalAfterDiscount;
        }

        private static decimal CalculateSupportFee(
            bool includePremiumSupport,
            string normalizedPlanCode,
            ref string notes)
        {
            decimal supportFee = 0m;

            if (includePremiumSupport)
            {
                if (normalizedPlanCode == "START")
                {
                    supportFee = 250m;
                }
                else if (normalizedPlanCode == "PRO")
                {
                    supportFee = 400m;
                }
                else if (normalizedPlanCode == "ENTERPRISE")
                {
                    supportFee = 700m;
                }

                notes += "premium support included; ";
            }

            return supportFee;
        }

        private static decimal CalculatePaymentFee(
            string normalizedPaymentMethod,
            decimal subtotalAfterDiscount,
            decimal supportFee,
            ref string notes)
        {
            decimal paymentFee = 0m;
            decimal feeBase = subtotalAfterDiscount + supportFee;

            if (normalizedPaymentMethod == "CARD")
            {
                paymentFee = feeBase * 0.02m;
                notes += "card payment fee; ";
            }
            else if (normalizedPaymentMethod == "BANK_TRANSFER")
            {
                paymentFee = feeBase * 0.01m;
                notes += "bank transfer fee; ";
            }
            else if (normalizedPaymentMethod == "PAYPAL")
            {
                paymentFee = feeBase * 0.035m;
                notes += "paypal fee; ";
            }
            else if (normalizedPaymentMethod == "INVOICE")
            {
                paymentFee = 0m;
                notes += "invoice payment; ";
            }
            else
            {
                throw new ArgumentException("Unsupported payment method");
            }

            return paymentFee;
        }

        private static decimal GetTaxRate(string country)
        {
            decimal taxRate = 0.20m;

            if (country == "Poland")
            {
                taxRate = 0.23m;
            }
            else if (country == "Germany")
            {
                taxRate = 0.19m;
            }
            else if (country == "Czech Republic")
            {
                taxRate = 0.21m;
            }
            else if (country == "Norway")
            {
                taxRate = 0.25m;
            }

            return taxRate;
        }

        private static decimal ApplyMinimumInvoiceAmount(decimal finalAmount, ref string notes)
        {
            if (finalAmount < 500m)
            {
                finalAmount = 500m;
                notes += "minimum invoice amount applied; ";
            }

            return finalAmount;
        }

        private static RenewalInvoice BuildInvoice(
            int customerId,
            int seatCount,
            string normalizedPlanCode,
            string normalizedPaymentMethod,
            Customer customer,
            decimal baseAmount,
            decimal discountAmount,
            decimal supportFee,
            decimal paymentFee,
            decimal taxAmount,
            decimal finalAmount,
            string notes)
        {
            var generatedAt = DateTime.UtcNow;

            return new RenewalInvoice
            {
                InvoiceNumber = $"INV-{generatedAt:yyyyMMdd}-{customerId}-{normalizedPlanCode}",
                CustomerName = customer.FullName,
                PlanCode = normalizedPlanCode,
                PaymentMethod = normalizedPaymentMethod,
                SeatCount = seatCount,
                BaseAmount = Math.Round(baseAmount, 2, MidpointRounding.AwayFromZero),
                DiscountAmount = Math.Round(discountAmount, 2, MidpointRounding.AwayFromZero),
                SupportFee = Math.Round(supportFee, 2, MidpointRounding.AwayFromZero),
                PaymentFee = Math.Round(paymentFee, 2, MidpointRounding.AwayFromZero),
                TaxAmount = Math.Round(taxAmount, 2, MidpointRounding.AwayFromZero),
                FinalAmount = Math.Round(finalAmount, 2, MidpointRounding.AwayFromZero),
                Notes = notes.Trim(),
                GeneratedAt = generatedAt
            };
        }

        private static void SaveInvoice(RenewalInvoice invoice)
        {
            LegacyBillingGateway.SaveInvoice(invoice);
        }

        private static void SendInvoiceEmail(
            Customer customer,
            string normalizedPlanCode,
            RenewalInvoice invoice)
        {
            if (string.IsNullOrWhiteSpace(customer.Email))
            {
                return;
            }

            string subject = "Subscription renewal invoice";
            string body =
                $"Hello {customer.FullName}, your renewal for plan {normalizedPlanCode} " +
                $"has been prepared. Final amount: {invoice.FinalAmount:F2}.";

            LegacyBillingGateway.SendEmail(customer.Email, subject, body);
        }
    }
}