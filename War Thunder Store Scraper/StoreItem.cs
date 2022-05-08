using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace War_Thunder_Store_Scraper
{
    internal class StoreItem
    {
        public string? Name { get; set; }
        public string? Id { get; set; }
        public string? Price { get; set; }
        public string? Url { get; set; }
        public string? Description { get; set; }
        public float? AverageRating { get; set; }
        public int? NumberOfVotes { get; set; }
        public DateTime? TimeStamp { get; set; }


        // Constructor
        public StoreItem(string? name = null, string? id = null, string? price = null, string? url = null, string? description = null, float? averageRating = null, int? numberOfVotes = null, DateTime? timeStamp = null)
        {
            Name = name;
            Id = url != null ? GetIdFromLink(url) : id;
            Price = price;
            Url = url;
            Description = description;
            AverageRating = averageRating;
            NumberOfVotes = numberOfVotes;
            TimeStamp = timeStamp;
         }

        // Get id from link
        public string? GetIdFromLink(string link)
        {
            // Get url parameters from link
            string queryString = new System.Uri(link).Query;
            var queryDictionary = System.Web.HttpUtility.ParseQueryString(queryString);

            // Try to get id from url parameters, return null if not found
            return queryDictionary.AllKeys.Contains("id") ? queryDictionary["id"] : null;
        }
    }
}
