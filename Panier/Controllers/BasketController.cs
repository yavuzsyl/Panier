﻿using System;
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
using Panier.Business.Extensions;
using Panier.Business.Services.Abstract;
using Panier.Entities;

namespace Panier.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class BasketController : ControllerBase
    {

        private readonly IBasketItemtService basketItemService;
        private readonly IMapper mapper;
        public BasketController(IBasketItemtService postService, IMapper mapper)
        {
            this.basketItemService = postService;
            this.mapper = mapper;
        }

        [HttpPost(ApiRoutes.Basket.Create)]
        public async Task<IActionResult> AddToBasket([FromBody, Required] AddToBasketRequestModel postRequest)
        {
            var currentUserId = HttpContext.GetUserId();
            var createResult = await basketItemService.AddToBasket(mapper.Map<BasketItem>(postRequest),currentUserId);
            return Ok(createResult);

        }
    }
}
