class GUI
{
    private Customer activeCustomer = new();

    public void Init()
    {
        Console.WriteLine("Welcome to -<Videoteket>- \nPress any button to login...");
        Console.ReadKey();

        while (true)
        {
            Console.Clear();
            Console.Write("Please enter your id, [Q] to quit or [S] to sign Up: ");
            string? input = Console.ReadLine().ToLower();

            if (input == "q") Environment.Exit(0);

            // Create new account and login with new id
            else if (input == "s")
            {
                int newId = SignUpMenu();

                if (newId == 0) continue;
                else Checkouthandler.TryLogin(newId);
            }
            // Try Login With ID
            else
            {
                int inputID = Int32.Parse(input);

                activeCustomer = Checkouthandler.TryLogin(inputID);

                if (activeCustomer == null) continue;
                else break;
            }
        }

        MainMenu();
    }

    private void MainMenu()
    {
        while (true)
        {
            string[] mainmenu = new string[]
            {
                "----<VIDEOTEKET>----",
                "[1] Loan a movie",
                "[2] Return movies",
                "[3] Quit"
            };
            foreach (var line in mainmenu)
            {
                Console.WriteLine(line);
            }
            var keyPress = Console.ReadKey();

            if (keyPress.Key == ConsoleKey.D1)
            {
                List<Movie> selections = LoanMovieMenu();

                if(selections.Count > 0)
                {
                    int newOrderId = Checkouthandler.LoanMovies(selections, activeCustomer);

                    Order newOrder = SqlWriter.sp_SelectObject<Order>("order_number, customer_id, total_price, order_date, final_return_date", "orders", "order_number", newOrderId);

                    string recieptString = PrintReciept(newOrder, false);
                }  
            }
            else if (keyPress.Key == ConsoleKey.D2)
            {
                //ReturnMoviemenu();
            }
            else if (keyPress.Key == ConsoleKey.D3)
            {
                Environment.Exit(0);
            }
        }
    }

    private int SignUpMenu()
    {
        List<string> customerData = new();

        Console.Clear();
        Console.Write("Please enter your first name: ");
        customerData.Add(Console.ReadLine());

        Console.Write("Please enter your last name: ");
        customerData.Add(Console.ReadLine());

        Console.Write("Please enter your email: ");
        customerData.Add(Console.ReadLine());

        Console.WriteLine("Please enter your telephone number: ");
        customerData.Add(Console.ReadLine());

        string Sqlstring = SqlWriter.FormatIntoSqlString(customerData);

        Console.Clear();
        if (Sqlstring != null)
        {
            Console.Write("Success! Account created.\nPress any key to log into your new account:");
            Console.ReadKey();

            Checkouthandler.CreateNewCustomer(Sqlstring);
            List<Customer> listOfCustomerIds = SqlWriter.sp_SelectTable<Customer>("id", "customers");
            return listOfCustomerIds.Last().id;
        }
        else
        {
            Console.WriteLine("Error: You used exempted characters or terms such as '*' or DROP");
            Console.ReadKey();
            return 0;
        }
    }

    private List<Movie> LoanMovieMenu()
    {
        List<Movie> selectedMovies = new();
        List<int> selections = new();
        List<Movie> listAllMovies = SqlWriter.sp_InnerJoinTables<Movie>(
        "barcode_id, customer_id, order_id",
        "title, is_old, current_stock, price_per_day",
        "movies", "movie_types", "movies.type_id = movie_types.id"
        );
        List<Movie> displayChoices = SqlWriter.sp_SelectTable<Movie>("id, title, price_per_day", "movie_types");

        while (true)
        {
            Console.WriteLine("Please select a movie to loan. You may loan a maximum of one copy of each movie:");
            foreach (var movietype in displayChoices)
            {
                Console.WriteLine($"[{movietype.type_id}]. {movietype.title} | {movietype.current_stock} |");
            }
            Int32.TryParse(Console.ReadLine(), out int result);
            selections.Add(result);

        anchor:

            Console.WriteLine("Would you like to add another movie? (Y/N)");
            var keyPress = Console.ReadKey();

            if (keyPress.Key == ConsoleKey.Y) continue;
            else if (keyPress.Key == ConsoleKey.N) break;
            else goto anchor;
        }

        foreach (var number in selections)
        {
            // Removes movie doublettes
            foreach (var movie in selectedMovies)
            {
                if (number == movie.type_id) selections.Remove(number);
            }

            foreach (var movie in listAllMovies)
            {
                if (movie.current_stock > 0 && number == movie.type_id)
                    selectedMovies.Add(movie);
            }
        }

        return selectedMovies;
    }

    private string PrintReciept(Order orderToPrint, bool isReturn)
    {
        string recieptString = "-- RECIEPT --\n";

        recieptString += $"ORDER NUMBER: {orderToPrint.order_number}";
  
        recieptString += $"\nORDER DATE: {orderToPrint.order_date} || FINAL RETURN DATE: {orderToPrint.final_return_date}\n"; 

        List<Movie> rentedMovies = SqlWriter.ExplicitSqlQuery<Movie>($"SELECT type_id, order_id, title, price_per_day " +
        $"FROM movies WHERE order_id = {orderToPrint.order_number} INNER JOIN movie_types ON movies.type_id = movie_types_id");

        foreach (var movie in rentedMovies)
        {
            recieptString += $"[{movie.type_id}] {movie.title} a. {movie.price_per_day}\n";
        }

        recieptString += $"\nTOTAL PRICE: {orderToPrint.total_price}\n";

        if(isReturn) recieptString += "RETURNED [x]";
        else recieptString += "RETURNED [x]";

        return recieptString;
    }
}