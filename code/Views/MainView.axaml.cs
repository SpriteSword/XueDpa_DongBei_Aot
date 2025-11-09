using Avalonia.Controls;
using Avalonia.Input;

namespace XueDpa_DongBei_Aot.Views;


/// <summary>
/// 有导航的，窗口下的最大的区域，不同于 MainWindow，MainWindow 是窗口。 </summary>
public partial class MainView : UserControl
{
	public MainView()
	{
		InitializeComponent();
	}

	//+++++ 居然要准确写是 TappedEventArgs 才行？
	public void OnMenuTapped(object? sender, TappedEventArgs eventArgs)
	{
		ServiceLocator.MainViewModel.OnMenuTapped();
	}
}