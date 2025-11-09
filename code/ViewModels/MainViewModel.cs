using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using XueDpa_DongBei_Aot.Service.Navigation;

namespace XueDpa_DongBei_Aot.ViewModels;

//+++++++++为什么汉堡菜单点1次后，后面就点不了了？？？准确说是点击非汉堡菜单区域导致菜单关闭后就不能再点击。
/// <summary>
/// 包含导航栏的整个视图的 ViewModel。 </summary>
public class MainViewModel : ViewModelBase
{
	private ViewModelBase? content;
	private bool is_pane_open; //  汉堡导航栏是否展开
	string title = "Daily Poetry Aval";
	MenuItem? selected_menu_item;

	private readonly IMenuNvgtnService _menu_nvgtn_service_;


	//  为了能集合更改时界面自动刷新
	public ObservableCollection<ViewModelBase> ContentStack { get; } = [];

	public ViewModelBase? Content
	{
		get => content;
		private set => SetProperty(ref content, value);
	}

	public bool IsPaneOpen
	{
		get => is_pane_open;
		private set => SetProperty(ref is_pane_open, value);
	}

	public string Title
	{
		get => title;
		private set => SetProperty(ref title, value);
	}

	public MenuItem? SelectedMenuItem
	{
		get => selected_menu_item;
		set => SetProperty(ref selected_menu_item, value);
	}

	public ICommand GoBackCmnd { get; }
	public ICommand OpenPaneCmnd { get; }
	public ICommand ClosePaneCmnd { get; }
	// public ICommand OnMenuTappedCmnd { get; }


	public MainViewModel(IMenuNvgtnService menu_nvgtn_service)
	{
		_menu_nvgtn_service_ = menu_nvgtn_service;

		GoBackCmnd = new RelayCommand(GoBack);
		OpenPaneCmnd = new RelayCommand(OpenPane);
		ClosePaneCmnd = new RelayCommand(ClosePane);
		// OnMenuTappedCmnd = new RelayCommand(OnMenuTapped);
	}

	/// <summary>
	/// 将 内容页面 压栈。 </summary>
	public void PushContent(ViewModelBase content_)
	{
		ContentStack.Insert(0, Content = content_); //+++++为什么不放最后？
	}

	public void GoBack()
	{
		if (ContentStack.Count <= 1)
		{
			return;
		}

		// ContentStack.RemoveAt(ContentStack.Count - 1);
		ContentStack.RemoveAt(0);
		Content = ContentStack[0];
	}

	//+++++++设置打开汉堡菜单时默认的选中的选项?????。
	/// <summary>
	/// 设置菜单和 内容页面。 </summary>
	/// <param name="view"></param> <param name="content_"></param>
	public void SetMenuAndContent(string view, ViewModelBase content_)
	{
		ContentStack.Clear();
		PushContent(content_);
		SelectedMenuItem = MenuItem.MenuItems.FirstOrDefault(p => p.View == view);

		if (SelectedMenuItem != null)
		{
			Title = SelectedMenuItem.Name;
		}

		IsPaneOpen = false;
	}

	void OpenPane() =>
		IsPaneOpen = true;

	void ClosePane() =>
		IsPaneOpen = false;

	public void OnMenuTapped()
	{
		if (SelectedMenuItem is null) { return; }
		_menu_nvgtn_service_.NavigateTo(SelectedMenuItem.View);     //++++++++是不是在这里解决如果随便乱点导致菜单隐藏，然后再也点不出菜单的bug？
	}
}

//   菜单的项
public class MenuItem
{
	public required string Name { get; init; }
	public required string View { get; init; }


	//+++++这个明明是当常量用却不能写死为常量，语言层面的锅！
	public static IEnumerable<MenuItem> MenuItems { get; } =
	[
		TodayView, QueryView, FavoriteView
	];


	//  不给外界new
	private MenuItem()
	{
	}

	//  为了单例
	private static MenuItem TodayView =>
		new() { Name = "今日推荐", View = MenuNvgtnConstant.TodayView };

	private static MenuItem QueryView =>
		new() { Name = "诗词搜索", View = MenuNvgtnConstant.QueryView };

	private static MenuItem FavoriteView =>
		new() { Name = "今日收藏", View = MenuNvgtnConstant.FavoriteView };
}