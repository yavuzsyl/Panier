
using Panier.Business.Services.Abstract;
using Panier.DataAccess.Repositories.Abstract;
using Panier.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Panier.Business.Services.Concrete
{
    public class AdvertisementService : BaseService<Advertisement>, IAdvertisementService
    {
        public AdvertisementService(IUnitOfWork unitOfWork, IAdvertisementRepository repository) : base(unitOfWork, repository)
        {
        }
    }
}
