class GUI
{
    private Checkouthandler checkouthandler = new();
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
                else checkouthandler.TryLogin(newId);
            }
            // Try Login With ID
            else
            {
                int inputID = Int32.Parse(input);

                activeCustomer = checkouthandler.TryLogin(inputID);

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
                //LoanMovieMenu();
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

            checkouthandler.CreateNewCustomer(Sqlstring);
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
        "movies", "movietypes", "movies.type_id = movie_types.id"
        );
        List<Movie> displayChoices = SqlWriter.sp_SelectTable<Movie>("id, title, price_per_day", "movie_types");

        while (true)
        {
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
            foreach (var movie in selectedMovies)
            {
                if (number == movie.type_id) selections.Remove(number);
            }
        }
    }
}