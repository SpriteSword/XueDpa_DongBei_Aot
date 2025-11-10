using XueDpa_DongBei_Aot.Service;
using XueDpa_DongBei_Aot.ViewModels;
using XueDpa_DongBei_Aot.Test.Helper;
using Xunit;
using System.Linq;
using Moq;
using XueDpa_DongBei_Aot.Service.Navigation;

namespace XueDpa_DongBei_Aot.Test.ViewModels;


public class ResultViewModelTest : IDisposable
{
	public ResultViewModelTest() =>
		PoetryStorageHelper.RemoveDbFile();

	public void Dispose() =>
		PoetryStorageHelper.RemoveDbFile();


	[Fact]
	public async Task Test_PoetryCollection()
	{
		Mock<IContentNvgtnService> content_nvgtn_service_mock = new();
		PoetryStorage poetry_storage = await PoetryStorageHelper.GetIntlzedPoetryStorage();
		ResultViewModel result_view_model = new(poetry_storage, content_nvgtn_service_mock.Object);

		//  记录每次状态变化。
		List<string?> status_list = [];
		result_view_model.PropertyChanged += (sender, args) =>
		{
			if (args.PropertyName == nameof(result_view_model.Status)) //+++++这是防止其他人的调用给传到这来了？要看看是不是 Status？
			{
				status_list.Add(result_view_model.Status);
			}
		};

		// Assert.Equal(0, result_view_model.PoetryCollection.Count);		//  过时！
		Assert.Empty(result_view_model.PoetryCollection);

		await result_view_model.PoetryCollection.LoadMoreAsync();
		Assert.Equal(20, result_view_model.PoetryCollection.Count);
		Assert.Equal("观书有感", result_view_model.PoetryCollection.Last().Name);
		Assert.True(result_view_model.PoetryCollection.CanLoadMore);
		Assert.Equal(2, status_list.Count); //  每次次载入，result_view_model.Status 都会先变成 "loading" 再变空 ""。
		Assert.Equal(ResultViewModel._loading_, status_list[0]);
		Assert.Equal("", status_list[1]);

		bool poetry_collection_changed = false;
		result_view_model.PoetryCollection.CollectionChanged +=
			(sender, args) => poetry_collection_changed = true; //+++++ 看不见

		await result_view_model.PoetryCollection
			.LoadMoreAsync(); //  +++++++ 直接用 dotnet test 这句会失败？？ SQLite.SQLiteException : no such table: poetry？


		Assert.True(poetry_collection_changed);
		Assert.Equal(30, result_view_model.PoetryCollection.Count); //  得事先准备好数据
		Assert.Equal("记承天寺夜游", result_view_model.PoetryCollection[29].Name); //  不用.Last()？
		Assert.False(result_view_model.PoetryCollection.CanLoadMore);
		Assert.Equal(5, status_list.Count); //  2*2+1，最后多1个 "没有更多结果"
		Assert.Equal("", status_list[3]);
	}
}