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
    public class CoreactAuditLog
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }
        [BsonElement("log_id")]
        [JsonProperty("log_id")]
        public int LogId { get; set; }
        [BsonElement("log_typ")]
        [JsonProperty("log_typ")]
        public string LogTyp { get; set; }
        [BsonElement("cat")]
        [JsonProperty("cat")]
        public string Cat { get; set; }
        [BsonElement("user_name")]
        [JsonProperty("user_name")]
        public string UserName { get; set; }
        [BsonElement("user_id")]
        [JsonProperty("user_id")]
        public int UserId { get; set; }
        [BsonElement("dte")]
        [JsonProperty("dte")]
        public DateTime Dte { get; set; }
        [BsonElement("det")]
        [JsonProperty("det")]
        public string Det { get; set; }
        [BsonElement("msg")]
        [JsonProperty("msg")]
        public string Msg { get; set; }
        [BsonElement("ent_id")]
        [JsonProperty("ent_id")]
        public int? EntId { get; set; }
        [BsonElement("ent_pro_id")]
        [JsonProperty("ent_pro_id")]
        public int? EndProdId { get; set; }
        [BsonElement("application_name")]
        [JsonProperty("application_name")]
        public string ApplicationName { get; set; }
    }
}
