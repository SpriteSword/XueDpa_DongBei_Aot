namespace XueDpa_DongBei_Aot.Service.Navigation;

//++++  这跟 IRootNvgtnService 不是一样吗？感觉接口膨胀？
public interface IContentNvgtnService
{
	void NavigateTo(string view, object? prmtr = null);
}


//++++这不就相当于基类分成纯函数接口的和纯数据的类？？
public static class ContentNvgtnConstant
{
	public const string TodayDetailView = nameof(TodayDetailView);
	public const string ResultView = nameof(ResultView);
	public const string DetailView = nameof(DetailView);
}