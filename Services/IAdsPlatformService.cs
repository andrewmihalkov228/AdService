namespace AdsWebService.Services
{
    public interface IAdsPlatformService
    {
        Task LoadPlatformsAsync(Stream fileStream);

        IEnumerable<string> SearchPlatforms(string location);
    }
}
