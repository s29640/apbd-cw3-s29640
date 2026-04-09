using LegacyRenewalApp;
using System;

namespace LegacyRenewalApp
{
    public class SubscriptionRenewalService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ISubscriptionPlanRepository _planRepository;
        private readonly IBillingGateway _billingGateway;
        private readonly IDiscountCalculator _discountCalculator;
        private readonly ISupportFeeCalculator _supportFeeCalculator;
        private readonly IPaymentFeeCalculator _paymentFeeCalculator;
        private readonly ITaxRateProvider _taxRateProvider;

        public SubscriptionRenewalService()
            : this(
                new CustomerRepository(),
                new SubscriptionPlanRepository(),
                new LegacyBillingGatewayAdapter(),
                new DiscountCalculator(),
                new SupportFeeCalculator(),
                new PaymentFeeCalculator(),
                new TaxRateProvider())
        {
        }

        public SubscriptionRenewalService(
            ICustomerRepository customerRepository,
            ISubscriptionPlanRepository planRepository,
            IBillingGateway billingGateway,
            IDiscountCalculator discountCalculator,
            ISupportFeeCalculator supportFeeCalculator,
            IPaymentFeeCalculator paymentFeeCalculator,
            ITaxRateProvider taxRateProvider)
        {
            _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
            _planRepository = planRepository ?? throw new ArgumentNullException(nameof(planRepository));
            _billingGateway = billingGateway ?? throw new ArgumentNullException(nameof(billingGateway));
            _discountCalculator = discountCalculator ?? throw new ArgumentNullException(nameof(discountCalculator));
            _supportFeeCalculator = supportFeeCalculator ?? throw new ArgumentNullException(nameof(supportFeeCalculator));
            _paymentFeeCalculator = paymentFeeCalculator ?? throw new ArgumentNullException(nameof(paymentFeeCalculator));
            _taxRateProvider = taxRateProvider ?? throw new ArgumentNullException(nameof(taxRateProvider));
        }

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

            var discountResult = _discountCalculator.Calculate(
                customer,
                plan,
                seatCount,
                useLoyaltyPoints,
                baseAmount);

            decimal discountAmount = discountResult.DiscountAmount;
            string notes = discountResult.Notes;

            decimal subtotalAfterDiscount = ApplyMinimumDiscountedSubtotal(
                baseAmount - discountAmount,
                ref notes);

            var supportFeeResult = _supportFeeCalculator.Calculate(
                includePremiumSupport,
                normalizedPlanCode);

            decimal supportFee = supportFeeResult.SupportFee;
            notes += supportFeeResult.Notes;

            var paymentFeeResult = _paymentFeeCalculator.Calculate(
                normalizedPaymentMethod,
                subtotalAfterDiscount,
                supportFee);

            decimal paymentFee = paymentFeeResult.PaymentFee;
            notes += paymentFeeResult.Notes;

            decimal taxRate = _taxRateProvider.GetTaxRate(customer.Country);
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

        private Customer GetCustomer(int customerId)
        {
            return _customerRepository.GetById(customerId);
        }

        private SubscriptionPlan GetPlan(string normalizedPlanCode)
        {
            return _planRepository.GetByCode(normalizedPlanCode);
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

        private static decimal ApplyMinimumDiscountedSubtotal(decimal subtotalAfterDiscount, ref string notes)
        {
            if (subtotalAfterDiscount < 300m)
            {
                subtotalAfterDiscount = 300m;
                notes += "minimum discounted subtotal applied; ";
            }

            return subtotalAfterDiscount;
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

        private void SaveInvoice(RenewalInvoice invoice)
        {
            _billingGateway.SaveInvoice(invoice);
        }

        private void SendInvoiceEmail(
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

            _billingGateway.SendEmail(customer.Email, subject, body);
        }
    }
}