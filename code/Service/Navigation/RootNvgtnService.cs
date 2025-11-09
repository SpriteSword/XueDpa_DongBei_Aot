using Avalonia.Controls;
using XueDpa_DongBei_Aot.Service.Navigation;


namespace XueDpa_DongBei_Aot.Service;

//
public class RootNvgtnService : IRootNvgtnService
{
	//  测试用
	// IMenuNvgtnService menu_nvgtn_service;
	//
	// public RootNvgtnService(IMenuNvgtnService menu_nvgtn_service)
	// {
	// 	this.menu_nvgtn_service = menu_nvgtn_service;
	// }
	//  测试用


	public void NavigateTo(string view)
	{
		//  课程讲课顺序的测试

		// 落后，诶，不对，你业务上就是这样1个个页面都有规定的，我只在这一个地方写这种转换也无不可。
		// if (view == "Result")
		// {
		// 	ServiceLocator.Current.MainWindowViewModel.Content = ServiceLocator.Current.ResultViewModel;
		//

		//  测试用
		// if (view == nameof(TodayViewModel))
		// {
		// 	ServiceLocator.Current.MainWindowViewModel.Content = ServiceLocator.Current.TodayViewModel;
		// }

		// if (view == RootNvgtnConstant.MainView)
		// {
		// 	ServiceLocator.Current.MainWindowViewModel.Content = ServiceLocator.Current.MainViewModel;

		//	//!!!!  测试用！
		//	// ServiceLocator.Current.MainViewModel.Content = ServiceLocator.Current.TodayViewModel;

		//	// ServiceLocator.Current.MainViewModel.PushContent(ServiceLocator.Current.TodayViewModel);
		//	// ServiceLocator.Current.MainViewModel.PushContent(ServiceLocator.Current.TodayViewModel);		//  测试用。因为所有都是单例，所以是把同一个对象的引用加进列表里

		//	//  测试用
		//	// menu_nvgtn_service.NavigateTo(MenuNvgtnConstant.TodayView);
		// }

		ServiceLocator.MainWindowViewModel.Content = view switch
		{
			RootNvgtnConstant.IntlztnView => ServiceLocator.IntlztnViewModel,
			RootNvgtnConstant.MainView => ServiceLocator.MainViewModel,
			_ => throw new Exception("Unknown view")
		};
	}

}