using Panier.Domain.Data;
using Panier.Domain.Repositories.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Panier.Domain.Repositories.Concrete
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly PanierContext context;

        public UnitOfWork(PanierContext context)
        {
            this.context = context;
        }
        public async Task<bool> CompleteAsync()
        {
            return await context.SaveChangesAsync() > 0;
        }
    }
}
