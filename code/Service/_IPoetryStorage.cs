using System.Linq.Expressions;
using XueDpa_DongBei_Aot.Model;

namespace XueDpa_DongBei_Aot.Service;


public interface IPoetryStorage
{
	bool IsIntlzd { get; }        //  is initialized

	Task InitAsync();       //  initialize Async

	Task<Poetry> GetPoetryAsync(int id);

	/// skip 翻页
	Task<IList<Poetry>> GetPoetriesAsync(Expression<Func<Poetry, bool>> where, int skip, int take);     //  万能查询。性能差。
}