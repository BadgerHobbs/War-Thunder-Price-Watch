using War_Thunder_Store_Scraper;

// Get named arguments from the command line
Dictionary<string, string> arguments = new Dictionary<string, string>();

// Parse hyphenated arguments
bool previousArgumentHyphenated = false;
string previousArgument = "";

foreach (string argument in Environment.GetCommandLineArgs())
{
    Console.WriteLine(argument);

    if (previousArgumentHyphenated)
    {
        previousArgumentHyphenated = false;
        arguments[previousArgument] = argument;
    }
    else if (argument.StartsWith("--"))
    {
        previousArgumentHyphenated = true;
    }
    else
    {
        previousArgumentHyphenated = false;
    }

    previousArgument = argument;
}

// Set values from arguments
string databaseDirectory = arguments.GetValueOrDefault("--database-directory", "");
int refreshInterval = int.Parse(arguments.GetValueOrDefault("--refresh-interval", "1440")); // in minutes

// Log arguments to console
Console.WriteLine($"Database directory: {databaseDirectory}");
Console.WriteLine($"Refresh interval: {refreshInterval} minutes");

while (true) 
{
    Console.WriteLine("Scraping...");

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
    Database database = new Database($"{databaseDirectory}warthunder_store.db");

    // Insert store items into database
    database.UpdateDatabaseWithStoreItems(storeItems);

    // Wait for next refresh
    Console.WriteLine($"Waiting {refreshInterval} minutes for next refresh...");
    Thread.Sleep(refreshInterval * 60 * 1000);
}

