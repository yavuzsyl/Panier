
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Panier.DataAccess.Repositories.Abstract
{
    public interface IUnitOfWork
    {
        Task<bool> CompleteAsync();
    }
}