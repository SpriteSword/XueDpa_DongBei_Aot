using XueDpa_DongBei_Aot.Model;

namespace XueDpa_DongBei_Aot.Service;

//  2阶段加载,先加载默认的,联网更新后再加载今日图片
public interface ITodayImageService
{
	Task<TodayImage> GetTodayImageAsync();
	Task<TodayImageServiceCheckUpdateResult> CheckUpdateAsync();
}

//
public class TodayImageServiceCheckUpdateResult
{
	public bool HasUpdate { get; set; }
	public TodayImage TodayImage { get; set; } = new();
}