﻿using Newtonsoft.Json;

namespace TwitchLib.Api.Helix.Models.Chat.SendChatMessage
{
    public class SendChatMessageResponse
    {
        /// <summary>
        /// The data for the chat message
        /// </summary>
        [JsonProperty(PropertyName = "data", NullValueHandling = NullValueHandling.Ignore)]
        public ChatMessageInfo[] Data { get; protected set; }
    }
}