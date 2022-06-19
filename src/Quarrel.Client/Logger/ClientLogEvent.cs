// Quarrel © 2022

using Quarrel.Attributes;

namespace Quarrel.Client.Logger
{
    /// <summary>
    /// An enum for events to log from the client.
    /// </summary>
    public enum ClientLogEvent
    {
        /// <summary>
        /// Encountered an issue in an http request, but it was handled.
        /// </summary>
        [StringValue("Http Exception Handled")]
        HttpExceptionHandled,

        /// <summary>
        /// The gateway encountered a known event.
        /// </summary>
        [StringValue("Known Gateway Event Encountered")]
        KnownGatewayEventEncountered,

        /// <summary>
        /// The gateway encountered an issue, but it was handled.
        /// </summary>
        [StringValue("Gateway Exception Handled")]
        GatewayExceptionHandled,

        /// <summary>
        /// The gateway encountered a unknown operation.
        /// </summary>
        [StringValue("Unknown Gateway Operation Encountered")]
        UnknownGatewayOperationEncountered,

        /// <summary>
        /// The gateway encountered a unknown event.
        /// </summary>
        [StringValue("Unknown Gateway Event Encountered")]
        UnknownGatewayEventEncountered,

        /// <summary>
        /// The gateway encountered a known operation but does not handle it.
        /// </summary>
        [StringValue("Unhandled Gateway Operation Encountered")]
        UnhandledGatewayOperationEncountered,

        /// <summary>
        /// The gateway encountered a known event but does not handle it.
        /// </summary>
        [StringValue("Unhandled Gateway Event Encountered")]
        UnhandledGatewayEventEncountered,

        /// <summary>
        /// The voice connection encountered an issue, but it was handled.
        /// </summary>
        [StringValue("Voice Exception Handled")]
        VoiceExceptionHandled,

        /// <summary>
        /// The voice encountered a known operation.
        /// </summary>
        [StringValue("Known Voice Operation Encountered")]
        KnownVoiceOperationEncountered,

        /// <summary>
        /// The voice connection encountered a unknown operation.
        /// </summary>
        [StringValue("Unknown Voice Operation Encountered")]
        UnknownVoiceOperationEncountered,

        /// <summary>
        /// The voice connection encountered a known operation but does not handle it.
        /// </summary>
        [StringValue("Unhandled Voice Operation Encountered")]
        UnhandledVoiceOperationEncountered,
    }
}
