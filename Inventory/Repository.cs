using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;

namespace Inventory
{
    public class Repository
    {
        private readonly SQLiteAsyncConnection conn;

        public string StatusMessage { get; set; }

        public Repository(string dbPath)
        {
            conn = new SQLiteAsyncConnection(dbPath);
            conn.CreateTableAsync<Item>().Wait();
        }

        public async Task CreateItem(Item item)
        {
            try
            {
                // Basic validation to ensure we have an item name.
                if (string.IsNullOrWhiteSpace(item.Name))
                    throw new Exception("Name is required");

                // Insert/update items.
                var result = await conn.InsertOrReplaceAsync(item).ConfigureAwait(continueOnCapturedContext: false);
                StatusMessage = $"{result} record(s) added [Item Name: {item.Name}])";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed to create item: {item.Name}. Error: {ex.Message}";
            }
        }

        public Task<List<Item>> GetAllItems()
        {
            // Return a list of items saved to the Item table in the database.
            return conn.Table<Item>().ToListAsync();
        }
    }
}