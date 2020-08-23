using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Panier.Business.Contracts;
using Panier.Business.Contracts.V1.Requests;
using Panier.Business.Contracts.V1.Responses;
using Panier.Business.Extensions;
using Panier.Business.Services.Abstract;
using Panier.Entities;

namespace Panier.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class BasketController : ControllerBase
    {

        private readonly IBasketItemService basketItemService;
        private readonly IMapper mapper;
        public BasketController(IBasketItemService postService, IMapper mapper)
        {
            this.basketItemService = postService;
            this.mapper = mapper;
        }

        [HttpPost(ApiRoutes.Basket.Create)]
        public async Task<Response<BasketItem>> AddToBasket([FromBody, Required] AddToBasketRequestModel postRequest)
        {
            var currentUserId = HttpContext.GetUserId();
            return await basketItemService.AddToBasket(mapper.Map<BasketItem>(postRequest),currentUserId);
           
        }
    }
}
