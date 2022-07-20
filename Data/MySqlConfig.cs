using System.IO;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml;
using Newtonsoft.Json;

namespace Data
{
	public class MySqlConfig
	{
		//Server=myServerAddress;Port=1234;Database=myDataBase;Uid=myUsername;Pwd=myPassword;
		public string Address { get; set; }
		public string Port { get; set; }
		public string Database { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }

		private string DatabaseNameOverride { get; set; }


		[Newtonsoft.Json.JsonConstructor]
		public MySqlConfig()
		{

		}
		/// <summary>
		/// Override the "database=" portion of the connection string configured in the file
		/// </summary>
		/// <param name="database">Database name</param>
		public MySqlConfig(string database)
		{
			this.DatabaseNameOverride = database;
		}

		public string GetConnectionString()
		{
			StringBuilder builder = new StringBuilder("Server=");
			builder.Append(Address.ToString());
			builder.Append("; Port=");
			builder.Append(Port.ToString());
			builder.Append("; Database=");
			if (string.IsNullOrEmpty(this.DatabaseNameOverride))
				builder.Append(Database.ToString());
			else builder.Append(DatabaseNameOverride.ToString());
			builder.Append("; Uid=");
			builder.Append(Username.ToString());
			builder.Append("; Pwd=");
			builder.Append(Password.ToString());
			builder.Append(";");
			return builder.ToString();
		}
		public static MySqlConfig FromJsonString(string json)
		{
			return JsonConvert.DeserializeObject<MySqlConfig>(json);
		}

		public static MySqlConfig FromConfigFile()
		{
			if (!Directory.Exists(Directories.AppData)) Directory.CreateDirectory(Directories.AppData);
			if (!File.Exists(Directories.MySqlConfigPath))
			{
				MySqlConfig config = new MySqlConfig();
				config.Address = "Address";
				config.Port = "Port";
				config.Database = "Database";
				config.Username = "Username";
				config.Password = "Password";
				File.WriteAllText(Directories.MySqlConfigPath, JsonConvert.SerializeObject(config, Newtonsoft.Json.Formatting.Indented));
				return config;
			}
			else return JsonConvert.DeserializeObject<MySqlConfig>(File.ReadAllText(Directories.MySqlConfigPath));

		}
	}
}