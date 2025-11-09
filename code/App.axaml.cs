using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using XueDpa_DongBei_Aot.Views;

namespace XueDpa_DongBei_Aot;


public partial class App : Application
{
	public override void Initialize()
	{
		AvaloniaXamlLoader.Load(this);
	}

	public override void OnFrameworkInitializationCompleted()
	{
		// 创建并配置 ServiceLocator
		// var serviceLocator = new ServiceLocator();

		// 注册到应用资源
		// Application.Current.Resources["ServiceLocator"] = ServiceLocator.Current;

		// 直接创建主窗口并设置 DataContext
		// var mainWindow = new MainWindow
		// {
		// 	DataContext = serviceLocator.MainWindowViewModel
		// };

		// 		#if DEBUG
		//     // 按 F12 打开开发者工具。不写也能按出。
		//     this.AttachDevTools();
		// #endif


		if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
		{
			// 避免 Avalonia 和 CommunityToolkit 的重复验证。
			// More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
			DisableAvaloniaDataAnnotationValidation();
			desktop.MainWindow = new MainWindowView
			{
				// DataContext = new MainWindowViewModel(),		//  我们自己改了Binding
				// DataContext = ServiceLocator.Current.MainWindowViewModel
			};

			// ServiceLocator serviceLocator = new();		//  测试导航
		}

		base.OnFrameworkInitializationCompleted();
	}

	private void DisableAvaloniaDataAnnotationValidation()
	{
		// Get an array of plugins to remove
		DataAnnotationsValidationPlugin[] dataValidationPluginsToRemove =
			BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

		// remove each entry found
		foreach (var plugin in dataValidationPluginsToRemove)
		{
			BindingPlugins.DataValidators.Remove(plugin);
		}
	}
}