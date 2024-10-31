using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoPass.Domain.DTOs.Request.PaymentRequestDTOs.Stripe
{
    public class ConfirmPaymentIntentRequestDto
    {
        public string PaymentIntentId { get; set; }
        public string PaymentMethodId { get; set; }
        public int TicketId { get; set; }
        public int UserId { get; set; }
    }
}
