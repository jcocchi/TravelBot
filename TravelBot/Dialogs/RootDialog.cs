using System;
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
using Newtonsoft.Json;
using System.Threading;
using TravelBot.App_Code;
using static TravelBot.Dialogs.WeatherDialog;

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
        private static readonly string LocationKey = "LocKey";

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
                                        "- Check the current news in a location\n" +
                                        "- Check the weather in a location for a specific date\n");
            context.Wait(MessageReceived);
        }

        [LuisIntent("ThankYou")]
        public async Task ThankYou(IDialogContext context, LuisResult result)
        {

            await context.PostAsync("You're welcome! To see what else I can help you with type \"help\".");
            context.Wait(MessageReceived);
        }

        [LuisIntent("FindDestination")]
        public async Task FindDestination(IDialogContext context, LuisResult result)
        {
            // Did the user enter a location?
            var location = GetLocation(context, new List<EntityRecommendation>(result.Entities));

            // If the user didn't enter a location this time, have they entered one previously in this conversation?
            if (location != null)
            {
                // We have all entities we need because the action was triggered
                // Get the location and call the API
                await context.PostAsync(String.Format("Here are some fun touristy things to do in {0}! Click each card to learn more.", location));
                var destination = await CallSearchAPI(location, "destination");

                // Create and display the news results
                await context.PostAsync(MakeDestinationCards(context, (DestinationResult)destination));
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
            var destinations = await CallSearchAPI(location, "destination");

            // Create and display the destination results
            await context.PostAsync(MakeDestinationCards(context, (DestinationResult)destinations));
            context.Wait(MessageReceived);
        }

        private IMessageActivity MakeDestinationCards(IDialogContext context, DestinationResult dest)
        {
            var resultMessage = context.MakeMessage();
            resultMessage.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            resultMessage.Attachments = new List<Attachment>();
            foreach (var item in dest.webPages.value)
            {
                var action = new CardAction();
                action.Type = "openUrl";
                action.Value = item.url;
                HeroCard heroCard = new HeroCard()
                {
                    Title = item.name,
                    Text = item.snippet,
                    Tap = action,
                };

                resultMessage.Attachments.Add(heroCard.ToAttachment());
            }

            return resultMessage;
        }

        [LuisIntent("GetNews")]
        public async Task GetNews(IDialogContext context, LuisResult result)
        {
            var location = GetLocation(context, new List<EntityRecommendation>(result.Entities));
            if (location != null)
            {
                // We have all entities we need so tell the user and call the API
                await context.PostAsync("I am handling your request to get news from " + location + ".");
                var news = await CallSearchAPI(location, "news");

                // Create and display the news results
                await context.PostAsync(MakeNewsCards(context, (NewsResult)news));
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
            var news = await CallSearchAPI(location, "news");

            // Create and display the news results
            await context.PostAsync(MakeNewsCards(context, (NewsResult)news));
            context.Wait(MessageReceived);
        }

        private IMessageActivity MakeNewsCards(IDialogContext context, NewsResult news)
        {
            var resultMessage = context.MakeMessage();
            resultMessage.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            resultMessage.Attachments = new List<Attachment>();
            foreach (var item in news.value)
            {
                var action = new CardAction();
                action.Type = "openUrl";
                action.Value = item.url;
                HeroCard heroCard = new HeroCard()
                {
                    Title = item.name,
                    Text = item.description,
                    Images = new List<CardImage>()
                    {
                        new CardImage() { Url = item.image.thumbnail.contentUrl }
                    },
                    Tap = action,
                };

                resultMessage.Attachments.Add(heroCard.ToAttachment());
            }

            return resultMessage;
        }

        [LuisIntent("GetWeather")]
        public async Task GetWeather(IDialogContext context, LuisResult result)
        {
            var entities = new List<EntityRecommendation>(result.Entities);

            // We are only looking at the first intent in the array because that represents this function
            // We know there is only one action for this function to search the weather
            var date = GetDate(entities);
            var location = GetLocation(context, new List<EntityRecommendation>(result.Entities));
            var givenVals = new Weather()
            {
                Date = date,
                Location = location
            };
            if (location != null && date.ToString() != defaultDate)
            {
                await context.PostAsync("I am handling your weather search in " + location + " on " + date.ToShortDateString());

                // Call API and determine which card to build
                var weather = await CallWeatherAPI(location, date);
                if (weather.GetType().Name == "TenDayForecastResult")
                {
                    await context.PostAsync(MakeWeatherCards(context, (TenDayForecastResult)weather, givenVals));
                }
                else
                {
                    await context.PostAsync(MakeWeatherCards(context, (AverageForecastResult)weather, givenVals));
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

        private IMessageActivity MakeWeatherCards(IDialogContext context, TenDayForecastResult weather, Weather givenVals)
        {
            var resultMessage = context.MakeMessage();
            resultMessage.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            resultMessage.Attachments = new List<Attachment>();

            // Find the correct date returned from the API and make the card
            var result = weather.forecast.simpleforecast.forecastday.Where(f => f.date.day == givenVals.Date.Day).FirstOrDefault();
            HeroCard heroCard = new HeroCard()
            {
                Title = "WEATHER IN " + givenVals.Location.ToUpper(),
                Text = String.Format("The high for {0}/{1} is {2} degrees F and the low is {3} degrees F. Overall conditions are {4}.", 
                                        result.date.month, result.date.day, result.high.fahrenheit, result.low.fahrenheit, result.conditions.ToLower()),
            };
            resultMessage.Attachments.Add(heroCard.ToAttachment());

            return resultMessage;
        }

        private IMessageActivity MakeWeatherCards(IDialogContext context, AverageForecastResult weather, Weather givenVals)
        {
            var resultMessage = context.MakeMessage();
            resultMessage.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            resultMessage.Attachments = new List<Attachment>();
            HeroCard heroCard = new HeroCard()
            {
                Title = "WEATHER IN " + givenVals.Location.ToUpper(),
                Text = String .Format("The average high for {0} is {1} degrees F and the average low is {2} degrees F.", 
                           weather.trip.period_of_record.date_start.date.monthname, weather.trip.temp_high.avg.F, weather.trip.temp_low.avg.F),
            };
            resultMessage.Attachments.Add(heroCard.ToAttachment());

            return resultMessage;
        }

        private async Task HandleWeatherSearch(IDialogContext context, IAwaitable<Weather> result)
        {
            var weather = await result;
            await context.PostAsync("I am handling your weather search in " + weather.Location + " on " + weather.Date.ToShortDateString());

            var res = await CallWeatherAPI(weather.Location, weather.Date);

            if (res.GetType().Name == "TenDayForecastResult")
            {
                await context.PostAsync(MakeWeatherCards(context, (TenDayForecastResult)res, weather));
            }
            else
            {
                await context.PostAsync(MakeWeatherCards(context, (AverageForecastResult)res, weather));
            }

            context.Wait(MessageReceived);
        }

        private string GetLocation(IDialogContext context, List<EntityRecommendation> entities)
        {
            // Find the location the user specified
            string location = null;
            if (entities.Any((entity) => entity.Type == luisCountry))
            {
                location = entities.Where((entity) => entity.Type == luisCountry).First().Entity;
            }
            else if (entities.Any((entity) => entity.Type == luisState))
            {
                location = entities.Where((entity) => entity.Type == luisState).First().Entity;
            }
            else if (entities.Any((entity) => entity.Type == luisCity))
            {
                location = entities.Where((entity) => entity.Type == luisCity).First().Entity;
            }
            // If the user didn't specifiy a location this time, is there a location already stored for them in this conversation?
            else
            {
                context.ConversationData.TryGetValue(LocationKey, out location);
            }

            // Reset location for the conversation and send back to dialog
            // If the user didn't enter a location and hadn't in the past, this will be set to null
            // If the user entered a new location, this will be set to that location
            // If the user didn't enter a location this time, but had in the past, this will be reset to the preexisting value
            context.ConversationData.SetValue(LocationKey, location);
            return location;
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

        private async Task<object> CallWeatherAPI(string location, DateTime date)
        {
            // Format the location
            var formatLoc = location.ToLower();
            formatLoc = formatLoc.Replace(" ", "_");

            // Decide which API to call
            string type;
            var uri = "http://api.wunderground.com/api/99739e85768e55e2/";
            var futureDate = DateTime.UtcNow.AddDays(10);
            if (date <= futureDate)
            {
                uri += String.Format("forecast10day/q/CA/{0}.json", formatLoc);
                type = "10day";
            }
            else
            {
                // Determine month1 and day1 format
                var month1 = date.Month.ToString();
                var day1 = date.Day.ToString();
                if(date.Month < 10)
                {
                    month1 = "0" + month1;
                }
                if (date.Day < 10)
                {
                    day1 = "0" + day1;
                }
                // Determine month2 and day2 format
                var date2 = date.AddDays(29);
                var month2 = date2.Month.ToString();
                var day2 = date2.Day.ToString();
                if (date2.Month < 10)
                {
                    month2 = "0" + month2;
                }
                if (date2.Day < 10)
                {
                    day2 = "0" + day2;
                }

                // Determine date range
                var range = String.Format("{0}{1}{2}{3}", 
                                month1, day1, month2, day2);
                uri += String.Format("planner_{0}/q/CA/{1}.json", range, formatLoc);
                type = "average";
            }

            var client = new HttpClient();
            var response = await client.GetAsync(uri);

            HttpContent resultContent = response.Content;
            var result = await resultContent.ReadAsStringAsync();

            if (type == "10day")
            {
                TenDayForecastResult forecast = new TenDayForecastResult();
                JsonConvert.PopulateObject(result, forecast);
                return forecast;
            }
            else
            {
                AverageForecastResult forecast = new AverageForecastResult();
                JsonConvert.PopulateObject(result, forecast);
                return forecast;
            }
        }
    }
}