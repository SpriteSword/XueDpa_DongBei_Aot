using XueDpa_DongBei_Aot.Helper;

namespace XueDpa_DongBei_Aot.Service;

public class FilePrfrncStorage : IPrfrncStorage
{
	public void Set(string key, int value) =>
		Set(key, value.ToString());

	public int Get(string key, int default_value) =>
		int.TryParse(Get(key, String.Empty), out int value) ? value : default_value;


	public void Set(string key, string value)
	{
		string path = PathHelper.GetLocalFilePath(key);
		File.WriteAllText(path, value);
	}

	public string Get(string key, string default_value)
	{
		string path = PathHelper.GetLocalFilePath(key);
		return File.Exists(path) ? File.ReadAllText(path) : default_value;
	}

	public void Set(string key, DateTime value) =>
		Set(key, value.ToString());

	public DateTime Get(string key, DateTime default_value) =>
		DateTime.TryParse(Get(key, String.Empty), out var value) ? value : default_value;
}