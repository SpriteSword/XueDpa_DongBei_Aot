using Avalonia.Data.Converters;
using System.Globalization;

namespace XueDpa_DongBei_Aot.Converter;


public class NegativeConverter : IValueConverter
{
	public object? Convert(object? value, Type target_type, object? param, CultureInfo culture)
	{
		return value is bool b ? !b : null;
	}

	public object? ConvertBack(object? value, Type target_type, object? param, CultureInfo culture)
	{
		throw new InvalidOperationException();
	}
}