using XueDpa_DongBei_Aot.Service.Navigation;
using XueDpa_DongBei_Aot.ViewModels;

namespace XueDpa_DongBei_Aot.Service;

public class ContentNvgtnService : IContentNvgtnService
{
	public void NavigateTo(string view, object? prmtr = null)
	{
		ViewModelBase view_model = view switch
		{
			ContentNvgtnConstant.TodayDetailView => ServiceLocator.TodayDetailViewModel,
			ContentNvgtnConstant.ResultView => ServiceLocator.ResultViewModel,
			ContentNvgtnConstant.DetailView => ServiceLocator.DetailViewModel,
			_ => throw new Exception("未知的视图")
		};

		view_model.SetParameter(prmtr);
		ServiceLocator.MainViewModel.PushContent(view_model);
		//  居然调用了页面导航的，算不算下级调用上级？？不算，因为就是整个页面换的，只是这些页面没有显示在汉堡菜单上！
	}
}