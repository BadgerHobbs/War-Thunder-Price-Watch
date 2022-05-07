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
            connection = new SqliteConnection("Data Source=" + filename); 
        }

        // Create Store Items table
        public void CreateStoreItemsTable()
        {
            // Open connection
            connection.Open();

            // Create table
            string sql = "CREATE TABLE IF NOT EXISTS StoreItems (id INTEGER PRIMARY KEY, name STRING, price FLOAT, url STRING, image STRING, description STRING, category STRING, averagerating FLOAT, numberofvotes INTEGER, timestamp STRING)";

            // Execute query
            SqliteCommand command = new SqliteCommand(sql, connection);
            command.ExecuteNonQuery();

            // Close connection
            connection.Close();
        }

        // Insert store items into database from list
        public void InsertStoreItems(List<StoreItem> storeItems)
        {
            // Open connection
            connection.Open();

            // Insert store items into database
            foreach (StoreItem storeItem in storeItems)
            {
                string sql = "INSERT INTO StoreItems (id, name, price, Url, image, description, category, averagerating, numberofvotes, timestamp) VALUES (@id, @name, @price, @url, @image, @description, @category, @averagerating, @numberofvotes, @timestamp)";

                // Execute query
                SqliteCommand command = new SqliteCommand(sql, connection);
                command.Parameters.AddWithValue("@id", storeItem.Id);
                command.Parameters.AddWithValue("@name", storeItem.Name ?? "");
                command.Parameters.AddWithValue("@price", storeItem.Price ?? "");
                command.Parameters.AddWithValue("@url", storeItem.Url);
                command.Parameters.AddWithValue("@image", storeItem.Image ?? "");
                command.Parameters.AddWithValue("@description", storeItem.Description ?? "");
                command.Parameters.AddWithValue("@category", storeItem.Category ?? "");
                command.Parameters.AddWithValue("@averagerating", storeItem.AverageRating.ToString() ?? "");
                command.Parameters.AddWithValue("@numberofvotes", storeItem.NumberOfVotes.ToString() ?? "");
                command.Parameters.AddWithValue("@timestamp", storeItem.TimeStamp.ToString() ?? "");
                command.ExecuteNonQuery();
            }

            // Close connection
            connection.Close();
        }
    }
}