using System.Globalization;
using Avalonia.Data.Converters;
using XueDpa_DongBei_Aot.Model;

namespace XueDpa_DongBei_Aot.Converter;

public class PoetryToStringConverter : IValueConverter
{
	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
		value is Poetry p ? $"{p.Dynasty} · {p.Author}\t\t{p.Snippet}" : null;

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
		throw new NotImplementedException();
}