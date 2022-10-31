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

}