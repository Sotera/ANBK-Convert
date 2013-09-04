using System.Collections.Generic;
using Newtonsoft.Json;

namespace JistBridge.Data.ReST {
	public class GetMetadataSchemasResponse {
		public class CSchemas {
			public List<string> All { get; set; }
		}

		public int resultCode { get; set; }
		public string description { get; set; }

		public string schemas {
			set {
				var json = value;
				Schemas = JsonConvert.DeserializeObject<CSchemas>(json);
			}
		}

		public CSchemas Schemas;
	}
}