namespace Sample.CommandsAndQueriesApi.MongoDb.IntegrationEvents.Constants;

internal static class ExtraValuesKeys
{
    public const string PAYLOAD_JSON = "integration_event_payload_json";
    public const string LOCK_UNTIL_UNIX_SECONDS = "integration_event_lock_until_unix_seconds";
    public const string PUBLISH_RESULT_TIME = "integration_event_publish_result_time";
    public const string PUBLISH_RESULT_SUCCESS = "integration_event_publish_result_success";
    public const string PUBLISH_RESULT_ERROR = "integration_event_publish_result_error";
}