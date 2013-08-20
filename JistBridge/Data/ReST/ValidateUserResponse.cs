using System.Collections.Generic;

namespace JistBridge.Data.ReST {
	public class ValidateUserResponse {
		public class RoleInfo {
			public string name { get; set; }
			public string poid { get; set; }
		}

		public class UserInfo {
			public string userName { get; set; }
			public string firstName { get; set; }
			public string lastName { get; set; }
			public string poid { get; set; }
		}

		public string description { get; set; }
		public int resultCode { get; set; }
		public List<RoleInfo> roleInfo { get; set; }
		public UserInfo userInfo { get; set; }
	}
}