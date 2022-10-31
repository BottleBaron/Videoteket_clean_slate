class Customer
{
    public int id { get; set; }
    public string? first_name { get; set; }
    public string? last_name { get; set; }
    public string? email { get; set; }
    public string? telephone_number { get; set; }

    // Runtime Properties
    public List<Movie> RentedMovies = new();


    public override string ToString()
    {
        return $"{id} | {first_name} | {last_name} | {email} | {telephone_number}";
    }

    public List<Movie> GetLoanedMovies()
    {
        List<Movie> loanedMovies = new();

        List<Order> listOfOrderIds = SqlWriter.sp_SelectTable<Order>("order_number, customer_id", "orders");
        List<Movie> listOfMovies = SqlWriter.sp_InnerJoinTables<Movie>(
        "barcode_id, customer_id, order_id",
        "title, is_old, current_stock, price_per_day",
        "movies", "movietypes", "movies.type_id = movie_types.id"
        );

        foreach (var order in listOfOrderIds)
        {
            if (order.customer_id == id)
            {
                foreach (var movie in listOfMovies)
                {
                    if (movie.order_id == order.order_number && !movie.IsInStock) loanedMovies.Add(movie);
                }
            }
        }

        foreach (var movie in loanedMovies)
        {
            Order orderForMovie = SqlWriter.sp_SelectObject<Order>("final_return_date", "order", "order_number", movie.order_id);

            if (DateTime.Now > orderForMovie.final_return_date) { } // FIX: Mark as overdue and send bill
        }

        return loanedMovies;
    }
}