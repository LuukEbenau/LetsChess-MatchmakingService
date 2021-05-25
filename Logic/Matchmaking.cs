using LetsChess_MatchmakingService.Hubs;

using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LetsChess_MatchmakingService.Logic
{
	public class Matchmaking
	{
		private readonly Queue<Player> _playersInQueue = new();
		private readonly MQConnector _mQConnector;
		private readonly IHubContext<MatchmakingHub, IMatchmakingHub> matchmakingHub;
		private readonly ILogger<Matchmaking> logger;

		public List<Match> Matches { get; } = new();
		public Matchmaking(MQConnector mQConnector, IHubContext<MatchmakingHub, IMatchmakingHub> matchmakingHub, ILogger<Matchmaking> logger) {
			this._mQConnector = mQConnector;
			this.matchmakingHub = matchmakingHub;
			this.logger = logger;
		}
		public void AddPlayer(Player player) {
			_playersInQueue.Enqueue(player);
			if (_playersInQueue.Count > 1) {
				var p1 = _playersInQueue.Dequeue();
				var p2 = _playersInQueue.Dequeue();
				var match = new Match(p1, p2);
				Matches.Add(match);

				logger.LogInformation($"found 2 players '{match.Player1.UserId}' and {match.Player2.UserId} waiting for match, creating match between them with id '{match.Id}'");
				_mQConnector.MatchFound(match.Id, match.Player1.UserId, match.Player2.UserId);
				matchmakingHub.Clients.All.MatchFound(match.Id, match.Player1.UserId);
				matchmakingHub.Clients.All.MatchFound(match.Id, match.Player2.UserId);
			}
		}
	}
}
