using XueDpa_DongBei_Aot.Model;

namespace XueDpa_DongBei_Aot.Service;

public interface ITodayPoetryService
{
	Task<TodayPoetry> GetTodayPoetryAsync();
}


public static class TodayPoetrySource
{
	public const string Jinrishici = nameof(Jinrishici);
	public const string Local = nameof(Local);
}