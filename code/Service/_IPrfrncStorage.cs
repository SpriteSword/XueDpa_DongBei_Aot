namespace XueDpa_DongBei_Aot.Service;

/// <summary>
/// preference storage 接口。偏好存储
/// </summary>
public interface IPrfrncStorage
{
	void Set(string key, int value);
	int Get(string key, int default_value);

	void Set(string key, string value);
	string Get(string key, string default_value);

	void Set(string key, DateTime value);
	DateTime Get(string key, DateTime default_value);
}