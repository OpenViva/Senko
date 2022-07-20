using System;

namespace Data
{
	public class Directories
	{
		public static string MySqlConfigPath { get { return $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}{Path.DirectorySeparatorChar}senko{Path.DirectorySeparatorChar}mysqlconfig.json"; } }
		public static string AppData { get { return $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}{Path.DirectorySeparatorChar}senko{Path.DirectorySeparatorChar}"; } }
		public static string Config { get { return $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}{Path.DirectorySeparatorChar}senko{Path.DirectorySeparatorChar}config.json"; } }
	}
}
