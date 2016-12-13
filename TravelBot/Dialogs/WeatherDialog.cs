using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Luis.Models;

namespace TravelBot.Dialogs
{
    [Serializable]
    public class WeatherDialog : IDialog<Weather>
    {
        private static string luisCity = "builtin.geography.city";
        private static string luisDate = "builtin.datetime.date";

        Weather weather;

        //public WeatherDialog(List<EntityRecommendation> entities)
        //{
        //    weather = new Weather();
        //    foreach (var entity in entities)
        //    {
        //        if (entity.Type == luisCity)
        //        {
        //            weather.Location = entity.Entity;
        //        }
        //        else if (entity.Type == luisDate)
        //        {
        //            weather.Date = entity.Entity;
        //        }
        //    }
        //}

        public async Task StartAsync(IDialogContext context)
        {
            weather = new Weather();
            if (weather.Location == null && weather.Date == null)
            {
                //await context.PostAsync("Please enter a US city to get the weather for.");
                //context.Wait(GetLocAndDate);
                PromptDialog.Text(context, GetLocAndDate, "Enter a city");
            }
            else if (weather.Location == null && weather.Date != null)
            {
                await context.PostAsync("Please enter a US city to get the weather for.");
                context.Wait(LocBackToRoot);
            }
            else if (weather.Date == null && weather.Location != null)
            {
                await context.PostAsync("Please enter the date you want to get the weather for.");
                context.Wait(DateBackToRoot);
            }
            else
            {
                context.Done(weather);
            }
        }

        private async Task GetLocAndDate(IDialogContext context, IAwaitable<string> result)
        {
            var temp = await result;
            //weather.Location = temp.Text;
            await context.PostAsync("Please enter the date you want to get the weather for.");
            context.Wait(DateBackToRoot);
        }

        private async Task LocBackToRoot(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var temp = await result;
            weather.Location = temp.Text;
            context.Done(weather);
        }

        private async Task DateBackToRoot(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var temp = await result;
            weather.Date = temp.Text;
            context.Done(weather);
        }
    }

    public class Weather
    {
        public string Location { get; set; }
        public string Date { get; set; }
    }
}