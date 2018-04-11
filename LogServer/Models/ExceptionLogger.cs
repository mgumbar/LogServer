using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogServer.Models
{
    [BsonIgnoreExtraElements]
    public class ExceptionLogger
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }
        [BsonElement("log_id")]
        [JsonProperty("log_id")]
        public int LogId { get; set; }
        [BsonElement("error_message")]
        [JsonProperty("error_message")]
        public string ErrorMessage { get; set; }
        [BsonElement("application_name")]
        [JsonProperty("application_name")]
        public string ApplicationName { get; set; }
    }
}
