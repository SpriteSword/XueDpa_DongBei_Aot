using Avalonia;
using Avalonia.Controls;
namespace XueDpa_DongBei_Aot.Views;


public partial class TodayView : UserControl
{
	public TodayView()
	{
		InitializeComponent();
	}

	~TodayView()
	{
		Console.WriteLine(nameof(TodayView) + " be GC");
	}


	public void OnIntlzd(object? sender, EventArgs eventArgs)		// ???: 这个事件会不会造成 无法GC？
	{
		ServiceLocator.TodayViewModel.OnIntlzd();
	}
}