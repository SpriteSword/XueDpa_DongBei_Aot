using Avalonia.Controls;
namespace XueDpa_DongBei_Aot.Views;


public partial class IntlztnView : UserControl
{
	public IntlztnView()
	{
		InitializeComponent();
	}


	public async void OnIntlzd(object? sender, EventArgs eventArgs)
	{
		await ServiceLocator.IntlztnViewModel.OnIntlzdAsync();
	}
}