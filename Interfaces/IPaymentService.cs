using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SK_Tutorials.Interfaces
{
    public interface IPaymentService
    {
        Task<Guid> RequestPaymentFromUserAsync(Guid cardId);
    }
}
