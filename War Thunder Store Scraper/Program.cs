using War_Thunder_Store_Scraper;

// Run scraper
Scraper scraper = new Scraper();
List<string> storeItemLinks = scraper.GetAllStoreItemLinks();

List<StoreItem> storeItems = new List<StoreItem>();

// Print all store item links
foreach (string link in storeItemLinks)
{
    Console.WriteLine(link);

    // Create store item
    StoreItem storeItem = scraper.CreateStoreItem(link);

    // Add store item to list
    storeItems.Add(storeItem);
}

// Create Database object
Database database = new Database("warthunder_store.db");

// Create table
database.CreateStoreItemsTable();

// Insert store items into database
database.InsertStoreItems(storeItems);

// Press any key to exit
Console.ReadKey();
