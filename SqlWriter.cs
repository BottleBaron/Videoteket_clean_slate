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

    public static void ExplicitSqlQuery(string query)
    {
        using (var connection = DBConnection())
        {
            try
            {
                connection.Query(query);
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
    public static T sp_SelectObject<T>(string columns, string table, string identifyer, int idValue)
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
    public static List<T> sp_InnerJoinTables<T>(string table, string columns, string jointable, string joincolumns, string joinparameter)
    {
        string query = $"SELECT {columns}, {joincolumns} FROM {table} INNER JOIN {jointable} ON {joinparameter}";
        using (var connection = new MySqlConnection("Server=localhost;Database=videoteket;Uid=root;Pwd=samsis123"))
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

    ///<summary>
    /// INSERT INTO ´table´ VALUES (´values´)
    ///</summary>
    public static void sp_InsertInto(string table, string values)
    {
        string query = $"INSERT INTO {table} VALUES ({values});";
        Console.WriteLine(query);

        using (var connection = new MySqlConnection("Server=localhost;Database=videoteket;Uid=root;Pwd=samsis123"))
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

    ///<summary>
    /// INSERT INTO ´table´ (´columns´) VALUES (´´)
    ///</summary>
    public static void sp_InsertInto(string table, string columns, string values)
    {
        string query = $"INSERT INTO {table} ({columns}) VALUES ({values});";
        Console.WriteLine(query);

        using (var connection = new MySqlConnection("Server=localhost;Database=videoteket;Uid=root;Pwd=samsis123"))
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

    ///<summary>
    /// UPDATE ´table´ SET ´parameters´ WHERE ´condition´
    ///</summary>
    public static void sp_UpdateTable(string table, string parameters, string condition)
    {
        string query = $"UPDATE {table} SET {parameters} WHERE {condition};";

        using (var connection = new MySqlConnection("Server=localhost;Database=videoteket;Uid=root;Pwd=samsis123"))
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
}