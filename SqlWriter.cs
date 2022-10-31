using Dapper;
using MySqlConnector;
static class SqlWriter
{
    private static MySqlConnection DBConnection()
    {
        var connection = new MySqlConnection("Server=localhost;Database=videoteket;Uid=root;Pwd=samsis123");
        return connection;
    }

    // ---------------
    // SELECT METHODS
    // ---------------

    public static List<T> ExplicitSqlQuery<T>(string query)
    {
        using (var connection = DBConnection())
        {
            try
            {
                var returnValue = connection.Query<T>(query).ToList();
                return returnValue;
            }
            catch (Exception e)
            {
                throw e;
            }

        }
    }

    /// <summary>
    /// SELECT ´columns´ FROM ´table´ WHERE ´identifyer´ = ´idValue´;
    /// </summary>
    public static T sp_SelectObject<T>(string columns, string table, string identifyer, int? idValue)
    {
        string query = $"SELECT {columns} FROM {table} WHERE {identifyer} = {idValue}";

        using (var connection = DBConnection())
        {
            try
            {
                T selectedObject = connection.QuerySingle<T>(query);

                return selectedObject;
            }
            catch (Exception e)
            {

                throw e;
            }
        }
    }

    /// <summary>
    /// SELECT ´columns´ FROM ´table´;
    /// </summary>
    public static List<T> sp_SelectTable<T>(string columns, string table)
    {
        string query = $"SELECT {columns} FROM {table}";

        using (var connection = DBConnection())
        {
            try
            {
                var result = connection.Query<T>(query).ToList();

                return result;
            }
            catch (Exception e)
            {

                throw e;
            }
        }
    }

    /// <summary>
    /// SELECT ´columns´ FROM ´table´ INNER JOIN ´jointable´ ON ´joinparameter´;
    /// </summary>
    public static List<T> sp_InnerJoinTables<T>(string columns, string joincolumns, string table, string jointable, string joinparameter)
    {
        string query = $"SELECT {columns}, {joincolumns} FROM {table} INNER JOIN {jointable} ON {joinparameter}";
        using (var connection = DBConnection())
        {
            try
            {
                var type = connection.Query<T>(query).ToList();

                return type;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }

    //----------------
    // MODIFYING METHODS
    //----------------

    /// <summary>
    /// INSERT INTO ´table´ VALUES (´values´)
    /// </summary>
    public static void sp_InsertInto(string table, string values)
    {
        string query = $"INSERT INTO {table} VALUES ({values});";
        Console.WriteLine(query);

        using (var connection = DBConnection())
        {
            try
            {
                connection.Execute(query);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }

    /// <summary>
    /// INSERT INTO ´table´ (´columns´) VALUES (´values´)
    /// </summary>
    public static void sp_InsertInto(string table, string columns, string values)
    {
        string query = $"INSERT INTO {table} ({columns}) VALUES ({values});";
        Console.WriteLine(query);

        using (var connection = DBConnection())
        {
            try
            {
                connection.Execute(query);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }

    /// <summary>
    /// UPDATE ´table´ SET ´parameters´ WHERE ´condition´
    /// </summary>
    public static void sp_UpdateTable(string table, string parameters, string condition)
    {
        string query = $"UPDATE {table} SET {parameters} WHERE {condition};";

        using (var connection = DBConnection())
        {
            try
            {
                connection.Execute(query);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }

    ///---------------------
    /// FORMATTING METHODS
    ///---------------------

    /// <summary>
    /// Formats string values into a single SQL-readable identifyer string
    /// </summary>
    public static string FormatIntoSqlString(List<string> values)
    {
        string? formattedString = "";

        foreach (var input in values)
        {
            if (ContainsExemptedValues(input)) return null;

            formattedString += input + ',';
        }
        return formattedString;
    }

    ///<summary>
    /// Checks a string for any exempted values eg. '*' or "DROP"
    ///</summary>
    ///<returns>
    /// True: The string contains exempted values,
    /// False: The string does not contain exempted values
    ///</returns>
    public static bool ContainsExemptedValues(string s)
    {
        char[] exemptedChars = new char[] { ',', '*', '<', '>', '=' };
        string[] exemptedStrings = new string[] { "SELECT", "INSERT", "DROP", "ALTER", "UPDATE", "DELETE", "CREATE" };

        foreach (var character in exemptedChars)
        {
            if (s.Contains(character)) return true;
        }
        foreach (var keyword in exemptedStrings)
        {
            if (s.ToUpper().Contains(keyword)) return true;
        }

        return false;
    }
}