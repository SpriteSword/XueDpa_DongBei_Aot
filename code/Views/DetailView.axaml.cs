using Avalonia.Controls;
using Avalonia.Interactivity;

namespace XueDpa_DongBei_Aot.Views;


public partial class DetailView : UserControl
{
	public DetailView()
	{
		InitializeComponent();
	}

	public async void OnLoaded(object? sender, RoutedEventArgs eventArgs)
	{
		await ServiceLocator.DetailViewModel.OnLoadedAsync();
	}
}