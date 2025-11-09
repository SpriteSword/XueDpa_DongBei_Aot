using Avalonia.Threading;
using Ursa.Controls;

namespace XueDpa_DongBei_Aot.Service;

public class AlertService : IAlertService
{
	public void Alert(string title, string message)
	{
		//  要在 UI 线程弹窗！！
		Dispatcher.UIThread.Post(async void () =>
			await MessageBox.ShowAsync(message, title));
	}
}