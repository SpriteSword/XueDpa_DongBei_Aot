using XueDpa_DongBei_Aot.Helper;
using XueDpa_DongBei_Aot.Model;

namespace XueDpa_DongBei_Aot.Service;

/// <summary>
/// 负责今日图片的存储。 </summary>
public class TodayImageStorage(IPrfrncStorage prfrnc_storage) : ITodayImageStorage
{
	readonly IPrfrncStorage _prfrnc_storage_ = prfrnc_storage; //  FilePrfrncStorage 来实现。居然直接以属性名作为文件名，来存进本地文件？

	//  也是存到本地时的文件名
	public static readonly string FullStartDateKey = nameof(TodayImageStorage) + "." + nameof(TodayImage.FullStartDate);
	public static readonly string ExpiresAtKey = nameof(TodayImageStorage) + "." + nameof(TodayImage.ExpiresAt);
	public static readonly string CopyrightKey = nameof(TodayImageStorage) + "." + nameof(TodayImage.Copyright);
	public static readonly string CopyrightLinkKey = nameof(TodayImageStorage) + "." + nameof(TodayImage.CopyrightLink);

	//  dflt default
	public const string _full_start_date_default_ = "201901010700";
	public static readonly DateTime _expires_at_default_ = new(2019, 1, 2, 7, 0, 0);
	public const string _copyright_default_ = "Salt field province vietnam work (© Quangpraha/Pixabay)";
	public const string _copyright_link_default_ = "https://pixabay.com/photos/salt-field-province-vietnam-work-3344508/";

	public const string _filename_ = "today-img.bin"; //  内置图片
	public static readonly string _today_image_path_ = PathHelper.GetLocalFilePath(_filename_);


	public async Task<TodayImage> GetTodayImageAsync(bool is_including_img_stream)
	{
		TodayImage today_image = new()
		{
			FullStartDate = _prfrnc_storage_.Get(FullStartDateKey, _full_start_date_default_),
			ExpiresAt = _prfrnc_storage_.Get(ExpiresAtKey, _expires_at_default_),
			Copyright = _prfrnc_storage_.Get(CopyrightKey, _copyright_default_),
			CopyrightLink = _prfrnc_storage_.Get(CopyrightLinkKey, _copyright_link_default_),
		};

		//  如果不存在,就把内置的图片复制过去
		if (!File.Exists(_today_image_path_))
		{
			await using FileStream image_asset_file_stream = new FileStream(_today_image_path_, FileMode.Create) ??
				throw new NullReferenceException("Null file stream");

			await using Stream image_asset_stream =
				typeof(TodayImageStorage).Assembly.GetManifestResourceStream(_filename_) ??
				throw new NullReferenceException("Null ManifestResourceStream");

			await image_asset_stream.CopyToAsync(image_asset_file_stream);
		}

		//  不包含图片流
		if (!is_including_img_stream)
		{
			return today_image;
		}

		//  包含图片流的继续执行。
		MemoryStream image_memory_stream = new();
		await using FileStream image_file_stream = new(_today_image_path_, FileMode.Open);
		await image_file_stream.CopyToAsync(image_memory_stream);
		today_image.ImageBytes = image_memory_stream.ToArray(); //  因为内存流有 ToArray()，方便。
		return today_image;
	}

	public async Task SaveTodayImageAsync(TodayImage image, bool is_saving_expires_at_only)
	{
		_prfrnc_storage_.Set(ExpiresAtKey, image.ExpiresAt);

		if (is_saving_expires_at_only)
		{
			return;
		}

		if (image.ImageBytes == null)
		{
			throw new ArgumentException("ImageBytes is null", nameof(image));
		}

		_prfrnc_storage_.Set(FullStartDateKey, image.FullStartDate);
		_prfrnc_storage_.Set(CopyrightKey, image.Copyright);
		_prfrnc_storage_.Set(CopyrightLinkKey, image.CopyrightLink);

		await using FileStream image_file_stream = new(_today_image_path_, FileMode.Create);
		/*
		也可以:
		var m = new MemoryStream(image.ImageBytes);
		await m.CopyToAsync(image_file_stream);
		*/
		// await image_file_stream.WriteAsync(image.ImageBytes, 0, image.ImageBytes.Length);		//  这个被弃用了？
		await image_file_stream.WriteAsync(image.ImageBytes.AsMemory(0, image.ImageBytes.Length));
	}
}