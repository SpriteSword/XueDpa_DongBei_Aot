using XueDpa_DongBei_Aot.Model;

namespace XueDpa_DongBei_Aot.Service;

public interface ITodayImageStorage
{
	/// <summary>
	///  </summary>
	/// <param name="is_including_img_stream">是否包含（读取）图片的流数据</param>
	/// <returns></returns>
	Task<TodayImage> GetTodayImageAsync(bool is_including_img_stream);

	/// <summary>
	///  </summary>
	/// <param name="image"></param><param name="is_saving_expires_at_only">是否只存 过期时间</param>
	/// <returns></returns>
	Task SaveTodayImageAsync(TodayImage image, bool is_saving_expires_at_only);
}