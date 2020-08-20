using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Panier.Domain.Repositories.Abstract
{
    public interface IUnitOfWork
    {
      Task<bool> CompleteAsync();
    }
}
