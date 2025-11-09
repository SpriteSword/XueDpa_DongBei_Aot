using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Jab;
using XueDpa_DongBei_Aot.ViewModels;
using XueDpa_DongBei_Aot.Views;

namespace XueDpa_DongBei_Aot;


[ServiceProvider]
[Scoped<MainWindowView>]
[Scoped<IntlztnView>]
[Scoped<MainView>]
[Scoped<DetailView>]
[Scoped<FavoriteView>]
[Scoped<QueryView>]
[Scoped<TodayDetailView>]
[Scoped<TodayView>]
[Scoped<ResultView>]
public partial class ViewLocator : IDataTemplate
{
	private static ViewLocator? current;


	public static ViewLocator Current
	{
		get
		{
			if (current != null) { return current; }

			if (Application.Current != null &&
				 Application.Current.TryGetResource(nameof(ViewLocator), null, out var resource) &&
				 resource is ViewLocator locator)
			{
				return current = locator;
			}

			throw new Exception(
				"Can not get ViewLocator. 理论上不应出现这种情况。需要在 App.axaml 里定义 <Application.Resources> 包含 ViewLocator。");
		}
	}


	// public Control? Build(object? param)
	// {
	// 	if (param is null) { return null; }
	//
	// 	var name = param.GetType().FullName!.Replace("ViewModel", "View", StringComparison.Ordinal);
	// 	var type = Type.GetType(name);
	//
	// 	if (type != null)
	// 	{
	// 		return (Control)Activator.CreateInstance(type)!;
	// 	}
	//
	// 	return new TextBlock { Text = "Not Found: " + name };
	// }


	public Control? Build(object? param)
	{
		return param is not ViewModelBase vmb ? null : Build(vmb);
	}

	public static Control Build(ViewModelBase vmb)
	{
		return vmb switch //++++++++++ axaml 也要写这么多类型，真的很烦！
		{
			//++++  每次都 new 是不是不太对？原本写的 Activator.CreateInstance 也相当于 new，每次进入呕同一个页面难道都会实例化一个新的 View？
			//  看需求吧
			MainWindowViewModel => MainWindowView,
			IntlztnViewModel => IntlztnView,
			MainViewModel => MainView,

			DetailViewModel => DetailView,
			FavoriteViewModel => FavoriteView,
			QueryViewModel => QueryView,
			TodayDetailViewModel => TodayDetailView,     //  试一下。++++++单例也会被调用析构函数？
			TodayViewModel => TodayView,
			ResultViewModel => ResultView,

			_ => new TextBlock { Text = "Not Found: " + vmb }
		};

	}


	public bool Match(object? data)
	{
		return data is ViewModelBase;
	}


	//++++这样写太麻烦了，能不能用源生成器？
	private static MainWindowView MainWindowView =>
		Current.GetService<MainWindowView>();

	private static IntlztnView IntlztnView =>
		Current.GetService<IntlztnView>();

	private static MainView MainView =>
		Current.GetService<MainView>();


	private static DetailView DetailView =>
		Current.GetService<DetailView>();

	private static FavoriteView FavoriteView =>
		Current.GetService<FavoriteView>();

	private static QueryView QueryView =>
		Current.GetService<QueryView>();

	private static TodayView TodayView =>
		Current.GetService<TodayView>();

	private static TodayDetailView TodayDetailView =>
		Current.GetService<TodayDetailView>();

	private static ResultView ResultView =>
		Current.GetService<ResultView>();
}