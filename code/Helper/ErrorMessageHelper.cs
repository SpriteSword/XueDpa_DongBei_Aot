namespace XueDpa_DongBei_Aot.Helper;

public class ErrorMessageHelper
{
	public const string _http_client_error_title_ = "连接错误";

	/// JSON Deserialization Error Title
	public const string _json_dsrlztn_error_title_ = "读取数据错误";
	public const string _json_dsrlztn_error_button_ = "确定";


	public static string GetHTTPClientError(string server, string message) =>
		$"与{server}连接时发生错误：\n{message}";

	public static string GetJsonDsrlztnError(string server, string message) =>
		$"从{server}读取数据时发生了错误：\n{message}";
}
// 感觉还是 GetHttpXxxx 的写法视觉效果好，断词清晰
