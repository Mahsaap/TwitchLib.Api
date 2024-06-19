using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using TwitchLib.Api.Core;
using TwitchLib.Api.Core.Enums;
using TwitchLib.Api.Core.Exceptions;
using TwitchLib.Api.Core.Interfaces;
using TwitchLib.Api.Helix.Models.EventSub;
using TwitchLib.Api.Helix.Models.EventSub.Conduits.CreateConduits;
using TwitchLib.Api.Helix.Models.EventSub.Conduits.GetConduits;
using TwitchLib.Api.Helix.Models.EventSub.Conduits.Shards.GetConduitShards;
using TwitchLib.Api.Helix.Models.EventSub.Conduits.Shards.UpdateConduitShards;
using TwitchLib.Api.Helix.Models.EventSub.Conduits.UpdateConduits;

namespace TwitchLib.Api.Helix
{
    /// <summary>
    /// EventSub related APIs
    /// </summary>
    public class EventSub : ApiBase
    {
        public EventSub(IApiSettings settings, IRateLimiter rateLimiter, IHttpCallHandler http) : base(settings, rateLimiter, http)
        {
        }

        #region CreateEventSubSubscription

        /// <summary>
        /// Creates an EventSub subscription.
        /// </summary>
        /// <param name="type">The type of subscription to create.</param>
        /// <param name="version">The version of the subscription type used in this request.</param>
        /// <param name="condition">The parameter values that are specific to the specified subscription type.</param>
        /// <param name="method">The transport method. Supported values: Webhook, Websocket.</param>
        /// <param name="websocketSessionId">The session Id of a websocket connection that you want to subscribe to an event for. Only needed if method is Websocket</param>
        /// <param name="conduitId">The conduit Id of a EventSub conduit. Only needed if method is Conduit.</param>
        /// <param name="webhookCallback">The callback URL where the Webhook notification should be sent. Only needed if method is Webhook</param>
        /// <param name="webhookSecret">The secret used for verifying a Webhooks signature. Only needed if method is Webhook</param>
        /// <param name="clientId">optional Client ID to override the use of the stored one in the TwitchAPI instance</param>
        /// <param name="accessToken">optional access token to override the use of the stored one in the TwitchAPI instance</param>
        /// <returns cref="CreateEventSubSubscriptionResponse"></returns>
        public Task<CreateEventSubSubscriptionResponse> CreateEventSubSubscriptionAsync(string type, string version, Dictionary<string, string> condition, EventSubTransportMethod method, string websocketSessionId = null, string webhookCallback = null,
            string webhookSecret = null, string conduitId = null, string clientId = null, string accessToken = null)
        {
            if (string.IsNullOrEmpty(type))
                throw new BadParameterException("type must be set");

            if (string.IsNullOrEmpty(version))
                throw new BadParameterException("version must be set");

            if (condition == null || condition.Count == 0)
                throw new BadParameterException("condition must be set");

            switch (method)
            {
                case EventSubTransportMethod.Webhook:
                    if (string.IsNullOrWhiteSpace(webhookCallback))
                        throw new BadParameterException("webhookCallback must be set");

                    if (webhookSecret == null || webhookSecret.Length < 10 || webhookSecret.Length > 100)
                        throw new BadParameterException("webhookSecret must be set, and be between 10 (inclusive) and 100 (inclusive)");

                    var webhookBody = new
                    {
                        type,
                        version,
                        condition,
                        transport = new
                        {
                            method = method.ToString().ToLowerInvariant(),
                            callback = webhookCallback,
                            secret = webhookSecret
                        }
                    };
                    return TwitchPostGenericAsync<CreateEventSubSubscriptionResponse>("/eventsub/subscriptions", ApiVersion.Helix, JsonConvert.SerializeObject(webhookBody), null, accessToken, clientId);
                case EventSubTransportMethod.Websocket:
                    if (string.IsNullOrWhiteSpace(websocketSessionId))
                        throw new BadParameterException("websocketSessionId must be set");

                    var websocketBody = new
                    {
                        type,
                        version,
                        condition,
                        transport = new
                        {
                            method = method.ToString().ToLowerInvariant(),
                            session_id = websocketSessionId
                        }
                    };
                    return TwitchPostGenericAsync<CreateEventSubSubscriptionResponse>("/eventsub/subscriptions", ApiVersion.Helix, JsonConvert.SerializeObject(websocketBody), null, accessToken, clientId);
                case EventSubTransportMethod.Conduit:
                    if (string.IsNullOrWhiteSpace(conduitId))
                        throw new BadParameterException("conduitId must be set");
                    
                    var conduitBody = new
                    {
                        type,
                        version,
                        condition,
                        transport = new
                        {
                            method = method.ToString().ToLowerInvariant(),
                            conduit_id = conduitId
                        }
                    };
                    return TwitchPostGenericAsync<CreateEventSubSubscriptionResponse>("/eventsub/subscriptions", ApiVersion.Helix, JsonConvert.SerializeObject(conduitBody), null, accessToken, clientId);
                default:
                    throw new ArgumentOutOfRangeException(nameof(method), method, null);
            }
        }
        #endregion

