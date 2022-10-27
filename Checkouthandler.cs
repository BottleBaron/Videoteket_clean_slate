class Checkouthandler
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
        }

        return listofOrderNumbers.Last().order_number;
    }

    ///<summary>
    /// Disables relation between selectedmovies and their order and calculates any overdue price 
    ///</summary>
    ///<returns>The overdue total price</returns>
    public static int ReturnMovies(List<Movie> selectedMovies)
    {
        int returnPrice = 0;

        List<Order> listOfOrders = SqlWriter.sp_SelectTable<Order>("order_number, final_return_date", "orders");

        foreach (var movie in selectedMovies)
        {
            foreach (var order in listOfOrders)
            {
                if (movie.order_id == order.order_number)
                {
                    if (DateTime.Now > order.final_return_date)
                    {
                        string dateDifferenceString = (order.final_return_date - DateTime.Now).TotalDays.ToString();

                        int daysSinceFinalReturn = Int32.Parse(dateDifferenceString);

                        if (daysSinceFinalReturn < 14) returnPrice += (movie.price_per_day * daysSinceFinalReturn);

                        else returnPrice += (movie.price_per_day * 14);
                    }
                }
            }

            SqlWriter.sp_UpdateTable("movies", "order_id = NULL", $"barcode_id = {movie.barcode_id}");
        }

        return returnPrice;
    }

    public Customer TryLogin(int id)
    {
        List<Customer> listOfCustomers = SqlWriter.sp_SelectTable<Customer>("id, first_name, last_name, email, telephone_number", "customers");

        foreach (var customer in listOfCustomers)
        {
            if (id == customer.id) return customer;
        }
        return null;
    }

    public void CreateNewCustomer(string values)
    {
        SqlWriter.sp_InsertInto("customers", values);
    }
}

