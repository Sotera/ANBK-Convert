using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security;

namespace JistBridge.Utilities.PInvoke {
	[SuppressUnmanagedCodeSecurity]
	internal static class NativeMethods {
		public const int HWND_BROADCAST = 0xffff;
		public static readonly int WM_SHOWME = RegisterWindowMessage("WM_SHOWME");

		[DllImport("user32")]
		public static extern bool PostMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);

		[DllImport("user32")]
		public static extern int RegisterWindowMessage(string message);

		public delegate IntPtr MessageHandler(WM uMsg, IntPtr wParam, IntPtr lParam, out bool handled);

		[DllImport("shell32.dll", EntryPoint = "CommandLineToArgvW", CharSet = CharSet.Unicode)]
		private static extern IntPtr _CommandLineToArgvW([MarshalAs(UnmanagedType.LPWStr)] string cmdLine, out int numArgs);


		[DllImport("kernel32.dll", EntryPoint = "LocalFree", SetLastError = true)]
		private static extern IntPtr _LocalFree(IntPtr hMem);


		public static string[] CommandLineToArgvW(string cmdLine) {
			var argv = IntPtr.Zero;
			try {
				var numArgs = 0;

				argv = _CommandLineToArgvW(cmdLine, out numArgs);
				if (argv == IntPtr.Zero)
					throw new Win32Exception();
				var result = new string[numArgs];

				for (var i = 0; i < numArgs; i++) {
					var currArg = Marshal.ReadIntPtr(argv, i*Marshal.SizeOf(typeof (IntPtr)));
					result[i] = Marshal.PtrToStringUni(currArg);
				}

				return result;
			}
			finally {
				var p = _LocalFree(argv);
				// Otherwise LocalFree failed.
				// Assert.AreEqual(IntPtr.Zero, p);
			}
		}
	}
}