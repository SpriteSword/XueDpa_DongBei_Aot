namespace XueDpa_DongBei_Aot.Model;

[SQLite.Table("poetry")]
public class Poetry
{
	private string? snippet;


	[SQLite.Column("id")] public int Id { get; set; }
	[SQLite.Column("name")] public string? Name { get; set; }
	[SQLite.Column("author_name")] public string Author { get; set; } = String.Empty;
	[SQLite.Column("dynasty")] public string Dynasty { get; set; } = String.Empty;
	[SQLite.Column("content")] public string Content { get; set; } = String.Empty;

	[SQLite.Ignore]     // ???: 不是数据库里的还需要写特性吗？难道不是默认忽略么？
	public string Snippet =>
		snippet ??= Content.Split('。')[0].Replace("\n", " ");
}

