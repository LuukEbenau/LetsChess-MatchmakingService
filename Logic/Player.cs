using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchmakingService.Logic
{
	public class Player
	{
		public string UserId { get; }
		public int Elo { get; }
		public Player(string userId) {
			this.UserId = userId;
			this.Elo = 800;
		}
	}
}
