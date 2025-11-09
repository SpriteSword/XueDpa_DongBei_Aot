using Moq;
using XueDpa_DongBei_Aot.Service;

namespace XueDpa_DongBei_Aot.Test.Helper;

public static class PoetryStorageHelper
{
	public static void RemoveDbFile() =>
		File.Delete(PoetryStorage._poetry_db_path_);

	/// <summary>
	/// 用来准备测试用的数据库。 </summary>
	public static async Task<PoetryStorage> GetIntlzedPoetryStorage()
	{
		Mock<IPrfrncStorage> ps_mock = new();
		_ = ps_mock.Setup(p => p.Get(PoetryStorageConstant._version_key_, -1))
			.Returns(-1);

		IPrfrncStorage psm_obj = ps_mock.Object;
		PoetryStorage poetry_strg = new(psm_obj);
		await poetry_strg.InitAsync();
		return poetry_strg;
	}
}