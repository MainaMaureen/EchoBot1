using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Builder;
using System.Threading;
using EchoBot1.Models;
using System;

namespace EchoBot1.Services
{
    public class BotStateService
    {
        #region Variables
        //State Variables
        public ConversationState ConversationState { get; }
        public UserState UserState { get; }
      
        //IDs
        public static string UserProfileId { get; } = $"{nameof(BotStateService)}.UserProfile";
        public static string ConversationDataId { get; } = $"{nameof(BotStateService)}.ConversationData";
        public static string DialogStateId { get; } = $"{nameof(BotStateService)}.DialogState";

        //Accessors
        public IStatePropertyAccessor<UserProfile> UserProfileAccessor { get; set; }
        public IStatePropertyAccessor<ConversationData> ConversationDataAccessor { get; set; }
        public IStatePropertyAccessor<DialogState> DialogStateAccessor { get; set; }
        #endregion
        public BotStateService(ConversationState conversationState, UserState userState)
        {
            UserState = userState ?? throw new ArgumentNullException(nameof(UserState));
            ConversationState = conversationState ?? throw new ArgumentNullException(nameof(conversationState));

            InitializeAccessors();
        }
     
        public void InitializeAccessors()
        {
            //Initializing User State
            UserProfileAccessor = UserState.CreateProperty<UserProfile>(UserProfileId);
            //Initializing Conversation State Accessors
            ConversationDataAccessor = ConversationState.CreateProperty<ConversationData>(ConversationDataId);
            DialogStateAccessor = ConversationState.CreateProperty<DialogState>(DialogStateId);
        }
    }
}
