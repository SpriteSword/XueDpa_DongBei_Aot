using Avalonia;
using Jab;
using XueDpa_DongBei_Aot.Service;
using XueDpa_DongBei_Aot.Service.Navigation;
using XueDpa_DongBei_Aot.ViewModels;


namespace XueDpa_DongBei_Aot;

//++++ 张引说 ServiceLocator 是 view 层的，但我觉得这不是上帝般的东西吗，不是跳出3界之外吗？所有东西的依赖都要靠它来注册。
[ServiceProvider]
[Scoped<IPrfrncStorage, FilePrfrncStorage>]
[Scoped<IPoetryStorage, PoetryStorage>]
[Scoped<IAlertService, AlertService>]
[Scoped<ITodayPoetryService, JinrishiciService>]
[Scoped<IRootNvgtnService, RootNvgtnService>]
[Scoped<IMenuNvgtnService, MenuNvgtnService>]
[Scoped<ITodayImageService, BingImageService>]
[Scoped<ITodayImageStorage, TodayImageStorage>]
[Scoped<IContentNvgtnService, ContentNvgtnService>]
[Scoped<IFavoriteStorage, FavoriteStorage>]

[Scoped<MainWindowViewModel>]
[Scoped<ResultViewModel>]
[Scoped<TodayViewModel>]
[Scoped<MainViewModel>]
[Scoped<QueryViewModel>]
[Scoped<FavoriteViewModel>]
[Scoped<IntlztnViewModel>]
[Scoped<TodayDetailViewModel>]
[Scoped<DetailViewModel>]
public partial class ServiceLocator
{
	private static ServiceLocator? current;


	public static ServiceLocator Current
	{
		get
		{
			if (current != null)
			{
				return current;
			}

			//  因为 ServiceLocator 的实例是在 axaml 里面定义的，全局资源，但 C# 代码不能直接获取，所以要这样。
			//  这就是割裂感来源！！lisp 何日能光大？
			if (Application.Current != null &&
				 Application.Current.TryGetResource(nameof(ServiceLocator), null, out var resource) &&
				 resource is ServiceLocator locator)
			{
				return current = locator;
			}

			throw new Exception(
				"Can not get ServiceLocator. 理论上不应出现这种情况。需要在 App.axaml 里定义 <Application.Resources> 包含 ServiceLocator。");
		}
	}


	public ServiceLocator()
	{
		// Console.WriteLine("I am ServiceLocator.");
	}


	public static MainWindowViewModel MainWindowViewModel =>
		Current.GetService<MainWindowViewModel>();

	public static ResultViewModel ResultViewModel =>
		Current.GetService<ResultViewModel>();


	public static TodayViewModel TodayViewModel =>
		Current.GetService<TodayViewModel>();


	// //  测试导航用，临时的
	// public IRootNvgtnService RootNvgtnService =>
	// 	Current.GetService<IRootNvgtnService>();


	public static MainViewModel MainViewModel =>
		Current.GetService<MainViewModel>();

	public static QueryViewModel QueryViewModel =>
		Current.GetService<QueryViewModel>();

	public static FavoriteViewModel FavoriteViewModel =>
		Current.GetService<FavoriteViewModel>();

	public static IntlztnViewModel IntlztnViewModel =>
		Current.GetService<IntlztnViewModel>();

	public static TodayDetailViewModel TodayDetailViewModel =>
		Current.GetService<TodayDetailViewModel>();

	public static DetailViewModel DetailViewModel =>
		Current.GetService<DetailViewModel>();

}