using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Builder.Dialogs.Choices;
using EchoBot1.Models;
using EchoBot1.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace EchoBot1.Dialogs
{
    public class QuestionDialog : ComponentDialog
    {
        #region Variables
        private readonly BotStateService _botStateService;
        #endregion

        public QuestionDialog(string dialogid, BotStateService botStateService) : base(dialogid)
        {
            _botStateService = botStateService ?? throw new System.ArgumentNullException(nameof(botStateService));
            InitializeWaterfallDialog();
        }
        private void InitializeWaterfallDialog()
        {
            //Create Waterfall Steps
            var waterfallSteps = new WaterfallStep[]
            {
                DescriptionStepAsync,
                QuestionStepAsync,
                Description2StepAsync,
                SummaryStepAsync
            };
            //Add Named Dialogs
            AddDialog(new WaterfallDialog($"{nameof(QuestionDialog)}.mainFlow", waterfallSteps));
            AddDialog(new TextPrompt($"{nameof(QuestionDialog)}.description"));
            AddDialog(new ChoicePrompt($"{nameof(QuestionDialog)}.response"));

            //Set the starting Dialog
            InitialDialogId = $"{nameof(QuestionDialog)}.mainFlow";
        }
        #region WaterfallSteps  
        private async Task<DialogTurnResult> DescriptionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync($"{nameof(QuestionDialog)}.description",
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("Welcome to our Feedback Feature. Kindly answer the questions provided to enable us to serve you better."),
                }, cancellationToken);
        }
        private async Task<DialogTurnResult> Description2StepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync($"{nameof(QuestionDialog)}.description2",
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("What information or service would you want us to provide?"),
                }, cancellationToken);
        }

        private async Task<DialogTurnResult> QuestionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync($"{nameof(QuestionDialog)}.response",
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("How useful was the information or service received?"),
                    //Choices = ChoiceFactory.ToChoices(new List<string> { "Useful", "Not Useful" }),
                }, cancellationToken);
        }

        private async Task<DialogTurnResult> SummaryStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["response"] = ((FoundChoice)stepContext.Result).Value;

            // Get the current profile object from user state.
            var userProfile = await _botStateService.UserProfileAccessor.GetAsync(stepContext.Context, () => new UserProfile(), cancellationToken);

            // Save all of the data inside the user profile
            userProfile.Description = (string)stepContext.Values["description"];
            userProfile.Response = (string)stepContext.Values["response"];


            // Show the summary to the user
            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Here is a summary of your feedback report:"), cancellationToken);
            await stepContext.Context.SendActivityAsync(MessageFactory.Text(String.Format("Description: {0}", userProfile.Description)), cancellationToken);
            await stepContext.Context.SendActivityAsync(MessageFactory.Text(String.Format("Response: {0}", userProfile.Response)), cancellationToken);

            // Save data in userstate
            await _botStateService.UserProfileAccessor.SetAsync(stepContext.Context, userProfile);

            // WaterfallStep always finishes with the end of the Waterfall or with another dialog, here it is the end.
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }
        #endregion
    }
}