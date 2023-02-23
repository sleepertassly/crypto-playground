using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoPlayground.Models
{
    public class TeamLetter
    {
		public int Id { get; set; }
		public int TeamId { get; set; }
		public int LetterId { get; set; }
		public DateTime? UnlockedOn { get; set; }
		public int RemainingAttempts { get; set; }
		public Team Team { get; set; }
		public Letter Letter { get; set; }
	}

}
