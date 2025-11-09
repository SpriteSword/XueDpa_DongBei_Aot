using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;

namespace XueDpa_DongBei_Aot.Converter;

public class ByteArrayToBitmapConverter : IValueConverter
{
	public object? Convert(object? value, Type target_type, object? prmtr, CultureInfo culture) =>
		value is byte[] bytes ? new Bitmap(new MemoryStream(bytes)) : null;

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
		throw new InvalidOperationException();
}