using EchoBot1.Models;
using EchoBot1.Services;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;


namespace EchoBot1.Bots
{
    public class FeedbackBot : ActivityHandler  
    {
        #region Variables
        private readonly BotStateService _botStateService;
        #endregion
        public FeedbackBot(BotStateService botStateservice)
        {
            _botStateService = botStateservice ?? throw new System.ArgumentNullException(nameof(botStateservice));
        }
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            try
            {
                var content = new JsonReader().Index();
            }
            catch (Exception ex)
            {
                throw ex;
            }
           
            await GetName(turnContext, cancellationToken);
        }
        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await GetName(turnContext, cancellationToken);
                }
            }
        }
        private async Task GetName(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            UserProfile userProfile = await _botStateService.UserProfileAccessor.GetAsync(turnContext, () => new UserProfile());
            ConversationData conversationData = await _botStateService.ConversationDataAccessor.GetAsync(turnContext, () => new ConversationData());
            if (!string.IsNullOrEmpty(userProfile.Name))
            {
                await turnContext.SendActivityAsync(MessageFactory.Text(String.Format("Hi  {0}.  Welcome back to our Feedback Feature. Please take time to answer the provided questions regarding our services, to enable us to serve you better.", userProfile.Name)), cancellationToken);
                //Prompt the first question <from the json array>
                await turnContext.SendActivityAsync(MessageFactory.Text(String.Format("**This should be the first question** How useful was the information or service you received? Please choose (1.USEFUL,) (2.NOT USEFUL)", cancellationToken)));
            }
            else
            {
                if (conversationData.PromptedUserForName)
                {
                    //Set the name to what user provided. 
                    userProfile.Name = turnContext.Activity.Text?.Trim();

                    //Acknowledge that we got their name and provide brief intoduction to the feature
                    await turnContext.SendActivityAsync(MessageFactory.Text(String.Format("Hello  {0}.  Welcome to our Feedback Feature(1st time user). Please take time to answer the provided questions regarding our services, to enable us to serve you better.", userProfile.Name)), cancellationToken);
                    //Prompt the first question <from the json array>
                    await turnContext.SendActivityAsync(MessageFactory.Text(String.Format("**This should be the first question** How useful was the information or service you received? Please choose (1.USEFUL,) (2.NOT USEFUL)", cancellationToken)));
                    
                    //Reset the flag to allow the bot to go through the cycle again.
                    conversationData.PromptedUserForName = false;
                }
                else
                {
                    //Prompt the user for their name.
                    await turnContext.SendActivityAsync(MessageFactory.Text($"What is your name?"), cancellationToken);

                    //Set the flag to true so that we do not prompt it in the next turn.
                    conversationData.PromptedUserForName = true;
                }

                // Save any state changes that might have occured during the turn.
                await _botStateService.UserProfileAccessor.SetAsync(turnContext, userProfile);
                await _botStateService.ConversationDataAccessor.SetAsync(turnContext, conversationData);

                await _botStateService.UserState.SaveChangesAsync(turnContext);
                await _botStateService.ConversationState.SaveChangesAsync(turnContext);

            }
        }
    }
}
