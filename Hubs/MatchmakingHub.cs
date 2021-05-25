using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchmakingService.Hubs
{
	public interface IMatchmakingHub {
		Task MatchFound(string matchId, string userId);
	}
	public class MatchmakingHub:Hub<IMatchmakingHub>
	{
		private readonly ILogger<MatchmakingHub> logger;

		public MatchmakingHub(ILogger<MatchmakingHub> logger)
		{
			this.logger = logger;
		}
		public override Task OnConnectedAsync()
		{
			logger.LogDebug($"A new connection has been established to the {nameof(MatchmakingHub)}");
			return base.OnConnectedAsync();
		}
		public Task MatchFound(string matchId, string userId) {
			return Clients.All.MatchFound(matchId, userId);		}
	}
}
