using Newtonsoft.Json;

namespace commercetools.Subscriptions
{
    /// <summary>
    /// ChangeSubscription
    /// </summary>
    /// <remarks>
    /// Different payloads for resource creation, update and deletion are delivered.
    /// </remarks>
    /// <see href="https://dev.commercetools.com/http-api-projects-subscriptions.html#changesubscription"/>
    public class ChangeSubscription 
    {
        #region Properties

        /// <summary>
        /// ResourceTypeId
        /// </summary>
        [JsonProperty(PropertyName = "resourceTypeId")]
        public string ResourceTypeId { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes this instance with JSON data from an API response.
        /// </summary>
        /// <param name="data">JSON object</param>
        public ChangeSubscription(dynamic data)
        {
            if (data == null)
            {
                return;
            }

            this.ResourceTypeId = data.resourceTypeId;
        }

        #endregion
    }
}