        # region GetEventSubSubscriptions

        /// <summary>
        /// Gets a list of your EventSub subscriptions. The list is paginated and ordered by the oldest subscription first.
        /// </summary>
        /// <param name="status">Filter subscriptions by its status.</param>
        /// <param name="type">Filter subscriptions by subscription type (e.g., channel.update).</param>
        /// <param name="userId">Filter subscriptions by user ID.</param>
        /// <param name="after">The cursor used to get the next page of results.</param>
        /// <param name="clientId">optional Client ID to override the use of the stored one in the TwitchAPI instance</param>
        /// <param name="accessToken">optional access token to override the use of the stored one in the TwitchAPI instance</param>
        /// <returns cref="GetEventSubSubscriptionsResponse">Returns a list of your EventSub subscriptions.</returns>
        public Task<GetEventSubSubscriptionsResponse> GetEventSubSubscriptionsAsync(string status = null, string type = null, string userId = null, string after = null, string clientId = null, string accessToken = null)
        {
            var getParams = new List<KeyValuePair<string, string>>();

            if (!string.IsNullOrWhiteSpace(status))
                getParams.Add(new KeyValuePair<string, string>("status", status));

            if (!string.IsNullOrWhiteSpace(type))
                getParams.Add(new KeyValuePair<string, string>("type", type));

            if (!string.IsNullOrWhiteSpace(userId))
                getParams.Add(new KeyValuePair<string, string>("user_id", userId));

            if (!string.IsNullOrWhiteSpace(after))
                getParams.Add(new KeyValuePair<string, string>("after", after));

            return TwitchGetGenericAsync<GetEventSubSubscriptionsResponse>("/eventsub/subscriptions", ApiVersion.Helix, getParams, accessToken, clientId);
        }
        #endregion

        #region DeleteEventSubSubscription

        /// <summary>
        /// Deletes an EventSub subscription.
        /// </summary>
        /// <param name="id">The ID of the subscription to delete.</param>
        /// <param name="clientId">optional Client ID to override the use of the stored one in the TwitchAPI instance</param>
        /// <param name="accessToken">optional access token to override the use of the stored one in the TwitchAPI instance</param>
        /// <returns>True: If successfully deleted; False: If delete failed</returns>
        public async Task<bool> DeleteEventSubSubscriptionAsync(string id, string clientId = null, string accessToken = null)
        {
            var getParams = new List<KeyValuePair<string, string>>
            {
                new("id", id)
            };

            var response = await TwitchDeleteAsync("/eventsub/subscriptions", ApiVersion.Helix, getParams, accessToken, clientId);

            return response.Key == (int) HttpStatusCode.NoContent;
        }

        #endregion

        #region GetConduits

        /// <summary>
        /// Gets the conduits for a client ID.
        /// </summary>
        /// <param name="clientId">optional Client ID to override the use of the stored one in the TwitchAPI instance</param>
        /// <param name="accessToken">optional access token to override the use of the stored one in the TwitchAPI instance</param>
        /// <returns cref="GetConduitsResponse">Returns a list of your conduits.</returns>
        public Task<GetConduitsResponse> GetConduitsAsync(string clientId = null, string accessToken = null)
        {
            return TwitchGetGenericAsync<GetConduitsResponse>("/eventsub/conduits", ApiVersion.Helix,
                null, accessToken, clientId);
        }
        #endregion

        #region CreateConduits

        /// <summary>
        /// Creates a new conduit.
        /// </summary>
        /// <param name="request">Request body parameters identifying conduit details</param>
        /// <param name="clientId">optional Client ID to override the use of the stored one in the TwitchAPI instance</param>
        /// <param name="accessToken">optional access token to override the use of the stored one in the TwitchAPI instance</param>
        /// <returns cref="CreateConduitsResponse">Returns a list of your conduits.</returns>
        public Task<CreateConduitsResponse> CreateConduitsAsync(CreateConduitsRequest request, string clientId = null,
            string accessToken = null)
        {
            if (request.ShardCount is <= 0 or > 20_000)
                throw new BadParameterException("request.ShardCount must be greater than 0 and less or equal than 20000");
            
            return TwitchPostGenericAsync<CreateConduitsResponse>("/eventsub/conduits", ApiVersion.Helix,
                JsonConvert.SerializeObject(request), null, accessToken, clientId);
        }
        #endregion

        #region UpdateConduits

