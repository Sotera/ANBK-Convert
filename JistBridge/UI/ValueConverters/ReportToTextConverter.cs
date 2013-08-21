using System;
using System.Globalization;
using System.Windows.Data;

namespace JistBridge.UI.ValueConverters {
	public class ReportToTextConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return parameter.ToString() + value.ToString();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return null;
		}
	}
}