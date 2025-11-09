using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;

namespace XueDpa_DongBei_Aot.Model;

/// <summary>
/// </summary>
public class Favorite : ObservableObject
{
	//  收藏数据与主数据分开存。只存 Id 且保证 Id 唯一。
	//  主键
	[PrimaryKey] public int PoetryId { get; set; }
	bool is_favorite;


	public virtual bool IsFavorite
	{
		get => is_favorite;
		set => SetProperty(ref is_favorite, value);
	}

	public Int64 Timestamp { get; set; }
}