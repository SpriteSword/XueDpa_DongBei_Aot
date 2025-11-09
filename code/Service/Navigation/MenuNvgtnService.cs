using System;
using XueDpa_DongBei_Aot.Service.Navigation;
using XueDpa_DongBei_Aot.ViewModels;

namespace XueDpa_DongBei_Aot.Service;

//
public class MenuNvgtnService : IMenuNvgtnService
{
	public void NavigateTo(string view)
	{
		ViewModelBase view_model = view switch
		{
			MenuNvgtnConstant.TodayView => ServiceLocator.TodayViewModel,
			MenuNvgtnConstant.QueryView => ServiceLocator.QueryViewModel,
			MenuNvgtnConstant.FavoriteView => ServiceLocator.FavoriteViewModel,
			_ => throw new Exception("未知的视图")
		};

		ServiceLocator.MainViewModel.SetMenuAndContent(view, view_model);
	}
}