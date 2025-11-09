using XueDpa_DongBei_Aot.Service;
using XueDpa_DongBei_Aot.Service.Navigation;

namespace XueDpa_DongBei_Aot.ViewModels;

/// <summary>
/// 只为了导航用。 </summary>
public class MainWindowViewModel : ViewModelBase
{
	readonly IPoetryStorage _poetry_storage_;
	readonly IFavoriteStorage _favorite_storage_;
	readonly IRootNvgtnService _root_nvgtn_service_;
	readonly IMenuNvgtnService _menu_nvgtn_service_;

	private ViewModelBase? content;


	/// <summary>
	/// 一给这个赋值，就直接能导航了。 </summary>
	public ViewModelBase? Content
	{
		get => content;
		set => SetProperty(ref content, value);
	}

	// public ICommand OnIntlzdCmnd { get; }


	public MainWindowViewModel(IPoetryStorage poetry_storage,
		IFavoriteStorage favorite_storage,
		IRootNvgtnService root_nvgtn_service,
		IMenuNvgtnService menu_nvgtn_service)

	{
		Console.WriteLine("I am MainWindowViewModel");
		_poetry_storage_ = poetry_storage;
		_favorite_storage_ = favorite_storage;
		_root_nvgtn_service_ = root_nvgtn_service;
		_menu_nvgtn_service_ = menu_nvgtn_service;

		// OnIntlzdCmnd = new RelayCommand(OnIntlzd);
	}

	//  0.
	public string Greeting { get; } = "MainWindowViewModel: Welcome to Avalonia!";


	public void OnIntlzd()
	{
		//  1
		// _root_nvgtn_service_.NavigateTo(RootNvgtnConstant.MainView);

		//  2 任何一个没初始化就跳转去初始化页面。
		if (!_poetry_storage_.IsIntlzd || !_favorite_storage_.IsIntlzd)
		{
			_root_nvgtn_service_.NavigateTo(RootNvgtnConstant.IntlztnView);
		}
		else
		{
			_root_nvgtn_service_.NavigateTo(RootNvgtnConstant.MainView);
			_menu_nvgtn_service_.NavigateTo(MenuNvgtnConstant.TodayView);
		}
	}
}