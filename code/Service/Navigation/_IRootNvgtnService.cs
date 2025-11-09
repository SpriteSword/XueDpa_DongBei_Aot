namespace XueDpa_DongBei_Aot.Service.Navigation;


/// <summary>
/// 根导航接口。 </summary>
public interface IRootNvgtnService
{
	void NavigateTo(string view);
}


//+++还能这么干？不搞个基类？
//  用字符串区分不同的 view model
public static class RootNvgtnConstant
{
	//  initialization view
	public const string IntlztnView = nameof(IntlztnView);
	public const string MainView = nameof(MainView);
}