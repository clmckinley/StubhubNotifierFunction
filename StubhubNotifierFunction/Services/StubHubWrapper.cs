using Newtonsoft.Json;
using stubhubnotifier.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace stubhubnotifier
{
    public class StubHubWrapper
    {
        public static string ResponseType = "application/json";
        public static string TokenUrl = "https://api.stubhub.com/sellers/oauth/accesstoken?grant_type=client_credentials";

        private string access_token;

        public StubHubWrapper(string login, string password, string key, string secret)
        {
            Task.Run(async () => { await GetToken(login, password, key, secret); }).Wait();
        }

        public async Task GetToken(string login, string password, string key, string secret)
        {
            try
            {
                var encodedAuthorization = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{key}:{secret}"));
                using (var httpClient = new HttpClient())
                {
                    using (var request = new HttpRequestMessage(new HttpMethod("POST"), TokenUrl))
                    {
                        request.Headers.TryAddWithoutValidation("Authorization", $"Basic {encodedAuthorization}");

                        request.Content = new StringContent($"{{\n    \"username\": \"{login}\",\n    \"password\": \"{password}\"\n}}", Encoding.UTF8, ResponseType);

                        var response = await httpClient.SendAsync(request);

                        if (response.IsSuccessStatusCode)
                        {
                            var str = await response.Content.ReadAsStringAsync();
                            var dynamic = JsonConvert.DeserializeObject<dynamic>(str);
                            access_token = dynamic.access_token;
                        }
                    }
                }
            }
            catch (Exception exc)
            {

            }
        }

        public async Task<List<Event>> GetEventsForCities(double maxPrice, int[] categoriesToIgnore, int minListings = 30, params string[] cities)
        {
            var rVal = new List<Models.Event>();
            foreach (var city in cities)
            {
                var cEvents = await GetEvents($"https://api.stubhub.com/sellers/search/events/v3?city={city}&rows=500", maxPrice, minListings, categoriesToIgnore);
                rVal.AddRange(cEvents);
            }
            return rVal;
        }

        public async Task<List<Event>> GetEventsForVenues(double maxPrice, int[] categoriesToIgnore, int minListings = 30, params string[] venues)
        {
            var rVal = new List<Models.Event>();
            foreach (var venue in venues)
            {
                rVal.AddRange(await GetEvents($"https://api.stubhub.com/sellers/search/events/v3?venue={venue}&rows=500", maxPrice, minListings, categoriesToIgnore));
            }
            return rVal;
        }


        public async Task<List<Event>> GetEvents(string url, double maxPrice = 30, int minListings = 30, params int[] categoriesToIgnore)
        {
            var catsToIgnore = (categoriesToIgnore ?? new[] { 28, 5242 }).ToList(); //sports and family
            List<Event> rVal = null;
            try
            {
                var response = await MakeRequest<StubhubResponse<Event>>(url);
                var filtered = response.events.Where(w => catsToIgnore.Count == 0 || w.ancestors.categories.Any(a => catsToIgnore.Contains(a.id)) == false).ToList();
                filtered = filtered.Where(w => w.name != null && w.name.ToUpper().Contains("PARKING PASS") == false).ToList();
                filtered = filtered.Where(w => w.ticketInfo.totalListings > minListings && w.ticketInfo.minListPrice > 0 && w.ticketInfo.minListPrice < maxPrice).ToList();
                rVal = filtered.ToList();
            }
            catch (Exception exc)
            {

            }
            return rVal;
        }

        public async Task<T> MakeRequest<T>(string url)
        {
            T rVal = default(T);
            using (var httpClient = new HttpClient())
            {
                using (var request = new HttpRequestMessage(new HttpMethod("GET"), url))
                {
                    request.Headers.TryAddWithoutValidation("Accept", ResponseType);
                    request.Headers.TryAddWithoutValidation("Authorization", $"Bearer {access_token}");

                    var response = await httpClient.SendAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        var str = await response.Content.ReadAsStringAsync();
                        rVal = JsonConvert.DeserializeObject<T>(str);
                    }
                }
            }
            return rVal;
        }

    }
}
