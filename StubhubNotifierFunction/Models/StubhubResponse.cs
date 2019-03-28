using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stubhubnotifier.Models
{
    public class StubhubResponse<T> 
        where T : class
    {
        public int numFound { get; set; }
        public T[] events { get; set; }
    }

    public class Event
    {
        public long id { get; set; }
        public string status { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string webURI { get; set; }
        public DateTimeOffset eventDateLocal { get; set; }
        public DateTimeOffset eventDateUTC { get; set; }
        public DateTimeOffset createdDate { get; set; }
        public DateTimeOffset lastUpdatedDate { get; set; }
        public Venue venue { get; set; }
        public string timezone { get; set; }
        public InfoPair[] performers { get; set; }
        public Ancestors ancestors { get; set; }
        public string currencyCode { get; set; }
        public string imageUrl { get; set; }
        public TicketInfo ticketInfo { get; set; }
    }
    public class TicketInfo
    {
        public double minListPrice { get; set; }
        public double maxListPrice { get; set; }
        public double totalTickets { get; set; }
        public double totalListings { get; set; }
    }
    public class Venue
    {
        public long id { get; set; }
        public string name { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string postalCode { get; set; }
        public string country { get; set; }
        public long venueConfigId { get; set; }
        public string venueConfigName { get; set; }
    }
    public class Ancestors
    {
        public InfoPair[] categories { get; set; }
        public InfoPair[] groupings { get; set; }
        public InfoPair[] performers { get; set; }
    }
    public class InfoPair
    {
        public int id { get; set; }
        public string name { get; set; }
    }
}
