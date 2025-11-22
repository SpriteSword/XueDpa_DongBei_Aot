using CommunityToolkit.Mvvm.Messaging.Messages;
using XueDpa_DongBei_Aot.Model;

namespace XueDpa_DongBei_Aot.Service;

public interface IFavoriteStorage
{
	bool IsIntlzd { get; }

	event EventHandler<Favorite> Updated; //  有个收藏发生变化

	Task InitializeAsync();
	Task<Favorite> GetFavoriteAsync(int poetry_id);
	Task<IEnumerable<Favorite>> GetFavoritesAsync();
	Task SaveFavoriteAsync(Favorite favorite);
}

//  弱引用消息，相当于事件。不会阻碍 GC。事件适合 处理方长时间存在，发起方短时间存在，消息反过来。
//  事件是 1 对多，多播。消息是广播的，任何人都能收发。这是内存里的模型
//  messager 可以是服务器，跨机器！???: 事件能不能跨机器？？
public class FavoriteStorageUpdatedMsg(Favorite value) : ValueChangedMessage<Favorite>(value)
{
}