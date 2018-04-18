using System;
using Xunit;
using Xunit.Extensions;
using LogServer;
using LogServer.Controllers;
using LogServer.Services;
using System.Globalization;
using System.Xml;
using MongoDB.Driver;
using MongoDB.Bson;
using Microsoft.Extensions.Configuration;
using System.IO;
using Xunit.Sdk;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Xunit.Abstractions;
using System.Reflection;

namespace XUnitTest
{
    public class LogsControllerTest
    {
        private const string APPLICATION_NAME = "xunit_test";
        private const string DB = "log";
        private const string LOG_COLLECTION = "log";

        [Fact]
        public void RunUnitTestsPart1()
        {
            Assert.True(this.CleanDatabaseCheck());
            Assert.True(this.InsertLogs());
            Assert.True(this.GetZeroLogs());
            Assert.True(this.TestPagination());
            Assert.True(this.TestDateRangeWithOneResult());
            Assert.True(this.TestDateRangeWithZeroResult());
            Assert.True(this.CleanDatabaseCheck());
        }

        private bool InsertLogs()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("logsSample.xml");
            foreach (XmlNode node in doc.DocumentElement.ChildNodes)
            {
                var logId = Convert.ToInt32(node["LOG_ID"].InnerText);
                var logTyp = String.IsNullOrEmpty(node["LOG_TYP"].InnerText) ? "" : node["LOG_TYP"].InnerText;
                var cat = String.IsNullOrEmpty(node["CAT"].InnerText) ? "" : node["CAT"].InnerText;
                var userId = String.IsNullOrEmpty(node["USR_ID"].InnerText) ? 0 : Convert.ToInt32(node["USR_ID"].InnerText);
                var userName = String.IsNullOrEmpty(node["NME"].InnerText) ? "" : node["NME"].InnerText;
                var dte = node["DTE"].InnerText;
                var det = String.IsNullOrEmpty(node["DET"].InnerText) ? "" : node["DET"].InnerText;
                var msg = String.IsNullOrEmpty(node["MSG"].InnerText) ? "" : node["MSG"].InnerText;
                var entId = String.IsNullOrEmpty(node["ENT_ID"].InnerText) ? 0 : Convert.ToInt32(node["ENT_ID"].InnerText);
                var entProdId = String.IsNullOrEmpty(node["ENT_PRO_ID"].InnerText) ? 0 : Convert.ToInt32(node["ENT_PRO_ID"].InnerText);

                var inserted = LogService.Instance.InsertCrEvents(logId,
                                                        logTyp,
                                                        cat,
                                                        dte,
                                                        userId,
                                                        userName,
                                                        det,
                                                        msg,
                                                        entId,
                                                        entProdId,
                                                        "xunit_test");
                if (!inserted)
                    return false;
            }
            return true;

        }

        private bool GetZeroLogs()
        {
            var startDate = "29/03/2018 00:00:00";
            var endDate = "29/03/2018 23:59:59";
            var limit = 5000;
            var page = 1;
            var pageSize = 10;
            var logList = LogService.Instance.GetLogsJson(APPLICATION_NAME,
                                                          out long nbPages,
                                                          DateTime.ParseExact(startDate, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                                                          DateTime.ParseExact(endDate, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                                                          limit,
                                                          page,
                                                          pageSize,
                                                          "",
                                                          "");
            return (nbPages == 0);
        }

        public bool TestPagination()
        {
            var startDate = "01/01/2018 14:00:09";
            var endDate = "12/01/2019 14:00:09";
            var limit = 5000;
            var page = 1;
            var pageSize = 10;
            var logList = LogService.Instance.GetLogsJson(APPLICATION_NAME,
                                                          out long nbPages,
                                                          DateTime.ParseExact(startDate, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                                                          DateTime.ParseExact(endDate, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                                                          limit,
                                                          page,
                                                          pageSize,
                                                          "",
                                                          "");
            var log = logList.First();
            Console.WriteLine("TEST" + log.ToJson().ToString());
            return (log.ApplicationName == "xunit_test" &&
                    log.Cat == "Projects" &&
                    log.Det == "Add document revision to project" &&
                    log.Dte.ToString("dd/MM/yyyy HH:mm:ss") == "19/01/2018 07:16:22" &&
                    log.EndProdId == 0 &&
                    log.EntId == 41974 &&
                    log.LogId == 31465526 &&
                    log.LogTyp == "Audit" &&
                    log.Msg == "The document revision 644023 has been added to the project \"Test Mustafah\" (3551) test" &&
                    log.UserId == 1 &&
                    log.UserName == "Developpement");
        }

        public bool TestDateRangeWithOneResult()
        {
            var startDate = "12/01/2018 14:00:09";
            var endDate = "12/01/2018 14:00:09";
            var limit = 5000;
            var page = 1;
            var pageSize = 10;
            var logList = LogService.Instance.GetLogsJson(APPLICATION_NAME,
                                                          out long nbPages,
                                                          DateTime.ParseExact(startDate, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                                                          DateTime.ParseExact(endDate, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                                                          limit,
                                                          page,
                                                          pageSize,
                                                          "",
                                                          "");
            var log = logList.First();
            
            return (log.ApplicationName == "xunit_test" && 
                    log.Cat == "BusinessRules_Error" &&
                    log.Det == "PRIIPs Cost Calculation (RIY calculation & Total Cost calculation)" &&
                    log.Dte.ToString("dd/MM/yyyy HH:mm:ss") == "12/01/2018 14:00:09" &&
                    log.EndProdId == 0 &&
                    log.EntId == 39214 &&
                    log.LogId == 31461091 &&
                    log.LogTyp == "Audit" &&
                    log.Msg == "Dynamic information mapping for code 'PRIIPS_RIY_PERIOD_1' not found" &&
                    log.UserId == 33 &&
                    log.UserName == "Co-React Automation" &&
                    nbPages == 1);
        }

        private bool TestDateRangeWithZeroResult()
        {
            var startDate = "12/01/2018 14:00:10";
            var endDate = "12/01/2018 14:00:10";
            var limit = 5000;
            var page = 1;
            var pageSize = 10;
            var logList = LogService.Instance.GetLogsJson(APPLICATION_NAME,
                                                          out long nbPages,
                                                          DateTime.ParseExact(startDate, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                                                          DateTime.ParseExact(endDate, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                                                          limit,
                                                          page,
                                                          pageSize,
                                                          "",
                                                          "");
            return (nbPages == 0);
        }

        private bool CleanDatabaseCheck()
        {
            this.CleanDataBase();
            var startDate = "01/01/2001 14:00:10";
            var endDate = "30/03/2018 14:00:10";
            var limit = 5000;
            var page = 1;
            var pageSize = 10;
            var logList = LogService.Instance.GetLogsJson(APPLICATION_NAME,
                                                          out long nbPages,
                                                          DateTime.ParseExact(startDate, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                                                          DateTime.ParseExact(endDate, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                                                          limit,
                                                          page,
                                                          pageSize,
                                                          "",
                                                          "");
            return (nbPages == 0);
        }

        private void CleanDataBase()
        {
            var filter = Builders<BsonDocument>.Filter.Eq("application_name", APPLICATION_NAME);
            var client = new MongoClient(this.ConnectionString());
            var database = client.GetDatabase(DB);
            var collection = database.GetCollection<BsonDocument>(LOG_COLLECTION).DeleteMany(filter);
        }

        private string ConnectionString()
        {
            var builder = new ConfigurationBuilder()
                                .SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile("appsettings.json");
            var config = builder.Build();
            return config["AppConfiguration:connectionString"];
        }
    }
}
