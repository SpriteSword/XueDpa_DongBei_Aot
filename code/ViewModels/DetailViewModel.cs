using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using XueDpa_DongBei_Aot.Model;
using XueDpa_DongBei_Aot.Service;
namespace XueDpa_DongBei_Aot.ViewModels;


public class DetailViewModel : ViewModelBase
{
	private readonly IFavoriteStorage _favorite_storage_;

	Poetry? poetry;
	Favorite? favorite;
	bool is_loading;


	public Poetry? Poetry
	{
		get => poetry;
		set => SetProperty(ref poetry, value);
	}

	public Favorite? Favorite
	{
		get => favorite;
		set => SetProperty(ref favorite, value);
	}

	public bool IsLoading
	{
		get => is_loading;
		set => SetProperty(ref is_loading, value);
	}

	// private ICommand OnLoadedCmnd { get; }
	public ICommand FavoriteSwitchCmnd { get; }


	public DetailViewModel(IFavoriteStorage favorite_storage)
	{
		_favorite_storage_ = favorite_storage;

		// OnLoadedCmnd = new AsyncRelayCommand(OnLoadedAsync);
		FavoriteSwitchCmnd = new AsyncRelayCommand(FavoriteSwitchClickedAsync);
	}


	public override void SetParameter(object? prmtr)
	{
		if (prmtr is Poetry p)
		{
			Poetry = p;
		}
	}

	public async Task OnLoadedAsync()
	{
		if (Poetry == null) { throw new NullReferenceException("数据损坏，Poetry 不能为空！"); }

		IsLoading = true;
		Favorite f = await _favorite_storage_.GetFavoriteAsync(Poetry.Id)
			?? new Favorite() { PoetryId = Poetry.Id };      //  只为了不为空，但不一定真的收藏了
		Favorite = f;
		IsLoading = false;
	}

	public async Task FavoriteSwitchClickedAsync()
	{
		if (Favorite == null) { throw new NullReferenceException("数据损坏，Favorite 不能为空！"); }

		IsLoading = true;
		await _favorite_storage_.SaveFavoriteAsync(Favorite);
		IsLoading = false;
	}

	// ???: 没有取消收藏从数据库移除？？好像也没必要，就存几个int数而已也不会占空间
}