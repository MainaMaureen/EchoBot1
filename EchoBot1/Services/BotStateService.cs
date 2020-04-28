using EchoBot1.Models;
using Microsoft.Bot.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        //Accessors
        public IStatePropertyAccessor<UserProfile> UserProfileAccessor { get; set; }
        public IStatePropertyAccessor<ConversationData> ConversationDataAccessor { get; set; }
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
        }
    }
}
