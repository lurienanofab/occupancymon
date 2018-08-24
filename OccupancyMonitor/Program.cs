using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Configuration;
using RestSharp.Authenticators;

namespace OccupancyMonitor
{
    class Program
    {
        static void Main(string[] args)
        {
            string profile = args[0];
            var profiles = GetProfiles();
            var p = profiles.FirstOrDefault(x => x.Name == profile);

            IRestClient client = new RestClient(ConfigurationManager.AppSettings["FeedUrl"]);

            IRestRequest req = new RestRequest();
            req.AddQueryParameter("area", p.AreaName);

            var resp = client.Execute(req);

            if (resp.ErrorException == null)
            {
                var jobj = JObject.Parse(resp.Content);
                var count = jobj["Data"]["default"].First["CurrentlyInLabCount"].Value<int>();
                HandleCount(p, count);
            }
        }

        private static void HandleCount(Profile p, int count)
        {
            bool state = count <= p.Trigger ? p.State : !p.State;
            var resp = SetState(p, state);
            AddLogEntry(p, count, state, resp);
        }

        private static void AddLogEntry(Profile p, int count, bool state, IRestResponse resp)
        {
            var entries = JsonConvert.DeserializeObject<IEnumerable<LogEntry>>(File.ReadAllText("log.json")).ToList();

            var entry = new LogEntry()
            {
                EntryDateTime = DateTime.Now,
                AreaName = p.AreaName,
                Trigger = p.Trigger,
                Count = count,
                State = state,
                StatusCode = (int)resp.StatusCode,
                Status = resp.StatusDescription,
                Error = false,
                Message = ""
            };

            if (resp.IsSuccessful)
            {
                var jobj = JObject.Parse(resp.Content);
                entry.Error = jobj["Error"].Value<bool>();
                entry.Message = jobj["Message"].Value<string>();
            }

            entries.Add(entry);

            string json = JsonConvert.SerializeObject(entries.OrderByDescending(x => x.EntryDateTime).ToArray(), Formatting.Indented);

            File.WriteAllText("log.json", json);
        }

        private static IRestResponse SetState(Profile p, bool state)
        {
            IRestClient client = new RestClient(ConfigurationManager.AppSettings["ApiHost"]);
            client.Authenticator = new HttpBasicAuthenticator(ConfigurationManager.AppSettings["BasicAuthUsername"], ConfigurationManager.AppSettings["BasicAuthPassword"]);
            IRestRequest req = new RestRequest("control/point/{pointId}/{state}");
            req.AddUrlSegment("pointId", p.PointID.ToString());
            req.AddUrlSegment("state", state ? "on" : "off");
            return client.Execute(req);
        }

        private static IEnumerable<Profile> GetProfiles()
        {
            var json = File.ReadAllText("profiles.json");
            return JsonConvert.DeserializeObject<IEnumerable<Profile>>(json);
        }
    }
}
