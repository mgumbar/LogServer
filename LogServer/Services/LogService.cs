using LogServer.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LogServer.Services
{
    public class LogService
    {
        // -----------------------------------------------------------
        // Singleton
        // -----------------------------------------------------------

        private static object syncRoot = new object();

        private static LogService instance;

        public static LogService Instance
        {

            get
            {
                var builder = new ConfigurationBuilder()
                                    .SetBasePath(Directory.GetCurrentDirectory())
                                    .AddJsonFile("appsettings.json");
                var config = builder.Build();
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new LogService(config["AppConfiguration:connectionString"]);
                        }
                    }
                }

                return instance;
            }
        }

        private string connectionString;

        public LogService(string inConnectionString)
        {
            connectionString = inConnectionString;

        }

        public List<string> GetLogsJson(string applicationName, DateTime starDate, DateTime endDate, string data, string logName)
        {
            var client = new MongoClient(this.connectionString);
            var database = client.GetDatabase("log");
            var collection = database.GetCollection<BsonDocument>("log");
            if (applicationName.ToLower() == "global")
                applicationName = null;
            var applicationNameQuery = String.IsNullOrEmpty(applicationName) ? String.Format("application_name: {{ $ne:null }}") : String.Format("application_name: '{0}'", applicationName);
            var dataQuery = String.IsNullOrEmpty(data) ? String.Format(", data: {{ $ne:null }}") : String.Format(", data: RegExp('{0}')", data);
            var logNameQuery = String.IsNullOrEmpty(logName) ? String.Format(", logname: {{ $ne:null }}") : String.Format(", logname: '{0}", logName);
            var filter = String.Format(@"{{{0}, date_time: {{$gte: ISODate('{1}'), $lte: ISODate('{2}')}}{3}{4} }}",
                                         applicationNameQuery,
                                         starDate.AddHours(-2).ToString("yyyy-MM-ddTHH:mm:ssZ"),
                                         endDate.AddHours(-2).ToString("yyyy-MM-ddTHH:mm:ssZ"),
                                         dataQuery,
                                         logNameQuery);

            //& builder.Eq("logname", logName);
            var documentArray = collection.Find(filter).Limit(500).ToList();
            var result = new List<string>();
            foreach (var document in documentArray)
            {
                var log = document.ToJson();
                result.Add(log);
            }
            return result;
        }
    }
}
