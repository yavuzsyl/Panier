using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Panier.Core.Mongo;
using System;
using System.Collections.Generic;
using System.Text;

namespace Panier.Entities.Mongo
{
    public class StatusMessage
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string statusMessage { get; set; }
        public string statusName { get; set; }
        public int statusCode { get; set; }
    }
}
