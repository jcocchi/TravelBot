using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Luis.Models;
using System.Text.RegularExpressions;
using Microsoft.Bot.Builder.Dialogs.Internals;
using TravelBot.App_Code;
using static TravelBot.Dialogs.WeatherDialog;

namespace TravelBot.Dialogs
{
    [Serializable]
    public class WeatherDialog : IDialog<Weather>
    {
        private static readonly string defaultDate = "1/1/0001 12:00:00 AM";
        private static readonly string datePrompt = "Please enter the date you want to get the weather for in MM/DD format.";
        private static readonly string locationPrompt = "Please enter a US city to get the weather for.";

        Weather weather;

        public WeatherDialog(Weather weather)
        {
            this.weather = weather;
        }

        public async Task StartAsync(IDialogContext context)
        {
            if (weather.Location == null && weather.Date.ToString() == defaultDate)
            {
                await context.PostAsync(locationPrompt);
                context.Wait(GetLocAndDate);
            }
            else if (weather.Location == null && weather.Date.ToString() != defaultDate)
            {
                await context.PostAsync(locationPrompt);
                context.Wait(LocBackToRoot);
            }
            else if (weather.Date.ToString() == defaultDate && weather.Location != null)
            {
                await context.PostAsync(datePrompt);
                context.Wait(DateBackToRoot);
            }
            else
            {
                context.Done(weather);
            }
        }

        private async Task GetLocAndDate(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var temp = await result;

            // Verify the user isn't trying to return to the main dialog
            if (temp.Text.Contains("help"))
            {
                context.Done("help");
            }
            else
            {
                weather.Location = temp.Text;
                await context.PostAsync(datePrompt);
                context.Wait(DateBackToRoot);
            }
        }

        private async Task LocBackToRoot(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var temp = await result;

            // Verify the user isn't trying to return to the main dialog
            if (temp.Text.Contains("help"))
            {
                context.Done("help");
            }
            else
            {
                weather.Location = temp.Text;
                context.Done(weather);
            }
        }

        private async Task DateBackToRoot(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var temp = await result;

            // Verify the user isn't trying to return to the main dialog
            if (temp.Text.Contains("help"))
            {
                context.Done("help");
            }
            else
            {
                var dateTime = new DateTime();

                if (DateTime.TryParse(temp.Text, out dateTime))
                {
                    weather.Date = dateTime.Date;
                    context.Done(weather);
                }
                else
                {
                    await context.PostAsync("Oops! That was an invalid date. Please enter the date you want to get the weather for in MM/DD format.");
                    context.Wait(DateBackToRoot);
                }
            }
        }
    }
}