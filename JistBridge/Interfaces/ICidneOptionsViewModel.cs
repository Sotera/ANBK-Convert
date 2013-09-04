namespace JistBridge.Interfaces {
	internal interface ICidneOptionsViewModel {
		string QueueReportUrl { get;  }
		string GetMetadataSchemasUrl { get;  }
		string ValidateUserUrl { get;  }
		string GetReportUrl { get;  }
		string SaveReportUrl { get;  }
		int GetReportPollDelayMS { get;  }
		bool EnableGetReportPolling { get;  }
	}
}