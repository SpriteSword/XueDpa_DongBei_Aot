namespace XueDpa_DongBei_Aot.Service.Navigation;

public interface IMenuNvgtnService
{
	void NavigateTo(string view);
}

public static class MenuNvgtnConstant
{
	public const string TodayView = nameof(TodayView);
	public const string QueryView = nameof(QueryView);
	public const string FavoriteView = nameof(FavoriteView);
}