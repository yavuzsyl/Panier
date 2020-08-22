using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using Panier.Business.Contracts.V1.Responses;
using Panier.Business.Services.Abstract;
using Panier.Business.Services.Abstract.Mongo;
using Panier.Business.Services.Concrete;
using Panier.Core.LoggerService.Abstract;
using Panier.Core.Redis.Repository.Abstract;
using Panier.DataAccess.Repositories.Abstract;
using Panier.Entities;
using Panier.Entities.Mongo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using Xunit;

namespace Panier.UnitTests
{
    public class BasketItemServiceTest
    {
        private readonly IBasketItemService basketItemtService;
        private readonly Mock<IAdvertisementService> mockAdvertisementService;
        private readonly Mock<IRedisRepository> mockRedisRepo;
        private readonly Mock<IStatusMessageRepository> mockStatusMessageRepo;
        private readonly Mock<ILoggerManager> mockLoggerMan;
        private readonly Mock<IUnitOfWork> mockUnitOfWork;
        private readonly Mock<IBasketItemRepository> mockbasketItemRepo;
        private readonly Mock<IBaseRepository<BasketItem>> mockBaseRepo;

        public BasketItemServiceTest()
        {
            mockAdvertisementService = new Mock<IAdvertisementService>();
            mockRedisRepo = new Mock<IRedisRepository>();
            mockStatusMessageRepo = new Mock<IStatusMessageRepository>();
            mockLoggerMan = new Mock<ILoggerManager>();
            mockUnitOfWork = new Mock<IUnitOfWork>();
            mockbasketItemRepo = new Mock<IBasketItemRepository>();
            mockBaseRepo = new Mock<IBaseRepository<BasketItem>>();
            basketItemtService = new BasketItemService(mockUnitOfWork.Object, mockbasketItemRepo.Object, mockAdvertisementService.Object, mockStatusMessageRepo.Object, mockRedisRepo.Object, mockLoggerMan.Object);
        }

        [Theory]
        [InlineData("c136d274-a69a-4733-bb0e-985471b05391", 10)]
        public void AddToBasketItem_ShouldReturnBadRequest_IfAdvertisementNotExists(string userId, int advertitsementId)
        {
            mockAdvertisementService.Setup(x => x.FindEntityById(advertitsementId)).Returns(Task.FromResult(new Response<Advertisement>("Not Found")));

            var result = basketItemtService.AddToBasket(new BasketItem { AdvertisementId = advertitsementId, Count = 254 }, userId);

            Assert.False(result.Result.Success);
        }

        [Theory]
        [InlineData("c136d274-a69a-4733-bb0e-985471b05391", 2)]
        public void AddToBasketItem_ShouldReturnBadRequestWithStatusMessage_IfAdvertisementDeletedOrNotActive(string userId, int advertitsementId)
        {
            mockRedisRepo.Setup(x => x.GetObjectAsync<StatusMessage>("NotActiveAdvertisement")).Returns(Task.FromResult(new StatusMessage { statusMessage = "This advertisement is not active" }));

            mockAdvertisementService.Setup(x => x.FindEntityById(advertitsementId)).Returns(Task.FromResult(new Response<Advertisement>
           (new Advertisement { IsActive = false, IsDeleted = false })));

            var result = basketItemtService.AddToBasket(new BasketItem { AdvertisementId = advertitsementId, Count = 254 }, userId);

            Assert.False(result.Result.Success);
            Assert.Equal(400, result.Result.StatusCode);
            Assert.Equal("This advertisement is not active", result.Result.Message);
            Assert.Null(result.Result.Result);
        }

        [Theory]
        [InlineData("c136d274-a69a-4733-bb0e-985471b05391", 1, 250)]
        public void AddToBasketItem_ShouldReturnBadRequestWithStatusMessage_IfRequestedStockMoreThenCurrentStock(string userId, int advertitsementId, int requestedCount)
        {
            mockRedisRepo.Setup(x => x.GetObjectAsync<StatusMessage>("NotEnoughStockAdvertisement")).Returns(Task.FromResult(new StatusMessage { statusMessage = "Not enough stock for this advertisement" }));

            mockAdvertisementService.Setup(x => x.FindEntityById(advertitsementId)).Returns(Task.FromResult(new Response<Advertisement>
           (new Advertisement { IsActive = true, IsDeleted = false, UnitsInStock = 100 })));

            var result = basketItemtService.AddToBasket(new BasketItem { AdvertisementId = advertitsementId, Count = requestedCount }, userId);

            Assert.False(result.Result.Success);
            Assert.Equal(400, result.Result.StatusCode);
            Assert.Equal("Not enough stock for this advertisement", result.Result.Message);
            Assert.Null(result.Result.Result);
        }


        [Theory]
        [InlineData("9bee167a-34f4-4a56-9de9-07c332b6defd", 1, 10,20)]
        public void AddToBasketItem_ShouldUpdateAndReturnBasketItem_IfUserHasExistingBasketItem(string userId, int advertitsementId, int requestedCount,int existAdvertisementCount)
        {
            var addToRequest = new BasketItem { AdvertisementId = advertitsementId, Count = requestedCount };
            var advertisement = new Advertisement { Id = 1, IsActive = true, IsDeleted = false, UnitsInStock = 100, Price = 12M };
            mockAdvertisementService.Setup(x => x.FindEntityById(advertitsementId)).Returns(Task.FromResult(new Response<Advertisement>
           (advertisement)));

            var basketItemThatAlreadyExist = new BasketItem() { AppUserId = userId, IsDeleted = false, Count = existAdvertisementCount, TotalPrice = 120, Id = 1, AdvertisementId = 1 };
            var basketItems = new List<BasketItem>() { basketItemThatAlreadyExist }.AsQueryable();
            mockbasketItemRepo.Setup(x => x.GetByExpression(It.IsAny<Expression<Func<BasketItem, bool>>>())).Returns(Task.FromResult(basketItems.FirstOrDefault()));



            mockUnitOfWork.Setup(x => x.CompleteAsync()).Returns(Task.FromResult(true));

            var result = basketItemtService.AddToBasket(new BasketItem { AdvertisementId = advertitsementId, Count = requestedCount }, userId);

            Assert.True(result.Result.Success);
            Assert.Equal(200, result.Result.StatusCode);
            Assert.Equal(existAdvertisementCount + addToRequest.Count, result.Result.Result.Count);
            Assert.Equal(result.Result.Result.Count * advertisement.Price, result.Result.Result.TotalPrice);
        }
    }
}
