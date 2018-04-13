using LogServer.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Globalization;
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

        private MongoClient client;
        private IMongoDatabase database;

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
                            instance = new LogService(config["AppConfiguration:connectionString"], config["AppConfiguration:db"]);
                        }
                    }
                }

                return instance;
            }
        }

        private string connectionString;

        public LogService(string inConnectionString, string db)
        {
            connectionString = inConnectionString;
            client = new MongoClient(this.connectionString);
            database = client.GetDatabase(db);
        }

        public bool InsertCrEvents(int logId, string logType, string category, string date, int userId, string userName, string details, string message, int? entId = null, int? entProdId = null)
        {
            try
            {
                
                var collection = database.GetCollection<CoreactAuditLog>("log");
                collection.InsertOneAsync(new CoreactAuditLog
                {
                    LogId = logId,
                    LogTyp = logType,
                    Cat = category,
                    Dte = DateTime.ParseExact(date, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                    UserId = userId,
                    UserName = userName,
                    Det = details,
                    Msg = message,
                    EntId = entId,
                    EndProdId = entProdId,
                    ApplicationName = "coreact_audits"
                });
            }
            catch (Exception e)
            {
                this.LogException(logId, e.Message, "coreact_audits");
                return false;
            }
            return true;
        }

        public void LogException(int logId, string errorMessage, string application)
        {
            var collection = database.GetCollection<ExceptionLogger>("log");
            collection.InsertOneAsync(new ExceptionLogger
            {
                LogId = logId,
                ErrorMessage = errorMessage,
                ApplicationName = "log_server"
            });
        }

        public List<CoreactAuditLog> GetLogsJson(string applicationName, out long nbPages, DateTime starDate, DateTime endDate, int? limit = null, int page = 1, int pageSize = 10, string userId = null, string entityId = null)
        {
            var client = new MongoClient(this.connectionString);
            var database = client.GetDatabase("log");
            var collection = database.GetCollection<BsonDocument>("log");
            if (applicationName.ToLower() == "global")
                applicationName = null;
            var applicationNameQuery = String.IsNullOrEmpty(applicationName) ? String.Format("application_name: {{ $ne:null }}") : String.Format("application_name: '{0}'", applicationName);
            var userIdQuery = String.IsNullOrEmpty(userId) ? "" : String.Format("user_id: {0}, ", userId);
            var entityIdQuery = String.IsNullOrEmpty(entityId) ? "" : String.Format("ent_id: {0}, ", entityId);
            //var dataQuery = String.IsNullOrEmpty(data) ? String.Format(", data: {{ $ne:null }}") : String.Format(", data: RegExp('{0}')", data);
            var filter = String.Format(@"{{{0}, {1}{2}dte: {{$gte: ISODate('{3}'), $lte: ISODate('{4}')}}}}",
                                         applicationNameQuery,
                                         userIdQuery,
                                         entityIdQuery,
                                         starDate.AddHours(-1).ToString("yyyy-MM-ddTHH:mm:ssZ"),
                                         endDate.AddHours(-1).ToString("yyyy-MM-ddTHH:mm:ssZ"));

            if (limit == null || limit == 0)
                limit = 500;
            if (page < 1)
                page = 1;
            int currentPage = (page - 1) * pageSize;
            nbPages = collection.Find(filter).Count();
            var documentArray = collection.Find(filter).Skip(currentPage).Limit(pageSize).ToList();
            var result = new List<CoreactAuditLog>();
            foreach (var document in documentArray)
            {
                var log = BsonSerializer.Deserialize<CoreactAuditLog>(document);
                result.Add(log);
            }
            return result;
        }
    }
}
