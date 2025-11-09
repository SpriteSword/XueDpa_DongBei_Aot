using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using XueDpa_DongBei_Aot.Service;
using XueDpa_DongBei_Aot.Service.Navigation;

namespace XueDpa_DongBei_Aot.ViewModels;

/// <summary>
/// Initialization View Model，专门用来初始化数据库的。 </summary>
public class IntlztnViewModel : ViewModelBase
{
	readonly IPoetryStorage _poetry_storage_;
	readonly IFavoriteStorage _favorite_storage_;
	readonly IRootNvgtnService _root_nvgtn_service_;
	readonly IMenuNvgtnService _menu_nvgtn_service_;


	private ICommand OnIntlzdCmnd { get; }


	public IntlztnViewModel(IPoetryStorage poetry_storage,
		IFavoriteStorage favorite_storage,
		IRootNvgtnService root_nvgtn_service,
		IMenuNvgtnService menu_nvgtn_service)
	
	{
		_poetry_storage_ = poetry_storage;
		_favorite_storage_ = favorite_storage;
		_root_nvgtn_service_ = root_nvgtn_service;
		_menu_nvgtn_service_ = menu_nvgtn_service;

		OnIntlzdCmnd = new AsyncRelayCommand(OnIntlzdAsync);
	}

	public async Task OnIntlzdAsync()
	{
		if (!_poetry_storage_.IsIntlzd)
		{
			await _poetry_storage_.InitAsync();
		}

		if (!_favorite_storage_.IsIntlzd)
		{
			await _favorite_storage_.InitializeAsync();
		}

		// await Task.Delay(500);		//  测试用

		_root_nvgtn_service_.NavigateTo(RootNvgtnConstant.MainView);
		_menu_nvgtn_service_.NavigateTo(MenuNvgtnConstant.TodayView);
	}
}