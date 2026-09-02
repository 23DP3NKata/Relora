using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relora.Payments.Domain.Enums;
public enum PaymentStatus
{
    Pending,
    Paid,
    Failed,
    Cancelled,
    Refunded
}
