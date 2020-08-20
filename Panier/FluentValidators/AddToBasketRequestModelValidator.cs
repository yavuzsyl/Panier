﻿using FluentValidation;
using Panier.Contracts.V1.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Panier.FluentValidators
{
    public class AddToBasketRequestModelValidator : AbstractValidator<AddToBasketRequestModel>
    {
        public AddToBasketRequestModelValidator()
        {
            RuleFor(x => x.AdvertisementId).NotEmpty().WithMessage("İlan id değeri boş girilimez").Must(x =>
            {   return x > 0; }).WithMessage("İlan id değeri 0 dan büyük olmalıdır");
            RuleFor(x => x.Count).NotEmpty().WithMessage("Sepet ürün adet bilgisi boş girilimez").Must(x =>
            { return x > 0; }).WithMessage("Sepet ürün adedi 0 dan büyük olmalıdır");

            //gibi
        }
    }
}
