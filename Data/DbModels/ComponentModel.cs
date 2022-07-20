using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DbModels
{
	public class ComponentModel
	{
		public Guid CustomId { get; set; }
		public DateTime Created { get; set; } = DateTime.Now;
		public ulong OwnerId { get; set; } = 0;
		public ulong ChannelId { get; set; } = 0;
		public ulong MessageId { get; set; } = 0;
		public string KyaruComponentType { get; set; }
	}
}
