using Avalonia.Controls;
namespace XueDpa_DongBei_Aot.Views;


public partial class TodayDetailView : UserControl
{
	public TodayDetailView()
	{
		InitializeComponent();
	}

	~TodayDetailView()
	{
		Console.WriteLine(nameof(TodayDetailView) + " be GC");
	}
}