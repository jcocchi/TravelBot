using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.FormFlow;

namespace TravelBot.Dialogs
{
    [Serializable]
    public class EnterLocationDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("What country, state, or city would you like to find travel suggestions for?");

            context.Wait(MessageRecieved);
        }

        private async Task MessageRecieved(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            await context.PostAsync("You said " + message);
            context.Done(message);
        }
    }
}