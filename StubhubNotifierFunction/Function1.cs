using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using stubhubnotifier;
using stubhubnotifier.Models;
using StubhubNotifierFunction.Extensions;
using StubhubNotifierFunction.Services;

namespace StubhubNotifierFunction
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static void Run([TimerTrigger("0 */1 * * * *")]TimerInfo myTimer, TraceWriter log, ExecutionContext context)
        {

            var maxPrice = SettingsService.GetSetting("MaxPrice", 30.0);
            var minTicketCount = SettingsService.GetSetting("MinTicketCount", 50);
            var login = SettingsService.GetSetting("StubhubLogin", string.Empty);
            var pass = SettingsService.GetSetting("StubhubPassword", string.Empty);
            var key = SettingsService.GetSetting("StubhubKey", string.Empty);
            var secret = SettingsService.GetSetting("StubhubSecret", string.Empty);
            var cities = SettingsService.GetSetting("cities", string.Empty).FromCSV(',');
            var venues = SettingsService.GetSetting("venues", string.Empty).FromCSV(',');

            var stubHub = new StubHubWrapper(login, pass, key, secret);
            var locationEvents = cities?.Count > 0 ? stubHub.GetEventsForCities(maxPrice, null, minTicketCount, cities.ToArray()).Result : new List<Event>();
            var venueEvents = venues?.Count > 0 ? stubHub.GetEventsForVenues(maxPrice, null, minTicketCount, venues.ToArray()).Result : new List<Event>();

            var allEvents = new List<Event>();//locationEvents;
            allEvents.AddRange(venueEvents);

            var output = allEvents.OrderBy(o => o.eventDateLocal).Select(s => $"{s.eventDateLocal} - {s.name} at {s.venue.name} for {s.ticketInfo.minListPrice} - {s.ticketInfo.maxListPrice} ({s.ticketInfo.totalListings})").ToList();
            output.ForEach(e => Console.WriteLine(e));
        }
    }
}
