using System.Linq;
using System.Linq.Expressions;
using Moq;
using XueDpa_DongBei_Aot.Helper;
using XueDpa_DongBei_Aot.Model;
using XueDpa_DongBei_Aot.Service;
using XueDpa_DongBei_Aot.Test.Helper;
using Xunit;

namespace XueDpa_DongBei_Aot.Test.Service;

public class PoetryStorageTest : IDisposable
{
	public PoetryStorageTest() =>
		PoetryStorageHelper.RemoveDbFile();

	public void Dispose()
	{
		PoetryStorageHelper.RemoveDbFile(); //+++++  万一文件不存在会崩吗？不会
	}


	[Fact]
	static void Test_IsIntlzed()
	{
		Mock<IPrfrncStorage> p_stroage_mock = new();
		p_stroage_mock
			.Setup(static p => p.Get(PoetryStorageConstant._version_key_, default(int)))
			.Returns(PoetryStorageConstant._version_); //  假如有人用默认值调用你，就返回版本号

		IPrfrncStorage ps_mock_obj = p_stroage_mock.Object;
		PoetryStorage poetry_strg = new(ps_mock_obj);
		Assert.True(poetry_strg.IsIntlzd);

		p_stroage_mock.Verify(
			static p => p.Get(PoetryStorageConstant._version_key_, default(int)),
			Times.Once); //  验证真有人调用该函数1次！！
	}

	//
	[Fact]
	async Task Test_InitAsync()
	{
		//  mock 欺骗。为什么不会对外边产生影响？
		Mock<IPrfrncStorage> p_stroage_mock = new();
		PoetryStorage poetry_strg = new(p_stroage_mock.Object);

		Assert.False(File.Exists(PoetryStorage._poetry_db_path_));
		await poetry_strg.InitAsync();
		Assert.True(File.Exists(PoetryStorage._poetry_db_path_));
		await poetry_strg.CloseAsync();
	}

	[Fact]
	async void Test_GetPoetryAsync()
	{
		var ps = await PoetryStorageHelper.GetIntlzedPoetryStorage();
		var p = await ps.GetPoetryAsync(10001);
		Assert.Equal("临江仙·夜归临皋", p.Name);
		p = await ps.GetPoetryAsync(10019);
		Assert.Equal("观书有感", p.Name);

		await ps.CloseAsync();
	}

	[Fact]
	async Task Test_GetPoetriesAsync()
	{
		const int _num_poetry_ = 30;

		var ps = await PoetryStorageHelper.GetIntlzedPoetryStorage();
		var poetries = await
			ps.GetPoetriesAsync(
				QueryHelper._where_dflt_,
				0, int.MaxValue);

		Assert.Equal(_num_poetry_, poetries.Count);
		await ps.CloseAsync();
	}

}



//  另开一个单独测试
public class PoetryStorageTest2 : IDisposable
{
	public PoetryStorageTest2() =>
		PoetryStorageHelper.RemoveDbFile();

	public void Dispose()
	{
		PoetryStorageHelper.RemoveDbFile(); //+++++  万一文件不存在会崩吗？不会
	}


	//  这货要单独测试
	[Fact]
	async Task Test_InitAsync()
	{
		//  mock 欺骗。为什么不会对外边产生影响？
		Mock<IPrfrncStorage> p_stroage_mock = new();
		PoetryStorage poetry_strg = new(p_stroage_mock.Object);

		Assert.False(File.Exists(PoetryStorage._poetry_db_path_));
		await poetry_strg.InitAsync();
		Assert.True(File.Exists(PoetryStorage._poetry_db_path_));
		await poetry_strg.CloseAsync();
	}
}