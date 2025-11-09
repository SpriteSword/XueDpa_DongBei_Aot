using CommunityToolkit.Mvvm.Messaging;
using SQLite;
using XueDpa_DongBei_Aot.Helper;
using XueDpa_DongBei_Aot.Model;

namespace XueDpa_DongBei_Aot.Service;

public class FavoriteStorage(IPrfrncStorage prfrnc_storage) : IFavoriteStorage
{
	public const string _db_name_ = "favorite.sqlite3";
	public static readonly string _poetry_db_path_ = PathHelper.GetLocalFilePath(_db_name_);
	readonly IPrfrncStorage _prfrnc_storage_ = prfrnc_storage;

	SQLiteAsyncConnection? connection;


	SQLiteAsyncConnection Connection =>
		connection ??= new SQLiteAsyncConnection(_poetry_db_path_);

	public bool IsIntlzd =>
		_prfrnc_storage_.Get(FavoriteStorageConstant._version_key_, default(int)) ==
		FavoriteStorageConstant._version_;

	// public event EventHandler<FavoriteStorageUpdatedEventArgs>? Updated;
	public event EventHandler<Favorite>? Updated;		//  后面改用消息，这个事件不会发出


	public async Task InitializeAsync()
	{
		//  数据库是创建的，不是内置的复制的，因为本来就没有数据嘛
		_ = await Connection.CreateTableAsync<Favorite>();
		_prfrnc_storage_.Set(FavoriteStorageConstant._version_key_, FavoriteStorageConstant._version_);
	}

	public async Task<Favorite> GetFavoriteAsync(int poetry_id)
	{
		return await Connection.Table<Favorite>().FirstOrDefaultAsync(p => p.PoetryId == poetry_id);
	}

	public async Task<IEnumerable<Favorite>> GetFavoritesAsync()
	{
		return await Connection.Table<Favorite>()
			.Where(p => p.IsFavorite)
			.OrderByDescending(p => p.Timestamp)
			.ToListAsync();
	}

	public async Task SaveFavoriteAsync(Favorite favorite)
	{
		favorite.Timestamp = DateTimeOffset.Now.Ticks; //  保存 tick 不易出错
		_ = await Connection.InsertOrReplaceAsync(favorite);
		// Updated?.Invoke(this, favorite);		//
		_ = WeakReferenceMessenger.Default.Send(new FavoriteStorageUpdatedMsg(favorite));
	}

	public async Task CloseAsync()
	{
		if (connection == null) { return; }
		await connection.CloseAsync();
	}
}

/// <summary>
///  </summary>
public static class FavoriteStorageConstant
{
	public const string _version_key_ = nameof(FavoriteStorageConstant) + "." + nameof(_version_);
	public const int _version_ = 1;
}