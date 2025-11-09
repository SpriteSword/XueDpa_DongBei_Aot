using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using XueDpa_DongBei_Aot.Helper;
using XueDpa_DongBei_Aot.Model;

namespace XueDpa_DongBei_Aot.Service;

/// <summary>
/// 负责与 bing 服务器连接，检查图片更新并下载今日图片。 </summary>
public class BingImageService : ITodayImageService
{
	const string _server_ = "Bing 每日图片服务器";

	private readonly ITodayImageStorage _today_image_storage_;
	readonly IAlertService _alert_service_;

	private static readonly HttpClient _http_client_ = new();

	//
	public BingImageService(ITodayImageStorage today_image_storage, IAlertService alert_service)
	{
		_today_image_storage_ = today_image_storage;
		_alert_service_ = alert_service;
	}


	public async Task<TodayImage> GetTodayImageAsync() =>
		await _today_image_storage_.GetTodayImageAsync(true);

	/// <summary>
	/// 检查更新。 </summary>
	/// <returns></returns>
	/// <exception cref="JsonException"></exception>
	public async Task<TodayImageServiceCheckUpdateResult> CheckUpdateAsync()
	{
		TodayImage today_image = await _today_image_storage_.GetTodayImageAsync(false);
		if (today_image.ExpiresAt > DateTime.Now)
		{
			return new TodayImageServiceCheckUpdateResult { HasUpdate = false };
		}

		HttpResponseMessage response;
		try
		{
			response = await _http_client_.GetAsync("https://www.bing.com/HPImageArchive.aspx?format=js&idx=0&n=1");
			_ = response.EnsureSuccessStatusCode(); //  保证连上服务器并获取到数据。防止 404 等错误。
		}
		catch (Exception e)
		{
			_alert_service_.Alert(ErrorMessageHelper._http_client_error_title_,
				ErrorMessageHelper.GetHTTPClientError(_server_, e.Message));
			return new() { HasUpdate = false };
		}

		string json = await response.Content.ReadAsStringAsync();
		string bing_image_url;

		try
		{
			// BingImageOfTheDayImage bing_image = JsonSerializer.Deserialize<BingImageOfTheDay>(json,
			// 		new JsonSerializerOptions { PropertyNameCaseInsensitive = true })?.Images
			// 	?.FirstOrDefault() ?? throw new JsonException();
			BingImageOfTheDayImage bing_image = JsonSerializer
				.Deserialize(json, SourceGnrtnContextBingImage.Default.BingImageOfTheDay)?.Images
				?.FirstOrDefault() ?? throw new JsonException();

			DateTime bing_img_start_date = DateTime.ParseExact(bing_image.FullStartDate ?? throw new JsonException(),
				"yyyyMMddHHmm", CultureInfo.InvariantCulture);

			DateTime today_img_start_date = DateTime.ParseExact(today_image.FullStartDate,
				"yyyyMMddHHmm", CultureInfo.InvariantCulture);

			if (bing_img_start_date <= today_img_start_date)
			{
				today_image.ExpiresAt = DateTime.Now.AddHours(2);
				await _today_image_storage_.SaveTodayImageAsync(today_image, true); //++++false
				return new() { HasUpdate = false };
			}

			today_image = new TodayImage
			{
				FullStartDate = bing_image.FullStartDate,
				ExpiresAt = bing_img_start_date.AddDays(1),
				Copyright = bing_image.Copyright ?? throw new JsonException(),
				CopyrightLink = bing_image.CopyrightLink ?? throw new JsonException(),
			};

			bing_image_url = bing_image.Url ?? throw new JsonException();
		}
		catch (Exception e)
		{
			_alert_service_.Alert(ErrorMessageHelper._json_dsrlztn_error_title_,
				ErrorMessageHelper.GetJsonDsrlztnError(_server_, e.Message));
			return new() { HasUpdate = false };
		}

		//  取出图片 url 后
		try
		{
			response = await _http_client_.GetAsync("https://bing.com" + bing_image_url);
			_ = response.EnsureSuccessStatusCode();
		}
		catch (Exception e)
		{
			_alert_service_.Alert(ErrorMessageHelper._http_client_error_title_,
				ErrorMessageHelper.GetHTTPClientError(_server_, e.Message));
			return new() { HasUpdate = false };
		}

		today_image.ImageBytes = await response.Content.ReadAsByteArrayAsync();
		await _today_image_storage_.SaveTodayImageAsync(today_image, false);
		return new() { HasUpdate = true, TodayImage = today_image };
	}
}

//
class BingImageOfTheDay
{
	[JsonPropertyName("images")] public required List<BingImageOfTheDayImage> Images { get; set; }
}

//
class BingImageOfTheDayImage
{
	[JsonPropertyName("startdate")] public string? StartDate { get; set; }
	[JsonPropertyName("fullstartdate")] public string? FullStartDate { get; set; }
	[JsonPropertyName("enddate")] public string? EndDate { get; set; }
	[JsonPropertyName("url")] public string? Url { get; set; }
	[JsonPropertyName("copyright")] public string? Copyright { get; set; }
	[JsonPropertyName("copyrightlink")] public string? CopyrightLink { get; set; }
}

/*  格式
{
  "images" : [ {
    "startdate" : "20251005",
    "fullstartdate" : "202510051600",
    "enddate" : "20251006",
    "url" : "/th?id=OHR.AnshunBridge_ZH-CN8392458102_1920x1080.jpg&rf=LaDigue_1920x1080.jpg&pid=hp",
    "urlbase" : "/th?id=OHR.AnshunBridge_ZH-CN8392458102",
    "copyright" : "安顺桥中秋灯展，成都，中国 (© Philippe LEJEANVRE/Getty Images)",
    "copyrightlink" : "https://www.bing.com/search?q=%E4%B8%AD%E7%A7%8B%E8%8A%82&form=hpcapt&mkt=zh-cn",
    "title" : "千里共婵娟",
    "quiz" : "/search?q=Bing+homepage+quiz&filters=WQOskey:%22HPQuiz_20251005_AnshunBridge%22&FORM=HPQUIZ",
    "wp" : true,
    "hsh" : "e276da4ffe4dd53c3ee48f4ce4386645",
    "drk" : 1,
    "top" : 1,
    "bot" : 1,
    "hs" : [ ]
  } ],
  "tooltips" : {
    "loading" : "正在加载...",
    "previous" : "上一个图像",
    "next" : "下一个图像",
    "walle" : "此图片不能下载用作壁纸。",
    "walls" : "下载今日美图。仅限用作桌面壁纸。"
  }
}
*/

//  Source Generation Context
[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(BingImageOfTheDay))]
[JsonSerializable(typeof(BingImageOfTheDayImage))]
internal partial class SourceGnrtnContextBingImage : JsonSerializerContext
{
}