static class Checkouthandler
{
    /// <summary>
    /// Creates a new order and sets up database relations for selected movies to that order
    /// </summary>
    /// <returns>The new orders ordernumber</returns> 
    public static int LoanMovies(List<Movie> selectedMovies, Customer activeCustomer)
    {
        int total_price = 0;
        foreach (var movie in selectedMovies)
        {
            total_price += movie.price_per_day;
        }

        string SqlOrderDate = DateTime.Now.ToString("yy-MM-dd HH:mm:ss");
        string SqlFinalReturnDate = DateTime.Now.AddDays(5).ToString("yy-MM-dd HH:mm:ss");
        SqlWriter.sp_InsertInto("orders", "order_number, customer_id, total_price, order_date, final_return_date", $"NULL, {activeCustomer.id}, {total_price}, '{SqlOrderDate}', '{SqlFinalReturnDate}'");

        List<Order> listofOrderNumbers = SqlWriter.sp_SelectTable<Order>("order_number", "orders");

        foreach (var movie in selectedMovies)
        {
            SqlWriter.sp_UpdateTable("movies", $"movies.order_id = {listofOrderNumbers.Last().order_number}", $"movies.barcode_id = {movie.barcode_id}");
            SqlWriter.sp_UpdateTable("movie_types", "current_stock - 1", $"id = {movie.type_id}");
        }

        return listofOrderNumbers.Last().order_number;
    }

    ///<summary>
    /// Disables relation between movies and their order and calculates any overdue price 
    ///</summary>
    ///<returns>The overdue total price</returns>
    public static int ReturnMovies(Order orderToReturn)
    {
        int returnPrice = 0;

        List<Movie> rentedMovies = SqlWriter.ExplicitSqlQuery<Movie>($"SELECT type_id, order_id, title, price_per_day " +
       $"FROM movies WHERE order_id = {orderToReturn.order_number} INNER JOIN movie_types ON movies.type_id = movie_types_id");


        if (DateTime.Now > orderToReturn.final_return_date)
        {
            foreach (var movie in rentedMovies)
            {
                string dateDifferenceString = (orderToReturn.final_return_date - DateTime.Now).TotalDays.ToString();

                int daysSinceFinalReturn = Int32.Parse(dateDifferenceString);

                if (daysSinceFinalReturn < 14) returnPrice += (movie.price_per_day * daysSinceFinalReturn);

                else returnPrice += (movie.price_per_day * 14);

                SqlWriter.sp_UpdateTable("movies", "order_id = NULL", $"barcode_id = {movie.barcode_id}");
                SqlWriter.sp_UpdateTable("movie_types", "current_stock + 1", $"id = {movie.type_id}");
            }
        }
        return returnPrice;
    }

    public static Customer TryLogin(int id)
    {
        List<Customer> listOfCustomers = SqlWriter.sp_SelectTable<Customer>("id, first_name, last_name, email, telephone_number", "customers");

        foreach (var customer in listOfCustomers)
        {
            if (id == customer.id) return customer;
        }
        return null;
    }

    public static void CreateNewCustomer(string values)
    {
        SqlWriter.sp_InsertInto("customers", values);
    }
}

