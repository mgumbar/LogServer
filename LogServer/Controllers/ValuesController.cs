using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using System.IO;
using LogServer.Services;

namespace LogServer.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {

        public ValuesController(IConfiguration configuration) => Configuration = configuration;

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

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

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
