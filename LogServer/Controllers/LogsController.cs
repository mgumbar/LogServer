using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using System.IO;
using LogServer.Services;
using LogServer.Models;
using MongoDB.Bson.Serialization;
using Newtonsoft.Json.Linq;

namespace LogServer.Controllers
{
    [Route("api/[controller]/[action]")]
    public class LogsController : Controller
    {

        public LogsController(IConfiguration configuration) => Configuration = configuration;

        public static IConfiguration Configuration { get; set; }
        // GET api/values
        [HttpGet]
        public List<string> Get()
        {
            var builder = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json");
            var config = builder.Build();

            var startDate = DateTime.Now.AddDays(-365).ToString();
            var endDate = DateTime.Now.ToString();
            var logList  = LogService.Instance.GetLogsJson("", DateTime.Parse(startDate), DateTime.Parse(endDate), "", "");


            return logList;
        }

        [HttpPost]
        public List<string> Get(string application, string startDate, string endDate, string data, string Username, int limit, int page, int pageSize)
        {
            var builder = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json");
            var config = builder.Build();

            if (String.IsNullOrEmpty(application))
                application = "";
            if (String.IsNullOrEmpty(startDate))
                startDate = DateTime.Now.AddDays(-365).ToString();
            if (String.IsNullOrEmpty(endDate))
                endDate = DateTime.Now.ToString();
            var logList = LogService.Instance.GetLogsJson(application, DateTime.Parse(startDate), DateTime.Parse(endDate), "", "", limit, page, pageSize);

            var result = new List<CoreactAuditLog>();
            foreach (var document in logList)
            {
                var log = BsonSerializer.Deserialize<CoreactAuditLog>(document);
                result.Add(log);
            }

            return logList;
        }

        [HttpPost]
        public bool LogCoreactEvents([FromBody]JObject request)
        {
            try
            {

                int? entId = String.IsNullOrEmpty(request["entId"].ToString()) ? 0 : request["entId"].ToObject<int>();
                int? entProdId = String.IsNullOrEmpty(request["entProdId"].ToString()) ? 0 : request["entProdId"].ToObject<int>();
                return LogService.Instance.InsertCrEvents(request["logId"].ToObject<int>(),
                                                        request["logType"].ToString(),
                                                        request["category"].ToString(),
                                                        request["date"].ToString(),
                                                        request["userId"].ToObject<int>(),
                                                        request["userName"].ToString(),
                                                        request["details"].ToString(),
                                                        request["message"].ToString(),
                                                        entId,
                                                        entProdId);
            }
            catch (Exception e)
            {
                var logId = String.IsNullOrEmpty(request["logId"].ToString()) ? 0 : request["logId"].ToObject<int>();
                LogService.Instance.LogException(logId, e.Message, "coreact_audits");
                throw;
            }
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        //[HttpPost]
        //public List<string> Post([FromBody]string application, string startDate, string endDate, string data)
        //{
        //    var builder = new ConfigurationBuilder()
        //    .SetBasePath(Directory.GetCurrentDirectory())
        //    .AddJsonFile("appsettings.json");
        //    var config = builder.Build();

        //    if (String.IsNullOrEmpty(startDate))
        //        startDate = DateTime.Now.AddDays(-365).ToString();
        //    if (String.IsNullOrEmpty(endDate))
        //        endDate = DateTime.Now.ToString();
        //    var logList = LogService.Instance.GetLogsJson(application, DateTime.Parse(startDate), DateTime.Parse(endDate), "", "");


        //    return logList;
        //}

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
