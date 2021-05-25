using LetsChess_MatchmakingService.Logic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LetsChess_MatchmakingService.Controllers
{
	[ApiController]
	[Route("/matchmaking")]
	public class MatchmakingController : ControllerBase
	{
		private readonly ILogger<MatchmakingController> logger;
		private readonly Matchmaking matchmaking;
		public MatchmakingController(ILogger<MatchmakingController> logger, Matchmaking matchmaking)
		{
			this.logger = logger;
			this.matchmaking = matchmaking;		
		}

		[HttpPost("findmatch")]
		public IActionResult FindMatch(string userId)
		{
			if (userId == null) return BadRequest($"the field userId was not supplied");
			logger.LogDebug("Findmatch endpoint called for userId",userId);
			matchmaking.AddPlayer(new Player(userId));
			return Ok($"player with id {userId} added");
		}
	}
}
