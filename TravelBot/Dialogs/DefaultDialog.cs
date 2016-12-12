﻿using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System.Web.Configuration;
using System.Net.Http;
using System.Web;
using TravelBot.JsonDeserialization;
using Newtonsoft.Json;

namespace TravelBot.Dialogs
{
    [Serializable]
    [LuisModel("138ad2b9-9738-4871-ba7a-93c1121aff70", "efe598955c1e4f4f93a89e018d468e0a")]
    public class DefaultDialog : LuisDialog<object>
    {
        private static string luisCountry = "builtin.geography.country";
        private static string luisState = "builtin.geography.us_state";
        private static string luisCity = "builtin.geography.city";

        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Sorry, I don't understand what you said.");
            context.Wait(MessageReceived);
        }

        [LuisIntent("Greeting")]
        public async Task Greeting(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Hello! How can I assist you with your travels today?\n" +
                                        "- Search for a travel destinatation\n" +
                                        "- Check the weather in a location\n" +
                                        "- Check the current news in a location\n");
            context.Wait(MessageReceived);
        }

        [LuisIntent("FindDesitnation")]
        public async Task FindDestination(IDialogContext context, LuisResult result)
        {
            var location = GetLocation(new List<EntityRecommendation>(result.Entities));

            // If the user specified a location
            if (location != null)
            {
                await context.PostAsync("You want to travel to " + location.Entity + "!");

                var destinations = await CallSearchAPI(location.Entity, "destination");

                // Create and display the news results
                //await context.PostAsync(MakeNewsCards(context, (NewsResult)news));
                await context.PostAsync("I have found " + (DestinationResult)destinations);
                context.Wait(MessageReceived);
            }
            else // The user needs to enter a location before getting destination suggestions
            {
                var prompt = "What country, state, or city would you like to find travel suggestions for?";
                PromptDialog.Text(context, HandleDestinationSearch, prompt);
            }
        }

        private async Task HandleDestinationSearch(IDialogContext context, IAwaitable<string> result)
        {
            var location = result.GetAwaiter().GetResult().ToString();
            await context.PostAsync("I am handling your request to search for destinations near " + location + ".");

            // Now that we have the location to search for news about, call the API
            var destinations = await CallSearchAPI(location, "destination");

            // Create and display the news results
            //await context.PostAsync(MakeNewsCards(context, (NewsResult)news));
            await context.PostAsync("I have found " + (DestinationResult)destinations);
            context.Wait(MessageReceived);
        }

        [LuisIntent("GetNews")]
        public async Task GetNews(IDialogContext context, LuisResult result)
        {
            var location = GetLocation(new List<EntityRecommendation>(result.Entities));
            string prompt;

            // If the user specified a location
            if (location != null)
            {
                // Now that we have the location to search for news about, call the API
                var news = await CallSearchAPI(location.Entity, "news");

                // Create and display the news results
                await context.PostAsync(MakeNewsCards(context, (NewsResult)news));
                context.Wait(MessageReceived);
            }
            else // The user needs to enter a location before getting destination suggestions
            {
                prompt = "What country, state, or city would you like to find news for?";
                PromptDialog.Text(context, GetNewsLocation, prompt);
            }
        }

        private async Task GetNewsLocation(IDialogContext context, IAwaitable<string> result)
        {
            var location = result.GetAwaiter().GetResult().ToString();
            await context.PostAsync("I am handling your request to get news from " + location + ".");

            // Now that we have the location to search for news about, call the API
            var news = await CallSearchAPI(location, "news");

            // Create and display the news results
            await context.PostAsync(MakeNewsCards(context, (NewsResult)news));
            context.Wait(MessageReceived);
        }

        private async Task<object> CallSearchAPI(string location, string searchType)
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", WebConfigurationManager.AppSettings["CogServices:NewsSearch:ID"]);

            // Request parameters
            queryString["count"] = "4";
            queryString["offset"] = "0";
            queryString["mkt"] = "en-US";
            queryString["safeSearch"] = "Moderate";
            string uri = null;
            if (searchType == "news")
            {
                queryString["q"] = "Travel related news about " + location;
                uri = "https://api.cognitive.microsoft.com/bing/v5.0/news/search?" + queryString;

            }
            else if (searchType == "destination")
            {
                queryString["q"] = "Travel desitnations in " + location;
                uri = "https://api.cognitive.microsoft.com/bing/v5.0/search?" + queryString;
            }
            var response = await client.GetAsync(uri);

            HttpContent resultContent = response.Content;
            var result = await resultContent.ReadAsStringAsync();

            // Populate a JSON object with the results of the API call
            if (searchType == "news")
            {
                NewsResult news = new NewsResult();
                JsonConvert.PopulateObject(result, news);
                return news;
            }
            else if (searchType == "destination")
            {
                DestinationResult destination = new DestinationResult();
                JsonConvert.PopulateObject(result, destination);
                return destination;
            }

            return null;
        }

        private IMessageActivity MakeNewsCards(IDialogContext context, NewsResult news)
        {
            var resultMessage = context.MakeMessage();
            resultMessage.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            resultMessage.Attachments = new List<Attachment>();
            foreach (var item in news.value)
            {
                ThumbnailCard thumbnailCard = new ThumbnailCard()
                {
                    Title = item.name,
                    Text = item.description,
                    Images = new List<CardImage>()
                    {
                        new CardImage() { Url = item.image.thumbnail.contentUrl }
                    },
                    Tap = new CardAction(item.url),
                };

                resultMessage.Attachments.Add(thumbnailCard.ToAttachment());
            }

            return resultMessage;
        }

        [LuisIntent("GetWeather")]
        public async Task GetWeather(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Checking the weather...");
            context.Wait(MessageReceived);
        }

        private EntityRecommendation GetLocation(List<EntityRecommendation> entities)
        {
            // Find the location the user specified
            EntityRecommendation location = null;
            if (entities.Any((entity) => entity.Type == luisCountry))
            {
                location = entities.Where((entity) => entity.Type == luisCountry).First();
            }
            else if (entities.Any((entity) => entity.Type == luisState))
            {
                location = entities.Where((entity) => entity.Type == luisState).First();
            }
            else if (entities.Any((entity) => entity.Type == luisCity))
            {
                location = entities.Where((entity) => entity.Type == luisCity).First();
            }

            return location;
        }
    }
}