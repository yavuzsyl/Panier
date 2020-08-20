using Panier.Domain.Entities;
using Panier.Domain.Repositories.Abstract;
using Panier.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Panier.Services.Concrete
{
    public class AdvertisementService : BaseService<Advertisement>, IAdvertisementService
    {
        public AdvertisementService(IUnitOfWork unitOfWork, IAdvertisementRepository repository) : base(unitOfWork, repository)
        {
        }
    }
}
