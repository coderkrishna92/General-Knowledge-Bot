// <copyright file="GenKnowledgeBot.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace GeneralKnowledgeBot.Bots
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using GeneralKnowledgeBot.Models;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Schema;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    public class GenKnowledgeBot : ActivityHandler
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<GenKnowledgeBot> logger;

        public GenKnowledgeBot(IConfiguration configuration, ILogger<GenKnowledgeBot> logger)
        {
            this.configuration = configuration;
            this.logger = logger;
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var isQuery = turnContext.Activity.Text.EndsWith('?') || turnContext.Activity.Text.EndsWith('.');
            if (isQuery)
            {
                var uri = this.configuration["KbHost"] + this.configuration["Service"] + "/knowledgebases/" + this.configuration["KbID"] + "/generateAnswer";
                var question = turnContext.Activity.Text;

                this.logger.LogInformation("Calling QnA Maker");

                using (var client = new HttpClient())
                using (var request = new HttpRequestMessage())
                {
                    request.Method = HttpMethod.Post;
                    request.RequestUri = new Uri(uri);
                    request.Content = new StringContent("{'question': '" + question + "'}", Encoding.UTF8, "application/json");
                    request.Headers.Add("Authorization", "EndpointKey " + this.configuration["EndpointKey"]);

                    var response = await client.SendAsync(request);
                    var responseText = await response.Content.ReadAsStringAsync();

                    var responseModel = JsonConvert.DeserializeObject<Response>(responseText);

                    if (responseModel != null)
                    {
                        // TODO: Convert this into a separate method
                        // Parameters: turnContext, cancellationToken, responseModel
                        // Internal logic: Making sure to have the adaptive cards as well
                        await GenKBot.SendAnswerMessage(turnContext, cancellationToken, responseModel.answers[0].answer, question);
                    }
                    else
                    {
                        await turnContext.SendActivityAsync(MessageFactory.Text("No QnA Maker answers were found."), cancellationToken);
                    }
                }
            }
            else if (turnContext.Activity.Text == "Welcome Message")
            {
                var botDisplayName = this.configuration["BotDisplayName"];
                await GenKBot.SendUserWelcomeMessage(turnContext, cancellationToken, botDisplayName);
            }
            else
            {
                await GenKBot.SendUnrecognizedInputMessage(turnContext, cancellationToken);
            }
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    var botDisplayName = this.configuration["BotDisplayName"];
                    await GenKBot.SendProactiveWelcomeMessage(turnContext, cancellationToken, botDisplayName);
                }
            }
        }
    }
}