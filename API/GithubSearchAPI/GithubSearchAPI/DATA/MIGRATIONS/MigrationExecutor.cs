using System.Data.SqlClient;

namespace GithubSearchAPI.DATA.MIGRATIONS
{
    public class MigrationExecutor
    {
        private readonly string _connectionString;

        public MigrationExecutor(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void RunMigration(string scriptPath, string password)
        {
            try
            {
                string script = File.ReadAllText(scriptPath);

                // Replace the placeholder password dynamically
                script = script.Replace("TemporaryPassword123!", password);

                // Split the script into batches using "GO"
                string[] batches = script.Split(new[] { "GO" }, StringSplitOptions.RemoveEmptyEntries);

                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    foreach (var batch in batches)
                    {
                        using (var command = new SqlCommand(batch, connection))
                        {
                            command.ExecuteNonQuery();
                        }
                    }
                }

                Console.WriteLine("Database migration completed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during migration: {ex.Message}");
                throw;
            }
        }

    }
}
