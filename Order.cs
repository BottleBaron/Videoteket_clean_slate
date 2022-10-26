class Order
{
    public int order_number { get; set; }
    public int customer_id { get; set; }
    public int total_price { get; set; }
    public DateTime order_date { get; set; }
    public DateTime final_return_date { get; set; }

    public override string ToString()
    {
        return $"{order_number} | {customer_id} | {total_price} | {order_date.ToShortDateString()} | {final_return_date.ToShortDateString()}";
    }
}