using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LetsChess_MatchmakingService.Logic
{
	public class Match
	{ 
		public string Id { get; }
		public Player Player1 { get; }
		public Player Player2 { get; }
		public Match(Player player1, Player player2) {
			Id = Guid.NewGuid().ToString();
			Player1 = player1;
			Player2 = player2;
		}
	}
}
