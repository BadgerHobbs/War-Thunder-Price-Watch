using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace War_Thunder_Store_Scraper
{
    internal class Scraper
    {
        // Set variables
        // Create http web client
        HttpClient client = new HttpClient();

        // Init
        public Scraper()
        {
        }

        // Get html from url
        private HtmlAgilityPack.HtmlDocument GetHtmlFromUrl(string url)
        {
            // Get html from url
            string html = client.GetStringAsync(url).Result;

            // Create html document
            HtmlAgilityPack.HtmlDocument document = new HtmlAgilityPack.HtmlDocument();

            // Load html
            document.LoadHtml(html);

            // Return html document
            return document;
        }


        // Get store items for page number
        public List<string> GetStoreItemLinksForPage(int pageNumber = 1)
        {
            // https://store.gaijin.net/catalog.php?category=WarThunder&page=X
            string url = $"https://store.gaijin.net/catalog.php?category=WarThunder&page={pageNumber}";

            // Get html from url
            HtmlAgilityPack.HtmlDocument document = GetHtmlFromUrl(url);

            // Get all store items of a tag with class "product-widget__link"
            HtmlAgilityPack.HtmlNodeCollection storeItemLinkNodes = document.DocumentNode.SelectNodes("//a[@class='product-widget__link']");

            // Return empty list if no store items found
            if (storeItemLinkNodes == null)
            {
                return new List<string>();
            }

            List<string> storeItemLinks = new List<string>();

            // Loop through store items
            foreach (HtmlAgilityPack.HtmlNode storeItemLinkNode in storeItemLinkNodes)
            {
                // Get store item link
                string storeItemLink = storeItemLinkNode.Attributes["href"].Value;

                // Add store item link to list
                storeItemLinks.Add(storeItemLink);
            }

            // Return store item links
            return storeItemLinks;
        }

        // Get all store item links
        public List<string> GetAllStoreItemLinks()
        {
            // List for store item links
            List<string> storeItemLinks = new List<string>();

            // Loop through pages until no new store item links are found or page limit is reached
            int pageNumber = 1;
            while (pageNumber <= 100)
            {
                // Get store item links for page
                var pageStoreItemLinks = GetStoreItemLinksForPage(pageNumber);

                // Check if page has no store item links
                if (pageStoreItemLinks.Count == 0)
                {
                    // Break loop
                    break;
                }

                // Add store item links to list
                storeItemLinks.AddRange(pageStoreItemLinks);

                // Increment page number
                pageNumber++;
            }

            // Remove duplicates from list
            storeItemLinks = storeItemLinks.Distinct().ToList();

            // Return store item links
            return storeItemLinks;
        }

        // Create store item from store item link
        public StoreItem CreateStoreItem(string storeItemLink)
        {
            // Get html from url
            HtmlAgilityPack.HtmlDocument document = GetHtmlFromUrl(storeItemLink);

            // Get name node and then string if found
            HtmlAgilityPack.HtmlNode? nameNode = document.DocumentNode.SelectSingleNode("//div[@class='shop__title']");
            string? name = nameNode != null ? nameNode.InnerText.Trim() : null;

            // Get price node and then string if found
            HtmlAgilityPack.HtmlNode? priceNode = document.DocumentNode.SelectSingleNode("//div[@class='shop-buy__price shop-price']");

            // If price node is found and has child node with class "shop-price__new"
            if (priceNode != null && priceNode.ChildNodes.Any(x => x.Attributes["class"]?.Value == "shop-price__new"))
            {
                // Set price node to price node with class "shop-price__new"
                priceNode = priceNode.ChildNodes.First(x => x.Attributes["class"]?.Value == "shop-price__new");
            }
            string? price = priceNode != null ? priceNode.InnerText.Trim().Split(" ").FirstOrDefault() : null;

            // Get description node and then string if found
            HtmlAgilityPack.HtmlNode? descriptionNode = document.DocumentNode.SelectSingleNode("//table[@class='shop__article-wrapper']");            
            string? description = descriptionNode != null ? descriptionNode.InnerText.Trim() : null;

            // Get average rating node and then string if found
            HtmlAgilityPack.HtmlNode? averageRatingNode = document.DocumentNode.SelectSingleNode("//span[@class='js-voting__average']");
            float? averageRating = averageRatingNode != null ? float.Parse(averageRatingNode.InnerText) : null;

            // Get number of ratings node and then string if found
            HtmlAgilityPack.HtmlNode? numberOfRatingsNode = document.DocumentNode.SelectSingleNode("//span[@class='js-voting__votes-count']");
            int? numberOfRatings = numberOfRatingsNode != null ? int.Parse(numberOfRatingsNode.InnerText) : null;

            // Create store item
            StoreItem storeItem = new StoreItem(
                name: name,
                price: price,
                description: description,
                averageRating: averageRating,
                numberOfVotes: numberOfRatings,
                url: storeItemLink,
                timeStamp: DateTime.Now
                );

            // Return store item
            return storeItem;
        }
    }
}
