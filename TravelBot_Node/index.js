var restify = require('restify');
var builder = require('botbuilder');

//=========================================================
// Bot Setup
//=========================================================

// Setup Restify Server
var server = restify.createServer();
server.listen(process.env.port || process.env.PORT || 3978, function () {
   console.log('%s listening to %s', server.name, server.url); 
});
  
// Create chat bot
var connector = new builder.ChatConnector({
    appId: process.env.MICROSOFT_APP_ID,
    appPassword: process.env.MICROSOFT_APP_PASSWORD
});
var bot = new builder.UniversalBot(connector);
server.post('/api/messages', connector.listen());

// Configure LUIS
//const LuisModelUrl = process.env.LUIS_MODEL_URL;
const LuisModelUrl = 'https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/138ad2b9-9738-4871-ba7a-93c1121aff70?subscription-key=efe598955c1e4f4f93a89e018d468e0a&verbose=true';
var recognizer = new builder.LuisRecognizer(LuisModelUrl);

//=========================================================
// Bots Dialogs
//=========================================================
bot.dialog('/', new builder.IntentDialog({ recognizers: [recognizer] })
    .matches('Greeting', (session, args) => {
            session.send('Hello! How can I assist you with your travels today?\n' +
                            '- Search for a travel destination\n' +
                            '- Check the current news in a location\n' +
                            '- Check the weather in a location for a specific date\n');
        }
    )
    .matches('FindDestination', [
        function (session, args) {
            session.send('Searching for travel destinations!');
        }
    ])
    .matches('GetNews', [
        function (session, args) {
            session.send('Searching for current news !');
        }
    ])
    .matches('GetWeather', [
        function (session, args) {
            session.send('Searching for weather!');
        }
    ])
    .matches('ThankYou', builder.DialogAction.send('You\'re welcome! To see what else I can help you with type \"help\".'))
    .onDefault((session) => {
        session.send('Sorry, I did not understand \'%s\'. Type \"help\" if you need assistance.', session.message.text);
    }));