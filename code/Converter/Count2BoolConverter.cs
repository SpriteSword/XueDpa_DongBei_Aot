using System.Globalization;
using Avalonia.Data.Converters;

namespace XueDpa_DongBei_Aot.Converter;

//
public class Count2BoolConverter : IValueConverter
{
	/// <summary>
	///  </summary>
	/// <param name="value"></param> <param name="target_type"></param> <param name="param">阈值</param> <param name="culture"></param>
	/// <returns></returns>
	public object? Convert(object? value, Type target_type, object? param, CultureInfo culture)
	{
		return value is int count && param is string condition_str &&
				 int.TryParse(condition_str, out int condition)
			? count > condition
			: null;
	}

	public object? ConvertBack(object? value, Type target_type, object? param, CultureInfo culture)
	{
		throw new InvalidOperationException();
	}
}