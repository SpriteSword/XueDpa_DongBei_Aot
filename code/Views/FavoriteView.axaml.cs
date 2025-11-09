using Avalonia.Controls;
using Avalonia.Input;
using XueDpa_DongBei_Aot.Model;

namespace XueDpa_DongBei_Aot.Views;

public partial class FavoriteView : UserControl
{
	public FavoriteView()
	{
		InitializeComponent();
	}

	public async void OnIntlzd(object? sender, EventArgs eventArgs)
	{
		await ServiceLocator.FavoriteViewModel.OnIntlzdAsync();
	}

	public void OnPoetryTapped(object? sender, TappedEventArgs eventArgs)
	{
		if (sender is not StackPanel sp) { return; }
		if (sp.Tag is not Poetry p) { return; }

		ServiceLocator.FavoriteViewModel.ShowPoetry(p);
	}
}