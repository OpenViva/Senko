using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Data;

namespace Senko
{
	class Config
	{

		string commandPrefix;
		string token;
		public string Token
		{
			get => token;
			set => token = value;
		}

		
		public string CommandPrefix
		{
			get => commandPrefix;
			set => commandPrefix = value;
		}
		

		public Config GetConfigFromFile()
		{
			if (!Directory.Exists(Directories.AppData)) Directory.CreateDirectory(Directories.AppData);
			if (File.Exists(Directories.Config))
			{
				return JsonConvert.DeserializeObject<Config>(File.ReadAllText(Directories.Config));
			}
			else
			{
				var config = new Config();
				File.WriteAllText(Directories.Config, JsonConvert.SerializeObject(config, Formatting.Indented));
				return config;
			}
		}
	}
}
