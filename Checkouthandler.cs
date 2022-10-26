class Checkouthandler
{
    /// <summary>
    /// Creates a new order and sets up relations for each selected movie to that order
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

        List<Order> listofOrders = SqlWriter.sp_SelectTable<Order>("order_number, customer_id, total_price, order_date, final_return_date", "orders");

        foreach (var movie in selectedMovies)
        {
            SqlWriter.sp_UpdateTable("movies", $"movies.order_id = {listofOrders.Last().order_number}", $"movies.barcode_id = {movie.barcode_id}");
        }

        return listofOrders.Last().order_number;
    }

    public static void ReturnMovies()
    {

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