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
    public class Log
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }
        [BsonElement("line")]
        [JsonProperty("line")]
        public int Line { get; set; }
        [BsonElement("application_name")]
        [JsonProperty("application_name")]
        public string ApplicationName { get; set; }
        [BsonElement("data")]
        [JsonProperty("data")]
        public string Data { get; set; }
        [BsonElement("date")]
        [JsonProperty("date")]
        public string Date { get; set; }
        [BsonElement("date_time")]
        [JsonProperty("date_time")]
        public DateTime DateTime { get; set; }
        [BsonElement("host")]
        [JsonProperty("host")]
        public string Host { get; set; }
        [BsonElement("logname")]
        [JsonProperty("logname")]
        public string Logname { get; set; }
        [BsonElement("user")]
        [JsonProperty("user")]
        public string User { get; set; }
        [BsonElement("time")]
        [JsonProperty("time")]
        public string Time { get; set; }
        [BsonElement("path")]
        [JsonProperty("path")]
        public string Path { get; set; }
        [BsonElement("request")]
        [JsonProperty("request")]
        public string Request { get; set; }
        [BsonElement("status")]
        [JsonProperty("status")]
        public string Status { get; set; }
        [BsonElement("reponse_size")]
        [JsonProperty("reponse_size")]
        public string ResponseSize { get; set; }
        [BsonElement("referrer")]
        [JsonProperty("referrer")]
        public string Referrer { get; set; }
        [BsonElement("user_agent")]
        [JsonProperty("user_agent")]
        public string UserAgent { get; set; }
        [BsonElement("process")]
        [JsonProperty("process")]
        public string Process { get; set; }
    }
}
