using System.Linq.Expressions;
using XueDpa_DongBei_Aot.Model;
namespace XueDpa_DongBei_Aot.Helper;


static class QueryHelper
{
	//  万能查询语句
	public readonly static Expression<Func<Poetry, bool>> _where_dflt_ =
		Expression.Lambda<Func<Poetry, bool>>(
			Expression.Constant(true),
			Expression.Parameter(typeof(Poetry), "p"));

}