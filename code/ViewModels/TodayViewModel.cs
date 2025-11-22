using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using XueDpa_DongBei_Aot.Model;
using XueDpa_DongBei_Aot.Service;
using XueDpa_DongBei_Aot.Service.Navigation;

namespace XueDpa_DongBei_Aot.ViewModels;

/// <summary>
/// 今日诗词、今日图片的 ViewModel。不包含导航栏。
/// </summary>
public class TodayViewModel : ViewModelBase
{
	readonly ITodayPoetryService _today_poetry_service_;
	readonly ITodayImageService _image_service_;
	private readonly IContentNvgtnService _content_nvgtn_service_;
	readonly IAlertService _alert_service_;


	TodayPoetry? today_poetry;
	private bool is_loading;

	TodayImage? today_image;


	public bool IsLoading
	{
		get => is_loading;
		private set => SetProperty(ref is_loading, value);
	}

	public TodayPoetry? TodayPoetry
	{
		get => today_poetry;
		set => SetProperty(ref today_poetry, value); //  能触发事件
	}

	public TodayImage? TodayImage
	{
		get => today_image;
		set => SetProperty(ref today_image, value);
	}


	// public ICommand OnIntlzdCmnd { get; }

	public ICommand ShowDetailCmnd { get; }


	public TodayViewModel(ITodayPoetryService today_poetry_service,
		IAlertService alert_service,
		ITodayImageService today_image_service,
		IContentNvgtnService content_nvgtn_service)
	{
		_today_poetry_service_ = today_poetry_service;
		_image_service_ = today_image_service;
		_content_nvgtn_service_ = content_nvgtn_service;

		_alert_service_ = alert_service; // NOTE: 弹窗实验


		// OnIntlzdCmnd = new RelayCommand(OnIntlzd);
		ShowDetailCmnd = new RelayCommand(ShowDetail);
	}


	// public Task OnIntlzdAsync()变为：
	public void OnIntlzd()
	{
		//  弹窗实验。不会等待弹窗关闭的！
		// _alert_service_.Alert("Pop!", "TodayViewModel: Pop! Test UI thread Pop");


		//  多线程同时开始，诗词与图片不干扰。弹窗要在 UI 线程，之前弹不出来。
		// ???: 用协程会如何??????
		_ = Task.Run(async () =>
		{
			IsLoading = true;
			// await Task.Delay(2000);		//  测试用

			TodayPoetry = await _today_poetry_service_.GetTodayPoetryAsync();
			IsLoading = false;
		});

		_ = Task.Run(async () =>
		{
			TodayImage = await _image_service_.GetTodayImageAsync();

			await Task.Delay(2000); //  测试用

			TodayImageServiceCheckUpdateResult update_result = await _image_service_.CheckUpdateAsync();

			if (update_result.HasUpdate) // ???: 为什么都是用程序自带的图，有时 HasUpdate 为 true,有时为 false？每次都图片都成功下载了在本地
			{
				TodayImage = update_result.TodayImage; // ???: 为什么更新了不替换旧的图？？？烦，另一个线程的打不了断点。为什么结果有时
			}
		});
	}


	void ShowDetail()
	{
		_content_nvgtn_service_.NavigateTo(ContentNvgtnConstant.TodayDetailView, TodayPoetry);
	}
}