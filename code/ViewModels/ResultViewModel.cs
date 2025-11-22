using System.Linq.Expressions;
using AvaloniaInfiniteScrolling;
using XueDpa_DongBei_Aot.Model;
using XueDpa_DongBei_Aot.Service;
using XueDpa_DongBei_Aot.ViewModels;
using XueDpa_DongBei_Aot.Helper;
using XueDpa_DongBei_Aot.Service.Navigation;


namespace XueDpa_DongBei_Aot.ViewModels;

public class ResultViewModel : ViewModelBase
{
	public const int _page_size_ = 20;

	public const string _loading_ = "正在载入"; //++++  应该用枚举吧？
	public const string _no_result_ = "没有满足条件的结果";
	public const string _no_more_result_ = "没有更多结果";

	readonly IPoetryStorage _poetry_storage_;
	readonly IContentNvgtnService _content_nvgtn_service_;

	bool can_load_more = true;
	private string? status;

	// private Expression<Func<Poetry, bool>>? where; //  查询的语句


	public string? Status
	{
		get => status;
		private set => SetProperty(ref status, value); //  典型 MVVM。写 status = value; 不行，SetProperty() 用来触发 状态改变消息。
	}

	// 1.  普通显示
	// public ObservableCollection<Poetry> PoetryCollection { get; } = [];
	// public ICommand OnIntlzdCmnd { get; } //  初始化后马上调用的命令

	// public string Greeting { get; } = "Res: Welcome to Avalonia!";


	//  2. 无限滚动
	public AvaloniaInfiniteScrollCollection<Poetry> PoetryCollection { get; }

	// public IRelayCommand<Poetry> ShowPoetryCmnd { get; } //  还不能传多个参数


	public ResultViewModel(IPoetryStorage poetry_storage, IContentNvgtnService content_nvgtn_service)
	{
		_poetry_storage_ = poetry_storage;
		_content_nvgtn_service_ = content_nvgtn_service;

		//  临时解决方案。数据库要初始化，数据库要存在在运行目录。运行过就会生成一个数据库在 ~/.local 下，但是第一次运行显示不出来？
		// poetry_storage.InitAsync().RunSynchronously();

		// 1.
		// OnIntlzdCmnd = new AsyncRelayCommand(OnIntlzdAsync); // ???: 为什么不能直接 new 写到变量上，说不能推断 OnIntlzedAsync 的类型？函数都静态的啊？Task不能编译时推断？


		// XXX: 确实分批加载了，但停不下来，一直把所有数据都加载了，难道不是我滚动到底部才加载吗？
		PoetryCollection = new AvaloniaInfiniteScrollCollection<Poetry>()
		{
			OnCanLoadMore = () => can_load_more,
		};
		//  因为函数体用到了 PoetryCollection.Count，只能后面再
		PoetryCollection.OnLoadMore = async () =>
		{
			// 1.
			// return await _poetry_storage_.GetPoetriesAsync(
			// 	QueryHelper._where_dflt_,
			// 	PoetryCollection.Count, _page_size_
			// );


			Status = _loading_;

			IList<Poetry> poetries = await _poetry_storage_.GetPoetriesAsync(
				QueryHelper._where_dflt_,
				PoetryCollection.Count, _page_size_);

			Status = String.Empty; // ???: 多余的？有用吗？

			if (poetries.Count < _page_size_)
			{
				can_load_more = false;
				Status = _no_more_result_;
			}

			if (PoetryCollection.Count != 0 || poetries.Count != 0)
			{
				return poetries;
			}

			can_load_more = false;
			Status = _no_result_;
			return poetries;
		};


		// ShowPoetryCmnd = new RelayCommand<Poetry>(ShowPoetry);
	}

	// 1. 实验
	// /// <summary>
	// /// on initialized async </summary>
	// public async Task OnIntlzdAsync()
	// {
	// 	Console.WriteLine("Res: OnIntlzdAsync");

	// 	await _poetry_storage_.InitAsync();

	// 	var poetries = await
	// 		_poetry_storage_.GetPoetriesAsync(QueryHelper._where_dflt_, 0, int.MaxValue);

	// 	foreach (var poetry in poetries)
	// 	{
	// 		PoetryCollection.Add(poetry);
	// 	}
	// }

	public override void SetParameter(object? prmtr)
	{
		if (prmtr is not Expression<Func<Poetry, bool>> w)
		{
			return;
		}

		// where = w;
		can_load_more = true;
		PoetryCollection.Clear();
	}


	public void ShowPoetry(Poetry? poetry)
	{
		_content_nvgtn_service_.NavigateTo(ContentNvgtnConstant.DetailView, poetry);
	}
}