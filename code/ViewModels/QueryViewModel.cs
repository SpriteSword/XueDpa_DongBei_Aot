using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using XueDpa_DongBei_Aot.Model;
using XueDpa_DongBei_Aot.Service.Navigation;

namespace XueDpa_DongBei_Aot.ViewModels;

//
public class QueryViewModel : ViewModelBase
{
	readonly IContentNvgtnService _content_nvgtn_service_;

	public ObservableCollection<FilterViewModel> FilterViewModels { get; }

	public ICommand QueryCmnd { get; }


	public QueryViewModel(IContentNvgtnService content_nvgtn_service_)
	{
		_content_nvgtn_service_ = content_nvgtn_service_;
		FilterViewModels = [new FilterViewModel(this)];
		QueryCmnd = new RelayCommand(Query);
	}

	public void AddFilterViewModel(FilterViewModel filter_vm)
	{
		FilterViewModels.Insert(FilterViewModels.IndexOf(filter_vm) + 1, new FilterViewModel(this));
	}

	public virtual void RemoveFilterViewModel(FilterViewModel filter_vm)
	{
		_ = FilterViewModels.Remove(filter_vm);
		if (FilterViewModels.Count == 0)
		{
			FilterViewModels.Add(new(this));
		}
	}

	/// <summary>
	/// 查询。应用场景有限，因为是要把数据库中所有数据都复制到内存才能执行 lambda 表达式的函数体，而不是直接由数据库查询！！！！ </summary>
	public void Query()
	{
		//  为了能：
		// Connection.Table<Poetry>().Where(p => p.Name.Contains("xxx")
		//                                      && p.Author.Contains("xxx")
		//                                      && p.Content.Contains("xxx")).Tolist();
		//  下面的 prmtr 就是上面的 p
		ParameterExpression prmtr = Expression.Parameter(typeof(Poetry), "p");

		//  aggregated expression 聚合表达式
		Expression agrgtd_exprsn = FilterViewModels
			.Where(f => !string.IsNullOrWhiteSpace(f.Content))
			.Select(f => GetExpression(prmtr, f))
			//  聚合成例如：true && p.Name.Contains("苏轼") && ...
			.Aggregate(Expression.Constant(true) as Expression, Expression.AndAlso);

		Expression<Func<Poetry, bool>> where = Expression.Lambda<Func<Poetry, bool>>(agrgtd_exprsn, prmtr);

		_content_nvgtn_service_.NavigateTo(ContentNvgtnConstant.ResultView, where);
	}

	/// <summary>
	/// 获取拼接好的查询语句。 </summary>
	/// <param name="prmtr">要传入 Poetry 类型</param>
	/// <param name="filter_vm"></param>
	/// <returns></returns>
	static MethodCallExpression GetExpression(ParameterExpression prmtr, FilterViewModel filter_vm)//  编译器建议“System.Linq.Expressions.Expression”更改为“System.Linq.Expressions.MethodCallExpression”以提高性能
	{
		//  prmtr 将是 Poetry
		//  获取 prmtr 的属性。filter_vm.Type.PropertyName 关联了了 Poetry 中的属性名 Name、Author、Content。
		MemberExpression property = Expression.Property(prmtr, filter_vm.Type.PropertyName);

		//  Contains(string s)，上面获取的属性将会调用这个。反射
		System.Reflection.MethodInfo? method = typeof(string).GetMethod("Contains", [typeof(string)]);

		//  上面 method 的参数
		ConstantExpression condition = Expression.Constant(filter_vm.Content, typeof(string));

		return Expression.Call(property, method, condition);
	}
}

//  不能继承自 ViewModelBase，因为 ViewLocator 会试图给你转成 view。
//++++++ Filter 不是过滤器的意思吗，怎么好像不太对？？
/// <summary>
/// 在可变动列表显示的每个项对应的viewmodel。 </summary>
public class FilterViewModel : ObservableObject
{
	readonly QueryViewModel _query_view_model_; //+++++=这样做也太不雅了，不应该用消息吗，由上级实例化下级并连接信号

	string? content;
	private FilterType type = FilterType._name_filter_;


	public string? Content
	{
		get => content;
		set => SetProperty(ref content, value);
	}

	public FilterType Type
	{
		get => type;
		set => SetProperty(ref type, value);
	}

	public ICommand AddCmnd { get; }
	public ICommand RemoveCmnd { get; }


	public FilterViewModel(QueryViewModel query_view_model_)
	{
		_query_view_model_ = query_view_model_;

		AddCmnd = new RelayCommand(Add);
		RemoveCmnd = new RelayCommand(Remove);
	}

	public void Add()
	{
		_query_view_model_.AddFilterViewModel(this);
	}

	public void Remove()
	{
		_query_view_model_.RemoveFilterViewModel(this);
	}
}

//+++++难道只能这样？因为不能公开构造函数，所以也不能有公共全局类，所以 C# 的所有单例都是要这样分散在一个个类中？？？
public class FilterType
{
	//  为了能 全局唯一 且 不增不减、只读
	public static readonly FilterType _name_filter_ = new("标题", nameof(Poetry.Name));
	public static readonly FilterType _author_name_filter_ = new("作者", nameof(Poetry.Author));
	public static readonly FilterType _content_filter_ = new("内容", nameof(Poetry.Content));


	public static List<FilterType> FilterTypes { get; } = [_name_filter_, _author_name_filter_, _content_filter_];

	public string Name { get; }
	public string PropertyName { get; }


	FilterType(string name, string property_name)
	{
		Name = name;
		PropertyName = property_name;
	}
}