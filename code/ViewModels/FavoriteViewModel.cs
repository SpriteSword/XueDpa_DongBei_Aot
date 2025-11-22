using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using XueDpa_DongBei_Aot.Model;
using XueDpa_DongBei_Aot.Service;
using XueDpa_DongBei_Aot.Service.Navigation;

namespace XueDpa_DongBei_Aot.ViewModels;

public class FavoriteViewModel : ViewModelBase, IRecipient<FavoriteStorageUpdatedMsg>
{
	readonly IFavoriteStorage _favorite_storage_;
	readonly IPoetryStorage _poetry_storage_;
	readonly IContentNvgtnService _content_nvgtn_service_;

	private bool is_loading;
	bool is_loaded;

	// ???: 直接用Poetry不好？
	public ObservableCollection<PoetryFavorite> PoetryFavoriteClctn { get; set; } = [];


	public bool IsLoading
	{
		get => is_loading;
		private set => SetProperty(ref is_loading, value);
	}

	// public ICommand OnIntlzdCmnd { get; }
	public IRelayCommand<Poetry> ShowPoetryCmnd { get; }


	public FavoriteViewModel(IFavoriteStorage favorite_storage,
		IPoetryStorage poetry_storage,
		IContentNvgtnService content_nvgtn_service)
	{
		_favorite_storage_ = favorite_storage;
		_favorite_storage_.Updated += OnFavoriteStorageUpdated;
		//  相当于 Updated 引用了 FavoriteViewModel，但没事，因为本来 VM 就是单例的。-= 有可能中间抛异常而没-=。所以事件适合 处理方长时间存在，发起方短时间存在

		WeakReferenceMessenger.Default.Register(this);     // ???: Default 又是单例？

		_poetry_storage_ = poetry_storage;
		_content_nvgtn_service_ = content_nvgtn_service;

		// OnIntlzdCmnd = new AsyncRelayCommand(OnIntlzdAsync);
		ShowPoetryCmnd = new RelayCommand<Poetry>(ShowPoetry);
	}


	public async Task OnIntlzdAsync()
	{
		if (is_loaded)
		{
			return;
		}

		IsLoading = true;
		is_loaded = true;

		PoetryFavoriteClctn.Clear();
		IEnumerable<Favorite> f_list = await _favorite_storage_.GetFavoritesAsync();

		// await Task.Delay(1000); //===

		/*
		int[] ints = [1, 2, 3, 4];
		double[] doubles = ints.Select(p => p * 1.0).ToArray();		// ???: 都是这么转的吗，不耗性能吗？

		//  只能拿到任务。还有异步 linq
		var fs_tasks = f_list.Select(async p => await _poetry_storage_.GetPoetryAsync(p.PoetryId)).ToArray();
		var fs = await Task.WhenAll(fs_tasks);
		*/


		// var p_favorites = (await Task.WhenAll(f_list.Select(f =>
		// 	Task.Run(async () => new PoetryFavorite
		// 	{
		// 		Poetry = await _poetry_storage_.GetPoetryAsync(f.PoetryId),
		// 		Favorite = f
		// 	})
		// ))).ToList();

		// ???: 按照上面的思路应该是这种写法，为什么会有上面的写法？？？
		PoetryFavorite[] p_favorites = await Task.WhenAll(f_list.Select(async f => new PoetryFavorite
		{
			Poetry = await _poetry_storage_.GetPoetryAsync(f.PoetryId),
			Favorite = f
		}).ToList());

		foreach (var pf in p_favorites)
		{
			PoetryFavoriteClctn.Add(pf);
		}

		IsLoading = false;
	}


	// ???: on 这个字不是加在最前面的？事件的返回值一定要 void，不能 Task！
	private async void OnFavoriteStorageUpdated(object? sender, Favorite favorite)
	{
		//  先删掉集合里的同一首诗。不等于同一个Poetry对象！！Remove() 传 null 也是安全的
#nullable disable
		_ = PoetryFavoriteClctn.Remove(
			PoetryFavoriteClctn.FirstOrDefault(p => p.Favorite.PoetryId == favorite.PoetryId));
#nullable enable

		if (!favorite.IsFavorite)
		{
			return;
		}

		PoetryFavorite pf = new()
		{
			Poetry = await _poetry_storage_.GetPoetryAsync(favorite.PoetryId),
			Favorite = favorite
		};

		// var index = PoetryFavoriteClctn.IndexOf(
		// 	PoetryFavoriteClctn.FirstOrDefault(p => p.Favorite.Timestamp < favorite.Timestamp));		// ???: 会改原变集合内容吗？排序吗？
		//
		// if (index == -1)
		// {
		// 	index = PoetryFavoriteClctn.Count;
		// }

		// PoetryFavoriteClctn.Insert(index, pf);

		PoetryFavoriteClctn.Insert(0, pf);		//++++++加在最后
	}

	//  后面又讲用消息
	public void Receive(FavoriteStorageUpdatedMsg message)
	{
		OnFavoriteStorageUpdated(message, message.Value);     //  瞎写 hh，虽然不报错
	}

	//
	public void ShowPoetry(Poetry? poetry)
	{
		_content_nvgtn_service_.NavigateTo(ContentNvgtnConstant.DetailView, poetry);
	}
}

//
/// <summary>
///  </summary>
public class PoetryFavorite
{
	public required Poetry Poetry { get; set; }
	public required Favorite Favorite { get; set; }
}