using Microsoft.AspNetCore.Mvc;

namespace AdsWebService.Services
{
    public class AdsPlatformService : IAdsPlatformService
    {
        private Dictionary<string, List<string>> _locationIndex = new Dictionary<string, List<string>>();

        public async Task LoadPlatformsAsync(Stream fileStream)
        {
            var newLocationIndex = new Dictionary<string, List<string>>();

            string line;

            using (var reader = new StreamReader(fileStream))
            {
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }

                    var parts = line.Split(':', 2);
                    if (parts.Length < 2)
                    {
                        continue;
                    }

                    var platformName = parts[0].Trim();
                    var locationsString = parts[1].Trim();

                    if (string.IsNullOrEmpty(platformName) || string.IsNullOrEmpty(locationsString))
                    {
                        continue;
                    }

                    var locations = locationsString.Split(',')
                        .Select(loc => loc.Trim())
                        .Where(loc => !string.IsNullOrEmpty(loc))
                        .ToList();

                    foreach (var location in locations)
                    {
                        if (!newLocationIndex.ContainsKey(location))
                        {
                            newLocationIndex[location] = new List<string>();
                        }
                        newLocationIndex[location].Add(platformName);
                    }
                }
            }

            _locationIndex = newLocationIndex;
        }

        public IEnumerable<string> SearchPlatforms(string location)
        {
            var foundPlatforms = new HashSet<string>();

            var currentIndex = _locationIndex;
            var currentLocation = location;

            while (!string.IsNullOrEmpty(currentLocation))
            {
                if (currentIndex.TryGetValue(currentLocation, out var platforms))
                {
                    foreach (var platform in platforms)
                    {
                        foundPlatforms.Add(platform);
                    }
                }

                var lastSlashIndex = currentLocation.LastIndexOf('/');
                if (lastSlashIndex > 0)
                {
                    currentLocation = currentLocation.Substring(0, lastSlashIndex);
                }
                else if (lastSlashIndex == 0 && currentLocation.Length > 1)
                {
                    currentLocation = "";
                }
                else
                {
                    currentLocation = null;
                }
            }

            return foundPlatforms;
        }
    }
}
