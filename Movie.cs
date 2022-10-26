class Movie
{
    // DB Properties
    public int barcode_id { get; set; }
    public int type_id { get; set; }
    public int? order_id { get; set; }
    public string? title { get; set; }
    public bool is_old { get; set; }
    public int current_stock { get; set; }
    public int price_per_day { get; set; }

    // Runtime Properties
    public bool IsInStock { get; set; } = true;

    Movie()
    {
        if (is_old) price_per_day = 29;
        else price_per_day = 49;

        if (order_id != null) IsInStock = false;
    }

    public override string ToString()
    {
        return $"{barcode_id} | {type_id} | {order_id} | {title} | {current_stock} | {price_per_day}";
    }
}