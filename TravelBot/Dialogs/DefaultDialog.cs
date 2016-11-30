using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

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
            await context.PostAsync("Hello! How can I assist you with your travels today?");
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
            } 
            else // The user needs to enter a location before getting destination suggestions
            {
                //await context.PostAsync("Please include a country, state, or city when asking for travel destinations.");
                await context.Forward(new EnterLocationDialog(), HandleDestinationSearch, location, new CancellationToken());
                

                //context.Call(new EnterLocationDialog(), HandleDestinationSearch);
            }
            context.Wait(MessageReceived);
        }

        private async Task HandleDestinationSearch(IDialogContext context, IAwaitable<object> result)
        {
            await context.PostAsync("I am handling your request");
            context.Wait(MessageReceived);
        }

        [LuisIntent("GetNews")]
        public async Task GetNews(IDialogContext context, LuisResult result)
        {
            var location = GetLocation(new List<EntityRecommendation>(result.Entities));

            // If the user specified a location
            if (location != null)
            {
                await context.PostAsync("You want news from " + location.Entity + "!");
            }
            else // The user needs to enter a location before getting destination suggestions
            {
                await context.PostAsync("Please include a country, state, or city when searching for news.");
            }

            // handle request

            context.Wait(MessageReceived);
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