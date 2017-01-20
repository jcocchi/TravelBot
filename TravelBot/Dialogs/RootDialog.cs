using System;
using System.Linq;
using System.Net.Http;
using TravelBot.App_Code;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis.Models;

namespace TravelBot.Dialogs
{
    [Serializable]
    [LuisModel("138ad2b9-9738-4871-ba7a-93c1121aff70", "efe598955c1e4f4f93a89e018d468e0a")]
    public class RootDialog : LuisDialog<object>
    {
        private static readonly string luisCountry = "builtin.geography.country";
        private static readonly string luisState = "builtin.geography.us_state";
        private static readonly string luisCity = "builtin.geography.city";
        private static readonly string luisDate = "builtin.datetime.date";
        private static readonly string defaultDate = "1/1/0001 12:00:00 AM";
        private MakeCards card = new MakeCards();
        private MakeAPICalls api = new MakeAPICalls();

        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Sorry, I don't understand what you said. Type \"help\" for a list of what I can help you with!");
            context.Wait(MessageReceived);
        }

        [LuisIntent("Greeting")]
        public async Task Greeting(IDialogContext context, LuisResult result)
        {

            await context.PostAsync("Hello! How can I assist you with your travels today?\n" +
                                        "- Search for a travel destination\n" +
                                        "- Check the weather in a location for a specific date\n" +
                                        "- Check the current news in a location\n");
            context.Wait(MessageReceived);
        }

        [LuisIntent("ThankYou")]
        public async Task ThankYou(IDialogContext context, LuisResult result)
        {

            await context.PostAsync("You're welcome! To see what else I can help you with type \"help\".");
            context.Wait(MessageReceived);
        }

        [LuisIntent("FindDesitnation")]
        public async Task FindDestination(IDialogContext context, LuisResult result)
        {
            var location = GetLocation(new List<EntityRecommendation>(result.Entities));
            if (location != null)
            {
                // We have all entities we need because the action was triggered
                // Get the location and call the API
                await context.PostAsync(String.Format("Here are some fun touristy things to do in {0}! Click each card to learn more.", location));
                var destination = await api.CallSearchAPI(location, "destination");

                // Create and display the news results 
                await context.PostAsync(card.MakeDestinationCards(context, (DestinationResult)destination));
                context.Wait(MessageReceived);
            }
            else
            {
                // We need to prompt the user for a location
                var prompt = "What country, state, or city would you like to find travel suggestions for?";
                PromptDialog.Text(context, HandleDestinationSearch, prompt);
            }
        }

        private async Task HandleDestinationSearch(IDialogContext context, IAwaitable<string> result)
        {
            var location = await result;
            await context.PostAsync("I am handling your request to search for destinations near " + location + "...");

            // Now that we have the location to search for news about, call the API
            var destinations = await api.CallSearchAPI(location, "destination");

            // Create and display the destination results
            await context.PostAsync(card.MakeDestinationCards(context, (DestinationResult)destinations));
            context.Wait(MessageReceived);
        }

        [LuisIntent("GetNews")]
        public async Task GetNews(IDialogContext context, LuisResult result)
        {
            var location = GetLocation(new List<EntityRecommendation>(result.Entities));
            if (location != null)
            {
                // We have all entities we need so tell the user and call the API
                await context.PostAsync("I am handling your request to get news from " + location + ".");
                var news = await api.CallSearchAPI(location, "news");

                // Create and display the news results
                await context.PostAsync(card.MakeNewsCards(context, (NewsResult)news));
                context.Wait(MessageReceived);
            }
            else
            {
                // We need to prompt the user for a location
                var prompt = "What country, state, or city would you like to find news for?";
                PromptDialog.Text(context, HandleNewsSearch, prompt);
            }
        }

        private async Task HandleNewsSearch(IDialogContext context, IAwaitable<string> result)
        {
            var location = await result;
            await context.PostAsync("I am handling your request to get news from " + location + ".");

            // Now that we have the location to search for news about, call the API
            var news = await api.CallSearchAPI(location, "news");

            // Create and display the news results
            await context.PostAsync(card.MakeNewsCards(context, (NewsResult)news));
            context.Wait(MessageReceived);
        }

        [LuisIntent("GetWeather")]
        public async Task GetWeather(IDialogContext context, LuisResult result)
        {
            var entities = new List<EntityRecommendation>(result.Entities);

            // We are only looking at the first intent in the array because that represents this function
            // We know there is only one action for this function to search the weather
            var date = GetDate(entities);
            var location = GetLocation(new List<EntityRecommendation>(result.Entities));
            var givenVals = new Weather()
            {
                Date = date,
                Location = location
            };
            if (location != null && date.ToString() != defaultDate)
            {
                await context.PostAsync("I am handling your weather search in " + location + " on " + date.ToShortDateString());

                // Call API and determine which card to build
                var weather = await api.CallWeatherAPI(location, date);
                if (weather.GetType().Name == "TenDayForecastResult")
                {
                    await context.PostAsync(card.MakeWeatherCards(context, (TenDayForecastResult)weather, givenVals));
                }
                else
                {
                    await context.PostAsync(card.MakeWeatherCards(context, (AverageForecastResult)weather, givenVals));
                }

                // Create and display the news results
                context.Wait(MessageReceived);
            }
            else
            {
                // We need to prompt the user for a location, a date or both
                context.Call<Weather>(new WeatherDialog(givenVals), HandleWeatherSearch); 
            }
        }

        private async Task HandleWeatherSearch(IDialogContext context, IAwaitable<Weather> result)
        {
            var weather = await result;
            await context.PostAsync("I am handling your weather search in " + weather.Location + " on " + weather.Date.ToShortDateString());

            var res = await api.CallWeatherAPI(weather.Location, weather.Date);

            if (res.GetType().Name == "TenDayForecastResult")
            {
                await context.PostAsync(card.MakeWeatherCards(context, (TenDayForecastResult)res, weather));
            }
            else
            {
                await context.PostAsync(card.MakeWeatherCards(context, (AverageForecastResult)res, weather));
            }

            context.Wait(MessageReceived);
        }

        private string GetLocation(List<EntityRecommendation> entities)
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
            else
            {
                return null;
            }

            return location.Entity;
        }

        private DateTime GetDate(List<EntityRecommendation> entities)
        {
            // Find the date the user specified
            EntityRecommendation dateEntity = null;
            DateTime date = new DateTime();
            if (entities.Any((entity) => entity.Type == luisDate))
            {
                dateEntity = entities.Where((entity) => entity.Type == luisDate).First();
                DateTime.TryParse(dateEntity.Entity, out date);
                // The user said something like "tomorrow" or "in two months"
                if (date.ToString() == defaultDate)
                {
                    DateTime.TryParse(dateEntity.Resolution.Values.FirstOrDefault(), out date);
                }
                // If the date is still the default date, the user said a month name like "May"
                if (date.ToString() == defaultDate)
                {
                    // Right now the date looks like "XXXX-04", we only want the month number so trim the rest
                    var trimMonth = dateEntity.Resolution.Values.FirstOrDefault().Remove(0, 5);
                    // Make the date the first of the month that the user entered
                    date = new DateTime(DateTime.UtcNow.Year, Int32.Parse(trimMonth), 1);
                }
                // If the user enters a date in the future, but doesn't specify the year it will be assumed to be this year
                // Because travel happens in the future not the past, add a year to account for this
                if (date < DateTime.UtcNow)
                {
                    date = date.AddYears(1);
                }
            }

            return date;
        }
    }
}