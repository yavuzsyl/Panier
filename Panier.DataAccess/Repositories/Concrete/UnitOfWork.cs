using Panier.DataAccess.Data;
using Panier.DataAccess.Repositories.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Panier.DataAccess.Repositories.Concrete
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