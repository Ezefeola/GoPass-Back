using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoPass.Domain.DTOs.Request.PaymentRequestDTOs.Stripe
{
    public class CreatePaymentIntentRequestDto
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; }
    }
}
