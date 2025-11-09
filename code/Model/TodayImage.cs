namespace XueDpa_DongBei_Aot.Model;

public class TodayImage
{
	//  缓存
	public string FullStartDate { get; set; } = String.Empty;
	public DateTime ExpiresAt { get; set; }		//  expires 过期

	//  显示
	public string Copyright { get; set; } = String.Empty;
	public string CopyrightLink { get; set; } = String.Empty;
	public byte[]? ImageBytes { get; set; }
}