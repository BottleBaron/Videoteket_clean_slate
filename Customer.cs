namespace Videoteket_clean_slate;

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

    public List<Order> GetOrders()
    {
        List<Order> orders = SqlWriter.sp_SelectTable<Order>("order_number, customer_id, total_price, order_date, final_return_date, is_returned", 
            $"orders WHERE customer_id = {id}");

        return orders;
    }
}