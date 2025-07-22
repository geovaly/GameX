namespace RequestResponseFramework.Client.Http;

public interface IRequestResponseClientSettingsProvider
{
    RequestResponseClientSettings ClientSettings { get; }
}