using System;
using System.Collections.Generic;
using System.Linq;
using EchoBot1.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace EchoBot1.Services
{
    public class JsonReader
    {
        public List<JObject> Index()
        {
            //Start by getting the Json filepath
            var path = AppContext.BaseDirectory + $"/Questions.json";
            var content = new List<JObject>();

            if (File.Exists(path))
            {
                content = JsonConvert.DeserializeObject<List<JObject>>(File.ReadAllText(path));
            }

            return content;
        }
    }
}

//stepContext.Values["description"]  =  (string)stepContext.Result;     --//stepContext is an object where values in that conversation are saved to.(it is NOT saved in state)

    