        /// <summary>
        /// Updates a conduit’s shard count. To delete shards, update the count to a lower number, and the shards above the count will be deleted. For example, if the existing shard count is 100, by resetting shard count to 50, shards 50-99 are disabled.
        /// </summary>
        /// <param name="request">Request body parameters identifying conduit details</param>
        /// <param name="clientId">optional Client ID to override the use of the stored one in the TwitchAPI instance</param>
        /// <param name="accessToken">optional access token to override the use of the stored one in the TwitchAPI instance</param>
        /// <returns cref="UpdateConduitsResponse">Returns a list of your conduits.</returns>
        public Task<UpdateConduitsResponse> UpdateConduitsAsync(UpdateConduitsRequest request, string clientId = null,
            string accessToken = null)
        {
            if (request.ShardCount is <= 0 or > 20_000)
                throw new BadParameterException("request.ShardCount must be greater than 0 and less or equal than 20000");
            
            return TwitchPatchGenericAsync<UpdateConduitsResponse>("/eventsub/conduits", ApiVersion.Helix,
                JsonConvert.SerializeObject(request), null, accessToken, clientId);
        }
        #endregion

        # region DeleteConduit

        /// <summary>
        /// Deletes a conduit.
        /// </summary>
        /// <param name="id">The ID of the conduit to delete.</param>
        /// <param name="clientId">optional Client ID to override the use of the stored one in the TwitchAPI instance</param>
        /// <param name="accessToken">optional access token to override the use of the stored one in the TwitchAPI instance</param>
        /// <returns>True: If successfully deleted; False: If delete failed</returns>
        public async Task<bool> DeleteConduitAsync(string id, string clientId = null, string accessToken = null)
        {
            var getParams = new List<KeyValuePair<string, string>>
            {
                new("id", id)
            };

            var response = await TwitchDeleteAsync("/eventsub/conduits", ApiVersion.Helix, getParams, accessToken, clientId);

            return response.Key == (int) HttpStatusCode.NoContent;
        }
        #endregion

        #region GetConduitShards

        /// <summary>
        /// Gets a lists of all shards for a conduit.
        /// </summary>
        /// <param name="conduitId">Conduit ID.</param>
        /// <param name="status">Status to filter by.</param>
        /// <param name="after">The cursor used to get the next page of results. The pagination object in the response contains the cursor’s value.</param>
        /// <param name="clientId">optional Client ID to override the use of the stored one in the TwitchAPI instance</param>
        /// <param name="accessToken">optional access token to override the use of the stored one in the TwitchAPI instance</param>
        /// <returns cref="GetConduitShardsResponse">Returns a list shards owned by the specified conduit.</returns>
        public Task<GetConduitShardsResponse> GetConduitShardsAsync(string conduitId, string status = null, string after = null, string clientId = null,
            string accessToken = null)
        {
            var getParams = new List<KeyValuePair<string, string>>
            {
                new("conduit_id", conduitId)
            };
            if(!string.IsNullOrWhiteSpace(status))
                getParams.Add(new KeyValuePair<string, string>("status", status));
            if(!string.IsNullOrWhiteSpace(after))
                getParams.Add(new KeyValuePair<string, string>("after", after));

            return TwitchGetGenericAsync<GetConduitShardsResponse>("/eventsub/conduits/shards", ApiVersion.Helix,
                getParams, accessToken, clientId);
        }
        #endregion

        #region UpdateConduitShards

        /// <summary>
        /// Updates shard(s) for a conduit.
        /// </summary>
        /// <param name="request">Request body parameters for updating conduit shards</param>
        /// <param name="clientId">optional Client ID to override the use of the stored one in the TwitchAPI instance</param>
        /// <param name="accessToken">optional access token to override the use of the stored one in the TwitchAPI instance</param>
        /// <returns cref="UpdateConduitShardsResponse">Returns a list of successfully and errored conduit shard updates</returns>
        public Task<UpdateConduitShardsResponse> UpdateConduitShardsAsync(UpdateConduitShardsRequest request, string clientId = null,
            string accessToken = null)
        {
            List<string> validMethods =
            [
                "webhook", "websocket"
            ];
            const int secretMinLength = 10;
            const int secretMaxLength = 100;
            
            foreach (var shard in request.Shards)
            {
                if (!validMethods.Contains(shard.Transport.Method))
                    throw new BadParameterException($"request.Shards.Transport.Method valid values: {String.Join(", ", validMethods)}");
                if (shard.Transport.Secret != null && (shard.Transport.Secret.Length < secretMinLength || shard.Transport.Secret.Length > secretMaxLength))
                    throw new BadParameterException(
                        $"request.Shards.Transport.Secret must be greater than or equal to {secretMinLength} and less than or equal to {secretMaxLength}");
            }
            
            return TwitchPatchGenericAsync<UpdateConduitShardsResponse>("/eventsub/conduits/shards",
                ApiVersion.Helix, JsonConvert.SerializeObject(request), null, accessToken, clientId);
        }
        #endregion
    }
}
