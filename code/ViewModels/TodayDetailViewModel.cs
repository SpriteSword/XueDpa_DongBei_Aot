using XueDpa_DongBei_Aot.Model;

namespace XueDpa_DongBei_Aot.ViewModels;

public class TodayDetailViewModel : ViewModelBase
{
	TodayPoetry? today_poetry;

	//++++  不加 public 居然 axaml 也能绑定？因为是靠反射的？AOT也行，但是debug就不行，太迷了吧？编译期不报错，运行时异常吞了，什么垃圾？？
	public TodayPoetry? TodayPoetry
	{
		get => today_poetry;
		private set => SetProperty(ref today_poetry, value);
	}


	public override void SetParameter(object? prmtr)
	{
		TodayPoetry = prmtr as TodayPoetry;
	}
}