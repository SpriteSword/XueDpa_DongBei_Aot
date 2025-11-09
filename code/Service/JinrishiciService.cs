using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
// using System.Runtime.InteropServices.JavaScript;
using System.Text.Json;
using System.Text.Json.Serialization;
using XueDpa_DongBei_Aot.Helper;
using XueDpa_DongBei_Aot.Model;
using XueDpa_DongBei_Aot.Service;

namespace XueDpa_DongBei_Aot.Service;

/// <summary>
/// 依赖外部的 今日诗词 网站，如果网站名不是 jinrishici，这样挤成一坨的名字鬼才用。 </summary>
public class JinrishiciService(
	IAlertService alert_service,
	IPrfrncStorage prfrnc_storage,
	IPoetryStorage poetry_storage,
	string domain_name = "v2.jinrishici.com"
	) : ITodayPoetryService
{
	private const string _server_ = "今日诗词服务器";
	readonly IAlertService _alert_service_ = alert_service;
	readonly IPoetryStorage _poetry_storage_ = poetry_storage;
	readonly IPrfrncStorage _prfrnc_storage_ = prfrnc_storage; //  键值存储。将诗词的Token存到本地文件。+++++存Token有什么用？？？？？
	public static readonly string _jinrishici_token_key_ = $"{nameof(JinrishiciService)}.Token";

	private readonly string domain_name = domain_name;
	string token = string.Empty;

	public async Task<string?> GetTokenAsync()
	{
		if (!String.IsNullOrWhiteSpace(token))
		{
			return token;
		}

		token = _prfrnc_storage_.Get(_jinrishici_token_key_, String.Empty);
		if (!String.IsNullOrWhiteSpace(token))
		{
			return token;
		}

		using HttpClient http_client = new();
		HttpResponseMessage response;
		//+++++  为什么不能 try(HttpResponseMessage response)？你 if(xx is Mouse m) 中，m 的作用域都能 if 外面！！！
		try
		{
			response = await http_client.GetAsync($"https://{domain_name}/token");
			response.EnsureSuccessStatusCode(); //  保证连上服务器并获取到数据。防止 404 等错误。
		}
		catch (HttpRequestException e)
		{
			//  不用一层层往上返
			_alert_service_.Alert(
			  ErrorMessageHelper._http_client_error_title_,
			  ErrorMessageHelper.GetHTTPClientError(_server_, e.Message));

			return token;
		}

		string json = await response.Content.ReadAsStringAsync();

		//  反序列化
		// var jinrishici_token = JsonSerializer.Deserialize<JinrishiciToken>(
		// 	json,
		// 	new JsonSerializerOptions { PropertyNameCaseInsensitive = true }); //  大小写不敏感。+++++其实真有必要拘泥于风格规范吗？难道不是一致性更重要？
		try
		{
			JinrishiciToken jinrishici_token = JsonSerializer.Deserialize(json, SourceGnrtnContextJinrishici.Default.JinrishiciToken)
			?? throw new JsonException();

			token = jinrishici_token.Data;
		}
		catch (Exception e)
		{
			_alert_service_.Alert(ErrorMessageHelper._json_dsrlztn_error_title_, ErrorMessageHelper.GetJsonDsrlztnError(_server_, e.Message));

			return token;
		}

		_prfrnc_storage_.Set(_jinrishici_token_key_, token);
		return token;
	}

	public async Task<TodayPoetry> GetTodayPoetryAsync()
	{
		string? token = await GetTokenAsync();
		if (String.IsNullOrWhiteSpace(token))
		{
			//  从数据库随机取数据
			return await GetRandomPoetryAsync();
		}

		using HttpClient http_client = new(); //  原来还要手动释放资源的啊？
		http_client.DefaultRequestHeaders.Add("X-User-Token", $"{token}");

		HttpResponseMessage response;
		try
		{
			response = await http_client.GetAsync($"https://{domain_name}/sentence");
			_ = response.EnsureSuccessStatusCode();    //  保证没有400、500等错误

			// throw new Exception("test alert!");		//  测试弹窗
		}
		catch (Exception e)
		{
			_alert_service_.Alert(
				ErrorMessageHelper._http_client_error_title_,
				ErrorMessageHelper.GetHTTPClientError(_server_, e.Message));

			return await GetRandomPoetryAsync();
		}

		string json = await response.Content.ReadAsStringAsync();
		JinrishiciSentence j_sentence;
		try //  防止服务器抽风，数据错误
		{
			// j_sentence = JsonSerializer.Deserialize<JinrishiciSentence>(json) ?? throw new JsonException();
			j_sentence = JsonSerializer.Deserialize(json, SourceGnrtnContextJinrishici.Default.JinrishiciSentence)
				?? throw new JsonException();
		}
		catch (Exception e)
		{
			_alert_service_.Alert(
			  ErrorMessageHelper._json_dsrlztn_error_title_,
			  ErrorMessageHelper.GetJsonDsrlztnError(_server_, e.Message));

			return await GetRandomPoetryAsync();
		}

		try // 防止缺失数据
		{
			return new TodayPoetry()
			{
				Snippet = j_sentence.Data?.Content ?? throw new JsonException(),
				Name = j_sentence.Data.Origin?.title ?? throw new JsonException(),
				Dynasty = j_sentence.Data.Origin.dynasty ?? throw new JsonException(),
				Author = j_sentence.Data.Origin.author ?? throw new JsonException(),
				Content = string.Join("\n", j_sentence.Data.Origin.content ?? throw new JsonException()),
				Source = TodayPoetrySource.Jinrishici,
			};
		}
		catch (Exception e)
		{
			_alert_service_.Alert(
			  ErrorMessageHelper._json_dsrlztn_error_title_,
			  ErrorMessageHelper.GetJsonDsrlztnError(_server_, e.Message));

			return await GetRandomPoetryAsync();
		}
	}

	/// <summary>
	/// 在本地数据库随机获取诗词。 </summary>
	public async Task<TodayPoetry> GetRandomPoetryAsync()//++++++++++++似乎有问题！！测试不通过
	{
		var poetries = await _poetry_storage_.GetPoetriesAsync(
			QueryHelper._where_dflt_,
			new Random().Next(PoetryStorage._num_poetry_), 1);

		Poetry poetry = poetries.First();
		return new TodayPoetry()
		{
			Snippet = poetry.Snippet,
			Name = poetry.Name,
			Dynasty = poetry.Dynasty,
			Author = poetry.Author,
			Content = poetry.Content,
			Source = TodayPoetrySource.Local
		};
	}
}

