
using LetsChess_MatchmakingService.Messages;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;

using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LetsChess_MatchmakingService.Logic
{
	public class MQConnector: IDisposable
	{
		private readonly ConnectionFactory factory;
		private IConnection connection;
		private IModel channel;
		private readonly ILogger<MQConnector> logger;

		public MQConnector(IOptions<ConnectionStrings> connectionStrings, ILogger<MQConnector> logger, IOptions<MQCredentials> mqCredentials) {
			this.logger = logger;

			factory = new ConnectionFactory() { 
				Endpoint = new AmqpTcpEndpoint(new Uri(connectionStrings.Value.MQ)),
				UserName = mqCredentials.Value.Username,
				Password = mqCredentials.Value.Password,
			};

			Connect();
		}

		private bool Connect()
		{
			logger.LogDebug("connecting to MQ");
			try
			{
				connection = factory.CreateConnection();
				connection.ConnectionShutdown += Connection_ConnectionShutdown;
				connection.ConnectionBlocked += Connection_ConnectionBlocked;
				channel = connection.CreateModel();

				channel.ExchangeDeclare("matchmaking", ExchangeType.Direct);
				var args = new Dictionary<string, object>
				{
					{ "x-message-ttl", 10000 }
				};
				channel.QueueDeclare("matchmaking", durable: false, exclusive: false, autoDelete: false, arguments: args);
				channel.QueueBind("matchmaking", "matchmaking", "matchmaking");
				logger.LogDebug("succesfully connected to MQ");

				return true;
				
			}
			catch (BrokerUnreachableException ex)
			{
				logger.LogError(ex, $"Could not connect to the service, '{ex.Message}' see {ex.HelpLink} for more details");
			}
			catch (Exception ex)
			{
				logger.LogError(ex, $"An error occured while connecting to the service {ex.Message}");
			}
			return false;
		}

		private void Connection_ConnectionBlocked(object sender, RabbitMQ.Client.Events.ConnectionBlockedEventArgs e)
		{
			logger.LogDebug($"MQ connection got blocked for reason {e.Reason}");
		}

		private void Connection_ConnectionShutdown(object sender, ShutdownEventArgs e)
		{
			logger.LogInformation($"Connection to the MQ has been shutdown with reason: <{e.ReplyCode}> '{e.ReplyText}'");
			connection.ConnectionShutdown -= Connection_ConnectionShutdown;

			Connect();
		}

		public void Dispose()
		{
			connection.Dispose();
			channel.Dispose();
		}

		public void MatchFound(string matchId, string player, string opponent, bool playingWhite) {
			var connected = connection != default || connection.IsOpen || channel.IsOpen;
			if (!connected) { 
				connected = Connect(); 
			}
			if (connected)
			{
				logger.LogDebug($"publishing matchfound for match {matchId} of user {player}");
				logger.LogDebug($"connection status: {connection.IsOpen}, {connection.Heartbeat}, channel open{channel.IsOpen}");
				var body = JsonConvert.SerializeObject(new MatchFoundMessage { MatchId = matchId, UserId = player, Opponent=opponent, PlayingWhite = playingWhite });
				channel.BasicPublish(exchange: "matchmaking", routingKey: "matchmaking", body: Encoding.UTF8.GetBytes(body));
			}
		}


	}
}
