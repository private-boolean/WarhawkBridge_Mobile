using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using Newtonsoft.Json;
using WarhawkBridge.Models;

namespace WarhawkBridge.Services
{
    class WebApi
    {
		private static readonly string API_BASE = "https://warhawk.thalhammer.it/api/";
		public List<ServerEntry> GetServers()
		{
			var client = new WebClient();
			var json = client.DownloadString(API_BASE + "server/"); //TODO make asynchronous
			return JsonConvert.DeserializeObject<List<ServerEntry>>(json);
		}
	}
}