///
///
///
public class JinrishiciToken//++++++什么垃圾？居然一定要 getset 才行？？不能直接解析进变量？？
{
	[JsonPropertyName("status")] public required string Status { get; set; }
	[JsonPropertyName("data")] public required string Data { get; set; }
}


//
public class JinrishiciSentence
{
	[JsonPropertyName("data")] public JinrishiciData Data { get; set; } = new();
}

//
public class JinrishiciData
{
	[JsonPropertyName("content")] public string Content { get; set; } = string.Empty;
	[JsonPropertyName("origin")] public JinrishiciOrigin Origin { get; set; } = new();
}

[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:命名样式", Justification = "需与外部 API 一致")]
public class JinrishiciOrigin
{
	public string title { get; set; } = string.Empty;
	public string dynasty { get; set; } = string.Empty;
	public string author { get; set; } = string.Empty;
	public List<string> content { get; set; } = [];
}



//  Source Generation Context 源生成器上下文，不用反射来反序列化
[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(JinrishiciToken))]
[JsonSerializable(typeof(JinrishiciSentence))]
[JsonSerializable(typeof(JinrishiciData))]
[JsonSerializable(typeof(JinrishiciOrigin))]
internal partial class SourceGnrtnContextJinrishici : JsonSerializerContext
{
}