namespace Videoteket_clean_slate;

class Movie : Movietype
{
    // DB Properties
    public int barcode_id { get; set; }
    public int type_id { get; set; }
    public int? order_id { get; set; }


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