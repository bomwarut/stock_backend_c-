namespace WebApplication1.Database
{
    public class ConnectDb
    {
        public MySql.Data.MySqlClient.MySqlConnection Connect() {
            MySql.Data.MySqlClient.MySqlConnection conn = new MySql.Data.MySqlClient.MySqlConnection();
            string myConnectionString;

            myConnectionString = "server=localhost;uid=root;" +
                "pwd=;database=stock_manage";
            try
            {
                conn = new MySql.Data.MySqlClient.MySqlConnection();
                conn.ConnectionString = myConnectionString;
                conn.Open();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                Console.WriteLine( ex.Message);
            }

            return conn;
        }
    }
}
