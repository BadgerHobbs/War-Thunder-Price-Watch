using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace War_Thunder_Store_Scraper
{
    internal class Database
    {
        string filename;
        SqliteConnection connection;

        // Constructor
        public Database(string filename)
        {
            this.filename = filename;

            // Create connection
            connection = new SqliteConnection($"Data Source={filename}");

            // Create tables
            CreateStoreItemsTable();
            CreatePriceHistoryTable();
            CreateRatingHistoryTable();
        }

        // Create Store Items table
        public void CreateStoreItemsTable()
        {
            // Open connection
            connection.Open();

            // Create table
            string sql = "CREATE TABLE IF NOT EXISTS StoreItems (id INTEGER PRIMARY KEY, name STRING, url STRING, description STRING, timestamp STRING)";

            // Execute query
            SqliteCommand command = new SqliteCommand(sql, connection);
            command.ExecuteNonQuery();

            // Close connection
            connection.Close();
        }

        // Create Price History table
        public void CreatePriceHistoryTable()
        {
            // Open connection
            connection.Open();

            // Create table with foreign key to store items
            string sql = "CREATE TABLE IF NOT EXISTS PriceHistory (id INTEGER PRIMARY KEY, storeitemid STRING, price FLOAT, timestamp STRING, FOREIGN KEY (storeitemid) REFERENCES StoreItems(id))";

            // Execute query
            SqliteCommand command = new SqliteCommand(sql, connection);
            command.ExecuteNonQuery();

            // Close connection
            connection.Close();
        }

        // Create Rating History table
        public void CreateRatingHistoryTable()
        {
            // Open connection
            connection.Open();

            // Create table with foreign key to store items
            string sql = "CREATE TABLE IF NOT EXISTS RatingHistory (id INTEGER PRIMARY KEY, storeitemid STRING, averagerating FLOAT, numberofvotes INTEGER, timestamp STRING, FOREIGN KEY (storeitemid) REFERENCES StoreItems(id))";

            // Execute query
            SqliteCommand command = new SqliteCommand(sql, connection);
            command.ExecuteNonQuery();

            // Close connection
            connection.Close();
        }

        // Update database with store items list
        public void UpdateDatabaseWithStoreItems(List<StoreItem> storeItems)
        {
            // Insert store items into database
            foreach (StoreItem storeItem in storeItems)
            {
                // Check if store item already exists with id
                StoreItem? existingStoreItem = GetStoreItem(storeItem.Id!);
                
                // Open connection
                connection.Open();

                if (existingStoreItem == null)
                {
                    string sql = "INSERT INTO StoreItems (id, name, url, description, timestamp) VALUES (@id, @name, @url, @description, @timestamp)";

                    // Execute query
                    SqliteCommand command = new SqliteCommand(sql, connection);
                    command.Parameters.AddWithValue("@id", storeItem.Id);
                    command.Parameters.AddWithValue("@name", storeItem.Name);
                    command.Parameters.AddWithValue("@url", storeItem.Url);
                    command.Parameters.AddWithValue("@description", storeItem.Description);
                    command.Parameters.AddWithValue("@timestamp", storeItem.TimeStamp.ToString());
                    command.ExecuteNonQuery();
                }

                // If existing price is different from new price, update price history
                if (existingStoreItem == null || float.Parse(existingStoreItem.Price!) != float.Parse(storeItem.Price!))
                {
                    string sql = "INSERT INTO PriceHistory (price, timestamp, storeitemid) VALUES (@price, @timestamp, @storeitemid)";

                    // Execute query
                    SqliteCommand command = new SqliteCommand(sql, connection);
                    command.Parameters.AddWithValue("@price", storeItem.Price);
                    command.Parameters.AddWithValue("@timestamp", storeItem.TimeStamp.ToString());
                    command.Parameters.AddWithValue("@storeitemid", storeItem.Id);
                    command.ExecuteNonQuery();
                }

                // If existing rating is different from new rating, update rating history
                if (existingStoreItem == null || existingStoreItem.AverageRating != storeItem.AverageRating || existingStoreItem.NumberOfVotes != storeItem.NumberOfVotes)
                {
                    string sql = "INSERT INTO RatingHistory (averagerating, numberofvotes, timestamp, storeitemid) VALUES (@averagerating, @numberofvotes, @timestamp, @storeitemid)";

                    // Execute query
                    SqliteCommand command = new SqliteCommand(sql, connection);
                    command.Parameters.AddWithValue("@averagerating", storeItem.AverageRating);
                    command.Parameters.AddWithValue("@numberofvotes", storeItem.NumberOfVotes);
                    command.Parameters.AddWithValue("@timestamp", storeItem.TimeStamp.ToString());
                    command.Parameters.AddWithValue("@storeitemid", storeItem.Id);
                    command.ExecuteNonQuery();
                }
            }

            // Close connection
            connection.Close();
        }

        // Get store item from database by id
        public StoreItem? GetStoreItem(string id)
        {
            // Open connection
            connection.Open();

            // Get store item from database
            string sql = $"SELECT * FROM StoreItems WHERE id = {id}";

            // Execute query
            SqliteCommand command = new SqliteCommand(sql, connection);

            // Read store item from database
            SqliteDataReader reader = command.ExecuteReader();

            // if no store item found, return null
            if (!reader.Read())
            {
                connection.Close();
                return null;
            }

            // Create store item
            StoreItem storeItem = new StoreItem(
                name: reader["name"].ToString(),
                url: reader["url"].ToString(),
                description: reader["description"].ToString(),
                timeStamp: Convert.ToDateTime(reader["timestamp"])
                );

            // Get latest price from database
            sql = $"SELECT * FROM PriceHistory WHERE storeitemid = {id} ORDER BY timestamp DESC LIMIT 1";

            // Execute query
            command = new SqliteCommand(sql, connection);

            // Read price from database
            reader = command.ExecuteReader();

            // Set price
            reader.Read();

            storeItem.Price = reader["price"].ToString();

            // Get latest rating from database
            sql = $"SELECT * FROM RatingHistory WHERE storeitemid = {id} ORDER BY timestamp DESC LIMIT 1";

            // Execute query
            command = new SqliteCommand(sql, connection);

            // Read rating from database
            reader = command.ExecuteReader();

            // Set rating
            reader.Read();
            
            storeItem.AverageRating = float.Parse(reader["averagerating"].ToString()!);
            storeItem.NumberOfVotes = int.Parse(reader["numberofvotes"].ToString()!);

            // Close connection
            connection.Close();

            // Return store item
            return storeItem;
        }
    }
}