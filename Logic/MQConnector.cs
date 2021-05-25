using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;

using RabbitMQ.Client;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchmakingService.Logic
{
	public class MQConnector: IDisposable
	{
		private readonly ConnectionFactory factory;
		private readonly IConnection connection;
		private readonly IModel channel;
		private readonly ILogger<MQConnector> logger;

		public MQConnector(IOptions<ConnectionStrings> connectionStrings, ILogger<MQConnector> logger) {
			this.logger = logger;

			this.factory = new ConnectionFactory() { 
				Endpoint=new AmqpTcpEndpoint(new Uri(connectionStrings.Value.MQ)),
				UserName="letschess",
				Password= "ht4boiuehgjofmcjhyudi"
			};
			try
			{
				this.connection = factory.CreateConnection();
				this.channel = connection.CreateModel();

				channel.QueueDeclare("match", durable: false, exclusive: false, autoDelete: false);
				logger.LogInformation("Connection to MQ set up");
			}
			catch (Exception e) {
				logger.LogError(e, $"failed to connect to MQ with error: {e.Message}");
			}
		}

		public void Dispose()
		{
			this.connection.Dispose();
			this.channel.Dispose();
		}

		public void MatchFound(string matchId, string player1, string player2) {
			var body = JsonConvert.SerializeObject(new MatchFoundMessage { MatchId = matchId, P1 = player1, P2 = player2 });
			channel.BasicPublish("", "match", true, null, Encoding.UTF8.GetBytes(body));
		}

		protected class MatchFoundMessage { 
			public string MatchId { get; set; }
			public string P1 { get; set; }
			public string P2 { get; set; }
		}
	}
}
