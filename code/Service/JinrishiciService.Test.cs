using System.Text.Json;
using Moq;
using XueDpa_DongBei_Aot.Service;
using XueDpa_DongBei_Aot.Test.Helper;
using Xunit;

namespace XueDpa_DongBei_Aot.Test.Service;

public class JinrishiciServiceTest
{

	// [Fact]
	// async Task Test_GetTokenAsync()
	// {
	// 	Mock<IAlertService> alert_service_mock = new();

	// 	JinrishiciService jinrishici = new(alert_service_mock.Object);
	// 	string json = await jinrishici.GetTokenAsync();
	// 	Assert.False(string.IsNullOrWhiteSpace(json));
	// }


	/// <summary>
	/// 不要每次都跑测试，避免每次都连一下网。 </summary>
	// [Fact]
	[Fact(Skip = "依赖远程服务的测试")]
	async Task GetTokenAsync_ReturnIsNotNullOrWhiteSpace()
	{
		Mock<IAlertService> alert_service_mock = new();
		var alert_service = alert_service_mock.Object;

		Mock<IPrfrncStorage> prfrnc_mock = new();
		var prfrnc = prfrnc_mock.Object;

		Mock<IPoetryStorage> poetry_storage_mock = new(); //  mock 要接口才能！！！
		var poetry_storage = poetry_storage_mock.Object;

		JinrishiciService jinrishici = new(alert_service, prfrnc, poetry_storage);
		var token = await jinrishici.GetTokenAsync();

		Assert.False(string.IsNullOrWhiteSpace(token));

		//  断言 alert 的函数没被调用过
		alert_service_mock.Verify(p => p.Alert(It.IsAny<string>(), It.IsAny<string>()), Times.Never);

		prfrnc_mock.Verify(p => p.Get(JinrishiciService._jinrishici_token_key_, string.Empty), Times.Once);
		prfrnc_mock.Verify(p => p.Set(JinrishiciService._jinrishici_token_key_, token), Times.Once);
	}

	/// <summary>
	/// 测试网络异常情况。 </summary>
	[Fact]
	async Task GetTokenAsync_NetworkError()
	{
		Mock<IAlertService> alert_service_mock = new();
		var alert_service = alert_service_mock.Object;

		Mock<IPrfrncStorage> prfrnc_mock = new();
		var prfrnc = prfrnc_mock.Object;

		Mock<IPoetryStorage> poetry_storage_mock = new();
		var poetry_storage = poetry_storage_mock.Object;

		JinrishiciService jinrishici = new(alert_service, prfrnc, poetry_storage, "http://no.such.url");
		var token = await jinrishici.GetTokenAsync();

		Assert.True(string.IsNullOrWhiteSpace(token));

		//  断言 alert 的函数没被调用过
		alert_service_mock.Verify(p => p.Alert(It.IsAny<string>(), It.IsAny<string>()), Times.Once);

		prfrnc_mock.Verify(p => p.Get(JinrishiciService._jinrishici_token_key_, string.Empty), Times.Once);
		prfrnc_mock.Verify(p => p.Set(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
		// ++++++ 验证 poetry_storage_mock
	}

	// [Fact]
	[Fact(Skip = "依赖远程服务的测试")]
	async Task GetTodayPoetryAsync_ReturnFromJinrishici()
	{
		Mock<IAlertService> alert_service_mock = new();
		var alert_service = alert_service_mock.Object;
		Mock<IPrfrncStorage> prfrnc_mock = new();
		var prfrnc = prfrnc_mock.Object;
		Mock<IPoetryStorage> poetry_storage_mock = new();
		var poetry_storage = poetry_storage_mock.Object;

		JinrishiciService jinrishici = new(alert_service, prfrnc, poetry_storage);
		var today_poetry = await jinrishici.GetTodayPoetryAsync();


		Assert.Equal(TodayPoetrySource.Jinrishici, today_poetry.Source); //  验证数据来源
		Assert.False(string.IsNullOrWhiteSpace(today_poetry.Snippet));


		//  断言 alert 的函数没被调用过
		alert_service_mock.Verify(p => p.Alert(It.IsAny<string>(), It.IsAny<string>()), Times.Never);

		prfrnc_mock.Verify(p => p.Get(JinrishiciService._jinrishici_token_key_, string.Empty), Times.Once);
		prfrnc_mock.Verify(p => p.Set(JinrishiciService._jinrishici_token_key_, It.IsAny<string>()), Times.Once);
	}


	[Fact]
	async Task GetRandomPoetryAsync_Dflt()
	{
		Mock<IAlertService> alert_service_mock = new();
		Mock<IPrfrncStorage> prfrnc_mock = new();
		PoetryStorage poetry_storage = await PoetryStorageHelper.GetIntlzedPoetryStorage();

		JinrishiciService jinrishici = new(alert_service_mock.Object, prfrnc_mock.Object, poetry_storage);
		var random_poetry = await jinrishici.GetRandomPoetryAsync();
		Assert.NotNull(random_poetry);
		Assert.False(string.IsNullOrWhiteSpace(random_poetry.Name));

		await poetry_storage.CloseAsync();
		PoetryStorageHelper.RemoveDbFile();
	}


	[Fact]
	void Test_Deserialize()
	{
		string json = "{\"status\":\"abcd\",\"data\":\"hello\"}";
		// var jinrishici_token = JsonSerializer.Deserialize<JinrishiciToken>(json); //+++++  好可恨！
		JinrishiciToken? jinrishici_token = JsonSerializer.Deserialize(json, SourceGnrtnContextJinrishici.Default.JinrishiciToken);

		Assert.NotNull(jinrishici_token);
		Assert.Equal("abcd", jinrishici_token.Status);
		Assert.Equal("hello", jinrishici_token.Data);
	}
}
