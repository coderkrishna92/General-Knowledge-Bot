﻿// <copyright file="WelcomeTeamAdaptiveCard.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace GeneralKnowledgeBot.Helpers.AdaptiveCards
{
    using System.Collections.Generic;
    using System.IO;
    using GeneralKnowledgeBot.Properties;

    /// <summary>
    /// The class for the adaptive card to be shown in a team.
    /// </summary>
    public class WelcomeTeamAdaptiveCard
    {
        private static readonly string CardTemplate;

        /// <summary>
        /// Initializes static members of the <see cref="WelcomeTeamAdaptiveCard"/> class.
        /// </summary>
        static WelcomeTeamAdaptiveCard()
        {
            var cardJsonFilePath = Path.Combine(".", "Helpers", "AdaptiveCards", "WelcomeTeamAdaptiveCard.json");
            CardTemplate = File.ReadAllText(cardJsonFilePath);
        }

        /// <summary>
        /// Method that returns the welcome team adaptive card.
        /// </summary>
        /// <param name="botDisplayName">The bot display name.</param>
        /// <param name="teamName">The team name.</param>
        /// <returns>The JSON string for the adaptive card.</returns>
        public static string GetCard(string botDisplayName, string teamName)
        {
            var welcomeTeamCardTitleText = string.Format(Resource.WelcomeTeamCardTitleText, teamName);
            var welcomeTeamCardContent = string.Format(Resource.WelcomeTeamCardContent, botDisplayName, teamName);
            var teamTourIntroText = Resource.TeamTourIntroText;
            var takeATeamTourButtonText = Resource.TakeATeamTourButtonText;

            var variablesToValues = new Dictionary<string, string>()
            {
                { "welcomeTeamCardTitleText", welcomeTeamCardTitleText },
                { "welcomeTeamCardContent", welcomeTeamCardContent },
                { "teamTourIntroText", teamTourIntroText },
                { "takeATeamTourButtonText", takeATeamTourButtonText },
            };

            var cardBody = CardTemplate;
            foreach (var kvp in variablesToValues)
            {
                cardBody = cardBody.Replace($"%{kvp.Key}%", kvp.Value);
            }

            return cardBody;
        }
    }
}