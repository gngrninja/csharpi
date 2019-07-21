using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace csharpi.Services
{
    public class LoggingService
    {

        // declare the fields used later in this class
        private readonly ILogger _logger;
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;

        public LoggingService(IServiceProvider services)
        { 
            // get the services we need via DI, and assign the fields declared above to them
            _discord = services.GetRequiredService<DiscordSocketClient>(); 
            _commands = services.GetRequiredService<CommandService>();
            _logger = services.GetRequiredService<ILogger<LoggingService>>();

            // hook into these events with the methods provided below
            _discord.Ready += OnReadyAsync;
            _discord.Log += OnLogAsync;
            _commands.Log += OnLogAsync;
        }

        // this method executes on the bot being connected/ready
        public Task OnReadyAsync()
        {
            _logger.LogInformation($"Connected as -> [{_discord.CurrentUser}] :)");
            _logger.LogInformation($"We are on [{_discord.Guilds.Count}] servers");
            return Task.CompletedTask;
        }

        // this method switches out the severity level from Discord.Net's API, and logs appropriately
        public Task OnLogAsync(LogMessage msg)
        { 
            string logText = $"{msg.Source}: {msg.Exception?.ToString() ?? msg.Message}";
            switch (msg.Severity.ToString())
            {
                case "Critical":
                {
                    _logger.LogCritical(logText);
                    break;
                }
                case "Warning":
                {
                    _logger.LogWarning(logText);
                    break;
                }
                case "Info":
                {
                    _logger.LogInformation(logText);
                    break;
                }
                case "Verbose":
                {
                    _logger.LogInformation(logText);
                    break;
                } 
                case "Debug":
                {
                    _logger.LogDebug(logText);
                    break;
                } 
                case "Error":
                {
                    _logger.LogError(logText);
                    break;
                } 
            }

            return Task.CompletedTask; 

        }
    }
}
