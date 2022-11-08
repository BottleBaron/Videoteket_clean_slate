namespace Videoteket_clean_slate;

class Movietype
{
    public int? id { get; set; }
    public string? title { get; set; }
    public bool is_old { get; set; }
    public int current_stock { get; set; }
    public int price_per_day { get; set; }
}