﻿using Newtonsoft.Json;

namespace TwitchLib.Api.Helix.Models.Moderation.AutomodSettings
{
    /// <summary>
    /// The list of AutoMod settings.
    /// </summary>
    public class AutomodSettingsResponseModel
    {
        /// <summary>
        /// The broadcaster’s ID.
        /// </summary>
        [JsonProperty(PropertyName = "broadcaster_id")]
        public string BroadcasterId;

        /// <summary>
        /// The moderator’s ID.
        /// </summary>
        [JsonProperty(PropertyName = "moderator_id")]
        public string ModeratorId;

        /// <summary>
        /// The default AutoMod level for the broadcaster. 
        /// This field is null if the broadcaster has set one or more of the individual settings.
        /// </summary>
        [JsonProperty(PropertyName = "overall_level")]
        public int? OverallLevel;

        /// <summary>
        /// The Automod level for discrimination against disability.
        /// </summary>
        [JsonProperty(PropertyName = "disability")]
        public int? Disability;

        /// <summary>
        /// The Automod level for hostility involving aggression.
        /// </summary>
        [JsonProperty(PropertyName = "aggression")]
        public int? Aggression;

        /// <summary>
        /// The AutoMod level for discrimination based on sexuality, sex, or gender.
        /// </summary>
        [JsonProperty(PropertyName = "sexuality_sex_or_gender")]
        public int? SexualitySexOrGender;

        /// <summary>
        /// The Automod level for discrimination against women.
        /// </summary>
        [JsonProperty(PropertyName = "misogyny")]
        public int? Misogyny;

        /// <summary>
        /// The Automod level for hostility involving name calling or insults.
        /// </summary>
        [JsonProperty(PropertyName = "bullying")]
        public int? Bullying;

        /// <summary>
        /// The Automod level for profanity.
        /// </summary>
        [JsonProperty(PropertyName = "swearing")]
        public int? Swearing;

        /// <summary>
        /// The Automod level for racial discrimination.
        /// </summary>
        [JsonProperty(PropertyName = "race_ethnicity_or_religion")]
        public int? RaceEthnicityOrReligion;

        /// <summary>
        /// The Automod level for sexual content.
        /// </summary>
        [JsonProperty(PropertyName = "sex_based_terms")]
        public int? SexBasedTerms;
    }
}
