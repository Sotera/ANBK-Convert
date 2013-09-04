using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace JistBridge.Data.ReST {
	public class ValidateUserResponse {
		public class CRoleInfo {
			public string name { get; set; }
			public string poid { get; set; }
		}

		public class CUserInfo {
			public string userName { get; set; }
			public string firstName { get; set; }
			public string lastName { get; set; }
			public string poid { get; set; }
		}

		public int resultCode { get; set; }
		public string description { get; set; }

		public string roleInfo {
			set {
				var json = value;
				RoleInfo = JsonConvert.DeserializeObject<List<CRoleInfo>>(json);
			}
		}

		public string userInfo {
			set {
				var json = value;
				UserInfo = JsonConvert.DeserializeObject<CUserInfo>(json);
			}
		}

		public List<CRoleInfo> RoleInfo;
		public CUserInfo UserInfo;
	}
}