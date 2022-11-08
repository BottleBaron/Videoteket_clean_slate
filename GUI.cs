
using System.Threading.Channels;

namespace Videoteket_clean_slate;

class GUI
{
    private Customer activeCustomer = new();

    public void Init()
    {
        Console.Clear();
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
            Console.Clear();
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

            switch (keyPress.Key)
            {
                case ConsoleKey.D1:
                {
                    List<Movie> selections = LoanMovieMenu();

                    if(selections.Count > 0)
                    {
                        int newOrderId = Checkouthandler.LoanMovies(selections, activeCustomer);

                        Order newOrder = SqlWriter.sp_SelectObject<Order>("order_number, customer_id, total_price, order_date, final_return_date", "orders", "order_number", newOrderId);

                        string recieptString = PrintReciept(newOrder);

                        Console.WriteLine(recieptString);
                        Console.ReadKey();
                    }
                    break;
                }
                case ConsoleKey.D2:
                    ReturnMoviesMenu();
                    break;
                case ConsoleKey.D3:
                    Environment.Exit(0);
                    break;
            }
        }
    }

    private void ReturnMoviesMenu()
    {
        List<Order> yourOrders = activeCustomer.GetOrders();

        Console.WriteLine("Select an order to return.");
        foreach (var order in yourOrders)
        {
            if (!order.is_returned)
            {
                Console.WriteLine(PrintReciept(order));
                Console.WriteLine("-----------------------------------");
            }
        }

        Console.Write("ORDER NUMBER: ");
        var input = Console.ReadLine();

        foreach (var order in yourOrders)
        {
            if (input == order.order_number.ToString() && !order.is_returned)
            {
                int? debitedPrice = Checkouthandler.ReturnMovies(order);
                Console.WriteLine($"Success. Your order was returned! {debitedPrice} kr in overdue price will be debited to your account");
                Console.ReadKey();
                break;
            }
        }
    }

    private int SignUpMenu()
    {
        List<string> customerData = new();

        Console.Clear();
        Console.Write("Please enter your first name: ");
        customerData.Add("'" + Console.ReadLine() + "'");

        Console.Write("Please enter your last name: ");
        customerData.Add("'" + Console.ReadLine() + "'");

        Console.Write("Please enter your email: ");
        customerData.Add("'" + Console.ReadLine() + "'");

        Console.Write("Please enter your telephone number: ");
        customerData.Add("'" + Console.ReadLine() + "'");

        string? sqlString = SqlWriter.FormatIntoSqlString(customerData);

        Console.Clear();
        if (sqlString != null)
        {
            Console.Write("Success! Account created.\nPress any key to log into your new account:");
            Console.ReadKey();

            Checkouthandler.CreateNewCustomer(sqlString);
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
            "barcode_id, type_id, order_id",
            "title, is_old, current_stock, price_per_day",
            "movies", "movie_types", "movies.type_id = movie_types.id"
        );
        
        List<Movietype> displayChoices = SqlWriter.sp_SelectTable<Movietype>("id, title, price_per_day, current_stock", "movie_types");

        while (true)
        {
            Console.WriteLine("Please select a movie to loan. You may loan a maximum of one copy of each movie:");
            foreach (var movietype in displayChoices)
            {
                if(movietype.current_stock > 0)
                    Console.WriteLine($"[{movietype.id}]. {movietype.title} | {movietype.price_per_day} kr / day | Stock: {movietype.current_stock} |");
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

        var singleSelectionsList = selections.Distinct().ToList();
        
        foreach ( var number in singleSelectionsList)
        {
            foreach (var movie in listAllMovies)
            {
                if (movie.current_stock > 0 && number == movie.type_id)
                {
                    selectedMovies.Add(movie);
                    break;
                }
            }
        }

        return selectedMovies;
    }

    private string PrintReciept(Order orderToPrint)
    {
        string recieptString = "-- RECIEPT --\n";

        recieptString += $"ORDER NUMBER: {orderToPrint.order_number}";
  
        recieptString += $"\nORDER DATE: {orderToPrint.order_date} || FINAL RETURN DATE: {orderToPrint.final_return_date}\n"; 

        List<Movie> rentedMovies = SqlWriter.ExplicitSqlQuery<Movie>($"SELECT type_id, order_id, title, price_per_day " +
                                                                     $"FROM movies INNER JOIN movie_types ON movies.type_id = movie_types.id WHERE order_id = {orderToPrint.order_number}");

        foreach (var movie in rentedMovies)
        {
            recieptString += $"[{movie.type_id}] {movie.title} a. {movie.price_per_day}\n";
        }

        recieptString += $"\nTOTAL PRICE: {orderToPrint.total_price}\n";

        if(orderToPrint.is_returned) recieptString += "RETURNED [x]";
        else recieptString += "RETURNED [ ]";

        return recieptString;
    }
}