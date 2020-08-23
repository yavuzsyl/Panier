using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Panier.Business.Contracts.V1.Responses
{
    public class ModelStateErrorResponse
    {
        public List<ModelStateErrorModel> Errors { get; set; } = new List<ModelStateErrorModel>();
    }

    public class ModelStateErrorModel
    {
        public string FieldName { get; set; }
        public string Message { get; set; }
    }
}
