using Avalonia.Controls;
namespace XueDpa_DongBei_Aot.Views;


public partial class MainWindowView : Window
{
	public MainWindowView()
	{
		InitializeComponent();
	}


	// ???: 垃圾 xaml 设计，不能绑到另一个类上，也不能绑静态函数，甚至写全命名空间反而报错，垃圾东西！！！！！！对声明式这么推崇，这等简单问题不改善。
	public void OnIntlzd(object? sender, EventArgs eventArgs)
	{
		Console.WriteLine("MainWindowView: OnInit");

		// await ServiceLocator.Current.ResultViewModel.OnIntlzdAsync();
		// ServiceLocator.Current.TodayViewModel.OnIntlzd();

		ServiceLocator.MainWindowViewModel.OnIntlzd();
	}
}