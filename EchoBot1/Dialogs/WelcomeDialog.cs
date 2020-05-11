using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using EchoBot1.Models;
using EchoBot1.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace EchoBot1.Dialogs
{
    public class WelcomeDialog : ComponentDialog //Enables reusable pieces of conversation
    {
        #region Variables
        private readonly BotStateService _botStateService;
        #endregion
        public WelcomeDialog(string dialogId, BotStateService botStateService) : base(dialogId)

        {
            _botStateService = botStateService ?? throw new System.ArgumentNullException(nameof(botStateService));
            InitializeWaterfallDialog();
        }

        private void InitializeWaterfallDialog()
        {
            //Create Waterfall Steps
            var waterfallSteps = new WaterfallStep[]   //Waterfall steps indicate the list or arrangement of the dialogs to be used. Steps are called sequentially.
            {
                InitialStepAsync,
                FinalStepAsync
            };
            //Add Named Dialogs
            AddDialog(new WaterfallDialog($"{nameof(WelcomeDialog)}.mainFlow", waterfallSteps));
            AddDialog(new TextPrompt($"{nameof(WelcomeDialog)}.name"));

            //Set the starting Dialog
            InitialDialogId = $"{nameof(WelcomeDialog)}.mainFlow";  //1st dialog in the step
        }
        private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            UserProfile userProfile = await _botStateService.UserProfileAccessor.GetAsync(stepContext.Context, () => new UserProfile()); //Checks user's name
            if (string.IsNullOrEmpty(userProfile.Name))
            {
                return await stepContext.PromptAsync($"{nameof(WelcomeDialog)}.name",
                        new PromptOptions
                        {
                            Prompt = MessageFactory.Text("What is your name?")  //If empty, the user's name is prompted 
                        }, cancellationToken);
            }
            else
            {
                return await stepContext.NextAsync(null, cancellationToken);   //Called if user's name exists
            }
        }
        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            UserProfile userProfile = await _botStateService.UserProfileAccessor.GetAsync(stepContext.Context, () => new UserProfile());  //Checks for user's name again
            if (string.IsNullOrEmpty(userProfile.Name))
            {
                //Set the user's name
                userProfile.Name = (string)stepContext.Result;

                //Save any state changes that might have occurred during the turn
                await _botStateService.UserProfileAccessor.SetAsync(stepContext.Context, userProfile);
            }

            await stepContext.Context.SendActivityAsync(MessageFactory.Text(String.Format("Hi {0}. How can i help you today?", userProfile.Name)), cancellationToken);
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
    }
}

