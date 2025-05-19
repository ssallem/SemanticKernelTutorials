using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SK_Tutorials.Interfaces
{
    public interface IUserContext
    {
        Guid GetCartId();
        Task<Guid> GetCartIdAsync();
    }
}
