using Newtonsoft.Json;

namespace commercetools.Subscriptions
{
    /// <summary>
    /// AWS SQS is a pull-queue on AWS.
    /// </summary>
    /// <remarks>
    /// The queue needs to exist beforehand. It is recommended to create an accessKey and accessSecret pair specifically for each subscription that only has the sqs:SendMessage permission on this queue.
    /// </remarks>
    /// <see href="https://dev.commercetools.com/http-api-projects-subscriptions.html#aws-sqs-destination"/>
    /// <seealso href="https://aws.amazon.com/sqs/"/>
    public class AWSSQSDestination : Destination
    {
        #region Properties

        /// <summary>
        /// QueueUrl
        /// </summary>
        [JsonProperty(PropertyName = "queueURL")]
        public string QueueUrl { get; private set; }

        /// <summary>
        /// AccessKey
        /// </summary>
        [JsonProperty(PropertyName = "accessKey")]
        public string AccessKey { get; private set; }

        /// <summary>
        /// AccessSecret
        /// </summary>
        [JsonProperty(PropertyName = "accessSecret")]
        public string AccessSecret { get; private set; }

        /// <summary>
        /// Region
        /// </summary>
        [JsonProperty(PropertyName = "region")]
        public string Region { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes this instance with JSON data from an API response.
        /// </summary>
        /// <param name="data">JSON object</param>
        public AWSSQSDestination(dynamic data)
            : base((object)data)
        {
            if (data == null)
            {
                return;
            }

            this.QueueUrl = data.queueURL;
            this.AccessKey = data.accessKey;
            this.AccessSecret = data.accessSecret;
            this.Region = data.region;
        }

        #endregion
    }
}
