using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace WarhawkBridge.Models
{
    class ServerEntry
    {
		[JsonProperty("id")]
		public string ID { get; set; }
		[JsonProperty("hostname")]
		public string Hostname { get; set; }
		[JsonProperty("created")]
		public DateTime Created { get; set; }
		[JsonProperty("response")]
		public string Response { get; set; }
		[JsonProperty("state")]
		public string State { get; set; }
		[JsonProperty("ping")]
		public int Ping { get; set; }
		[JsonProperty("name")]
		public string Name { get; set; }

		public bool IsOnline
		{
			get { return State == "online"; }
		}
	}
}
