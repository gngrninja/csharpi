using Discord;
using Discord.Net;
using Discord.WebSocket;
using Discord.Commands;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using csharpi.Database;
using Microsoft.Extensions.DependencyInjection;

namespace csharpi.Modules
{
    // for commands to be available, and have the Context passed to them, we must inherit ModuleBase
    public class EightBallCommands : ModuleBase
    {
        private readonly CsharpiEntities _db;
        private List<String> _validColors = new List<String>();
        private readonly IConfiguration _config;

        public EightBallCommands(IServiceProvider services)
        {
            // we can pass in the db context via depedency injection
            _db = services.GetRequiredService<CsharpiEntities>();
            _config = services.GetRequiredService<IConfiguration>();

            _validColors.Add("green");
            _validColors.Add("red");
            _validColors.Add("blue");
        }

        [Command("add")]
        public async Task AddResponse(string answer, string color)
        {            
            var sb = new StringBuilder();
            var embed = new EmbedBuilder();

            // get user info from the Context
            var user = Context.User;
            
            // check to see if the color is valid
            if (!_validColors.Contains(color.ToLower()))
            {
                sb.AppendLine($"**Sorry, [{user.Username}], you must specify a valid color.**");
                sb.AppendLine("Valid colors are:");
                sb.AppendLine();
                foreach (var validColor in _validColors)
                {
                    sb.AppendLine($"{validColor}");
                }       
                embed.Color = new Color(255, 0, 0);         
            }
            else 
            {
                // add answer/color to table
                await _db.AddAsync(new EightBallAnswer
                    {
                        AnswerText  = answer,
                        AnswerColor = color.ToLower()                     
                    }
                );
                // save changes to database
                await _db.SaveChangesAsync();                
                sb.AppendLine();
                sb.AppendLine("**Added answer:**");
                sb.AppendLine(answer);
                sb.AppendLine();
                sb.AppendLine("**With color:**");
                sb.AppendLine(color);
                embed.Color = new Color(0, 255, 0);  
            }

            // set embed
            embed.Title = "Eight Ball Answer Addition";
            embed.Description = sb.ToString();
            
            // send embed reply
            await ReplyAsync(null, false, embed.Build());
        }

        [Command("list")]
        public async Task ListAnswers()
        {            
            var sb = new StringBuilder();
            var embed = new EmbedBuilder();

            // get user info from the Context
            var user = Context.User;
            
            var answers = await _db.EightBallAnswer.ToListAsync();
            if (answers.Count > 0)
            {
                foreach (var answer in answers)
                {
                    sb.AppendLine($":small_blue_diamond: [{answer.AnswerId}] **{answer.AnswerText}**");
                }
            }
            else
            {
                sb.AppendLine("No answers found!");
            }

            // set embed
            embed.Title = "Eight Ball Answer List";
            embed.Description = sb.ToString();
            
            // send embed reply
            await ReplyAsync(null, false, embed.Build());
        }   

        [Command("remove")]
        public async Task RemoveAnswer(int id)
        {            
            var sb = new StringBuilder();
            var embed = new EmbedBuilder();

            // get user info from the Context
            var user = Context.User;
            
            var answers = await _db.EightBallAnswer.ToListAsync();
            var answerToRemove = answers.Where(a => a.AnswerId == id).FirstOrDefault();

            if (answerToRemove != null)
            {
                _db.Remove(answerToRemove);
                await _db.SaveChangesAsync();
                sb.AppendLine($"Removed answer -> [{answerToRemove.AnswerText}]");
            }
            else
            {
                sb.AppendLine($"Did not find answer with id [**{id}**] in the database");
                sb.AppendLine($"Perhaps use the {_config["prefix"]}list command to list out answers");
            }
            
            // set embed
            embed.Title = "Eight Ball Answer List";
            embed.Description = sb.ToString();
            
            // send embed reply
            await ReplyAsync(null, false, embed.Build());
        } 

        [Command("8ball")]
        [Alias("ask")]
        public async Task AskEightBall([Remainder]string args = null)
        {
            // I like using StringBuilder to build out the reply
            var sb = new StringBuilder();

            // let's use an embed for this one!
            var embed = new EmbedBuilder();
            
            // add our possible replies from the database
            var replies = await _db.EightBallAnswer.ToListAsync();

            // add a title                        
            embed.Title = "Welcome to the 8-ball!";
            
            // we can get lots of information from the Context that is passed into the commands
            // here I'm setting up the preface with the user's name and a comma
            sb.AppendLine($"{Context.User.Username},");
            sb.AppendLine();

            // let's make sure the supplied question isn't null 
            if (args == null)
            {
                // if no question is asked (args are null), reply with the below text
                sb.AppendLine("Sorry, can't answer a question you didn't ask!");
            }
            else 
            {
                // if we have a question, let's give an answer!
                // get a random number to index our list with 
                var answer = replies[new Random().Next(replies.Count)];
                
                // build out our reply with the handy StringBuilder
                sb.AppendLine($"You asked: [**{args}**]...");
                sb.AppendLine();
                sb.AppendLine($"...your answer is [**{answer.AnswerText}**]");

                switch (answer.AnswerColor)
                {
                    case "red":
                    {
                        embed.WithColor(255, 0, 0);
                        break;
                    }
                    case "blue":
                    {
                        embed.WithColor(0, 0, 255);
                        break;
                    }
                    case "green":
                    {
                        embed.WithColor(0, 255, 0);
                        break;
                    }                                        
                }                               
            }

            // now we can assign the description of the embed to the contents of the StringBuilder we created
            embed.Description = sb.ToString();

            // this will reply with the embed
            await ReplyAsync(null, false, embed.Build());
        }
    }
}