using System.Linq.Expressions;
using SQLite;
using XueDpa_DongBei_Aot.Helper;
using XueDpa_DongBei_Aot.Model;

namespace XueDpa_DongBei_Aot.Service;

public class PoetryStorage : IPoetryStorage
{
	public const int _num_poetry_ = 30;  //  方便测试而已

	private const string _db_name_ = "poetry.sqlite3";
	public static readonly string _poetry_db_path_ = PathHelper.GetLocalFilePath(_db_name_);
	readonly IPrfrncStorage _prfrnc_storage_; //  键值存储，用来存版本号
	private SQLiteAsyncConnection? connection;


	SQLiteAsyncConnection Connection =>
		connection ??= new SQLiteAsyncConnection(_poetry_db_path_);

	public bool IsIntlzd =>
		_prfrnc_storage_.Get(PoetryStorageConstant._version_key_, default(int)) ==
		PoetryStorageConstant._version_; //  is initialized.async


	public PoetryStorage(IPrfrncStorage prfrnc_storage)
	{
		_prfrnc_storage_ = prfrnc_storage;
	}

	/// initialize Async
	public async Task InitAsync()
	{
		await using FileStream db_file_stream = new(_poetry_db_path_, FileMode.OpenOrCreate);
		await using Stream? db_asset_stream = typeof(PoetryStorage).Assembly.GetManifestResourceStream(_db_name_)
			?? throw new FileNotFoundException("Embedded poetry database not found.", _db_name_);
		// ???: Assembly 的资源应该程序启动时就已经在内存了吧，不需要用异步吧？大资源的情况又如何？

		// ???: 是因为资源内嵌了，所以要流对流复制，不能文件到文件复制？
		await db_asset_stream.CopyToAsync(db_file_stream); //XueDpa_DongBei_Aot.poetry.sqlite3
	}

	/// <summary>
	/// 获取诗词。 </summary>
	/// <param name="id"></param>
	/// <returns></returns>
	public async Task<Poetry> GetPoetryAsync(int id) =>
		await Connection.Table<Poetry>().FirstOrDefaultAsync(p => p.Id == id);

	/// <summary>
	/// 万能查询。 </summary>
	/// <param name="where"></param><param name="skip"></param><param name="take"></param><returns></returns>
	public async Task<IList<Poetry>>
		GetPoetriesAsync(Expression<Func<Poetry, bool>> where, int skip, int take) //  性能差。async
	{
		// await Task.Delay(1000);

		return await Connection.Table<Poetry>().Where(where).Skip(skip).Take(take).ToListAsync();		// ???: 张引说会翻译成sql再执行，不知真假
	}

	//  关闭表。没有在接口中写这个，
	public async Task CloseAsync()
	{
		if (connection != null) { await connection.CloseAsync(); }     // ???: 异步函数居然不能用 .? 符？
	}


	/*
	//  解释！！
	Task Do()
	{
		Connection.Table<Poetry>().Where(p => true);
		//  where 部分二者等价，下面也是定义了个 输入 Poetry，返回 bool 的 λ 函数
		var where = Expression.Lambda<Func<Poetry, bool>>(
			Expression.Constant(true),		//  函数体，直接返回 true
			Expression.Parameter(typeof(Poetry), "p"));		//  参数是叫 p 的 Poetry，张引说这句只在 debug 下打印调式信息时有用
	}
	*/
}

/// <summary>
/// 。 </summary>
public static class PoetryStorageConstant
{
	public const string _version_key_ = nameof(PoetryStorageConstant) + "." + nameof(_version_);
	public const int _version_ = 1;
}