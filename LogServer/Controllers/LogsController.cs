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
using System.Globalization;

namespace LogServer.Controllers
{
    [Route("api/[controller]/[action]")]
    public class LogsController : Controller
    {

        public LogsController(IConfiguration configuration) => Configuration = configuration;

        public static IConfiguration Configuration { get; set; }

        [HttpPost]
        public IActionResult Logs([FromBody]JObject request)
        {
            try
            {
                string application = request["application"].ToString();
                string startDate = request["startDate"].ToString();
                string endDate = request["endDate"].ToString();
                string userId = String.IsNullOrEmpty(request["userId"].ToString()) ? null : request["userId"].ToString();
                string entityID = String.IsNullOrEmpty(request["entityID"].ToString()) ? null : request["entityID"].ToString();
                int limit = request["limit"].ToObject<int>();
                int page = request["page"].ToObject<int>();
                int pageSize = request["pageSize"].ToObject<int>();

                if (String.IsNullOrEmpty(application))
                    application = "";
                if (String.IsNullOrEmpty(startDate))
                    startDate = DateTime.Now.AddDays(-365).ToString();
                if (String.IsNullOrEmpty(endDate))
                    endDate = DateTime.Now.ToString();
                long nbPages;
                var logList = LogService.Instance.GetLogsJson(application, 
                                                              out nbPages,
                                                              DateTime.ParseExact(startDate, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture), 
                                                              DateTime.ParseExact(endDate, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture), 
                                                              limit, 
                                                              page, 
                                                              pageSize,
                                                              userId,
                                                              entityID);
                return Json(new {
                    totalPages = nbPages,
                    logs = logList
                });
            }
            catch (Exception e)
            {
                var errorMessage = e.Message + "params: " + request.ToString();
                LogService.Instance.LogException(0, errorMessage, "log_server");
                throw;
            }
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
                LogService.Instance.LogException(logId, e.Message, "log_server");
                throw;
            }
        }

    }
}
