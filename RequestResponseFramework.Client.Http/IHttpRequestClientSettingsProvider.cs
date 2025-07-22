namespace RequestResponseFramework.Client.Http;

public interface IHttpRequestClientSettingsProvider
{
    HttpRequestClientSettings ClientSettings { get; }
}