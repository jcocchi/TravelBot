//using Microsoft.Bot.Builder.Dialogs;
//using Microsoft.Bot.Connector;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;

//namespace TravelBot.Dialogs
//{
//    [Serializable]
//    internal class MakeCards
//    {
//        public MakeCards() { } 

//        public IMessageActivity MakeDestinationCards(IDialogContext context, DestinationResult dest)
//        {
//            var resultMessage = context.MakeMessage();
//            resultMessage.AttachmentLayout = AttachmentLayoutTypes.Carousel;
//            resultMessage.Attachments = new List<Attachment>();
//            foreach (var item in dest.webPages.value)
//            {
//                var action = new CardAction();
//                action.Type = "openUrl";
//                action.Value = item.url;
//                HeroCard heroCard = new HeroCard()
//                {
//                    Title = item.name, 
//                    Text = item.snippet,
//                    Tap = action,
//                };

//                resultMessage.Attachments.Add(heroCard.ToAttachment());
//            }

//            return resultMessage;
//        }

//        public IMessageActivity MakeNewsCards(IDialogContext context, NewsResult news)
//        {
//            var resultMessage = context.MakeMessage();
//            resultMessage.AttachmentLayout = AttachmentLayoutTypes.Carousel;
//            resultMessage.Attachments = new List<Attachment>();
//            foreach (var item in news.value)
//            {
//                var action = new CardAction();
//                action.Type = "openUrl";
//                action.Value = item.url;
//                HeroCard heroCard = new HeroCard()
//                {
//                    Title = item.name,
//                    Text = item.description,
//                    Images = new List<CardImage>()
//                    {
//                        new CardImage() { Url = item.image.thumbnail.contentUrl }
//                    },
//                    Tap = action,
//                };

//                resultMessage.Attachments.Add(heroCard.ToAttachment());
//            }

//            return resultMessage;
//        }

//        public IMessageActivity MakeWeatherCards(IDialogContext context, TenDayForecastResult weather, Dialogs.WeatherDialog.Weather givenVals)
//        {
//            var resultMessage = context.MakeMessage();
//            resultMessage.AttachmentLayout = AttachmentLayoutTypes.Carousel;
//            resultMessage.Attachments = new List<Attachment>();

//            // Find the correct date returned from the API and make the card
//            var result = weather.forecast.simpleforecast.forecastday.Where(f => f.date.day == givenVals.Date.Day).FirstOrDefault();
//            HeroCard heroCard = new HeroCard()
//            {
//                Title = "WEATHER IN " + givenVals.Location.ToUpper(),
//                Text = String.Format("The high for {0}/{1} is {2} degrees F and the low is {3} degrees F. Overall conditions are {4}.",
//                                        result.date.month, result.date.day, result.high.fahrenheit, result.low.fahrenheit, result.conditions.ToLower()),
//            };
//            resultMessage.Attachments.Add(heroCard.ToAttachment());

//            return resultMessage;
//        }

//        public IMessageActivity MakeWeatherCards(IDialogContext context, AverageForecastResult weather, Dialogs.WeatherDialog.Weather givenVals)
//        {
//            var resultMessage = context.MakeMessage();
//            resultMessage.AttachmentLayout = AttachmentLayoutTypes.Carousel;
//            resultMessage.Attachments = new List<Attachment>();
//            HeroCard heroCard = new HeroCard()
//            {
//                Title = "WEATHER IN " + givenVals.Location.ToUpper(),
//                Text = String.Format("The average high for {0} is {1} degrees F and the average low is {2} degrees F.",
//                           weather.trip.period_of_record.date_start.date.monthname, weather.trip.temp_high.avg.F, weather.trip.temp_low.avg.F),
//            };
//            resultMessage.Attachments.Add(heroCard.ToAttachment());

//            return resultMessage;
//        }
//    }
//}