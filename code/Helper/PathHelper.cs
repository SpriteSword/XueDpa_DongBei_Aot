namespace XueDpa_DongBei_Aot.Helper;

static class PathHelper
{
	static string local_folder = string.Empty;

	static string LocalFolder
	{
		get
		{
			if (!string.IsNullOrEmpty(local_folder))
			{
				return local_folder;
			}

			local_folder = Path.Combine(
				Environment.GetFolderPath(
					Environment.SpecialFolder.LocalApplicationData),
					AppDomain.CurrentDomain.FriendlyName		// ???: AppDomain 比程序集的概念要大吧？原来 nameof(XueDpa_DongBei_Aot)
				);

			if (!Directory.Exists(local_folder))
			{
				_ = Directory.CreateDirectory(local_folder);
			}

			return local_folder;
		}
	}

	public static string GetLocalFilePath(string file_name)
	{
		return Path.Combine(LocalFolder, file_name);
	}
}