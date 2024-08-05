using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using NoteApp.Models;
using Microsoft.AspNetCore.Identity;
using NoteApp.Models.ViewModels;

namespace NoteApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly PasswordHasher<Account> _passwordHasher;


    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult SignIn()
    {
        return View();
    }

    public IActionResult Home()
    {
        if (HttpContext.Session.GetString("IsLogged") == "true")
        {
            var noteViewModel = GetNoteByUser();
            return View(noteViewModel);
        }
        else
        {
            TempData["LoginRequired"] = "true";
            return RedirectToAction("Index");
        }

    }

    [HttpPost]
    public IActionResult Insert(Note note)
    {

        using (SqliteConnection con =
                new SqliteConnection("Data Source=db.sqlite"))
        {
            using (var tableCmd = con.CreateCommand())
            {
                con.Open();
                tableCmd.CommandText = $"INSERT INTO Notes (Email ,Title , Text) VALUES (@email, @title, @text)";
                tableCmd.Parameters.AddWithValue("@email", HttpContext.Session.GetString("UserEmail"));
                tableCmd.Parameters.AddWithValue("@title", note.Title);
                tableCmd.Parameters.AddWithValue("@text", note.Text);
                try
                {
                    tableCmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        return RedirectToAction("Home");

    }

    [HttpPost]
    public IActionResult AddAccount(Account account)
    {
        // Hash the password

        using (SqliteConnection con =
                new SqliteConnection("Data Source=db.sqlite"))
        {
            using (var tableCmd = con.CreateCommand())
            {
                con.Open();
                tableCmd.CommandText = $"INSERT INTO Account (Email , password) VALUES (@email, @password)";
                tableCmd.Parameters.AddWithValue("@email", account.Email);
                tableCmd.Parameters.AddWithValue("@password", account.Password);
                try
                {
                    tableCmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        return RedirectToAction("Home");
    }

    [HttpPost]
    public IActionResult LogIn(Account account)
    {
        using (SqliteConnection con =
                new SqliteConnection("Data Source=db.sqlite"))
        {
            using (var tableCmd = con.CreateCommand())
            {
                con.Open();
                tableCmd.CommandText = $"SELECT * FROM Account WHERE Email = @Email AND Password = @Password";
                tableCmd.Parameters.AddWithValue("@Email", account.Email);
                tableCmd.Parameters.AddWithValue("@Password", account.Password);

                using (var reader = tableCmd.ExecuteReader())

                {
                    if (reader != null && reader.HasRows)
                    {
                        HttpContext.Session.SetString("IsLogged", "true");
                        HttpContext.Session.SetString("UserEmail", account.Email);
                        return RedirectToAction("Home");
                    }
                    else
                    {
                        // Authentication failed
                        return RedirectToAction("Index"); // Return to the login view with the provided account details and error message
                    }

                }

            }
        }

    }


    public IActionResult LogOut()
    {
        HttpContext.Session.Clear(); // Förstör sessionen
        return RedirectToAction("Index");
    }

    internal NoteViewModel GetNoteByUser()
    {
        List<Note> notes = new();

        try
        {
            using (SqliteConnection con = new SqliteConnection("Data Source=db.sqlite"))
            {
                using (var tableCmd = con.CreateCommand())
                {
                    con.Open();
                    tableCmd.CommandText = $"SELECT * FROM Notes WHERE Email = @Email";

                    tableCmd.Parameters.AddWithValue("@Email", HttpContext.Session.GetString("UserEmail"));

                    using (var reader = tableCmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                notes.Add(new Note
                                {
                                    Id = reader.GetInt32(0),
                                    Email = reader.GetString(1),
                                    Title = reader.GetString(2),
                                    Text = reader.GetString(3)
                                });
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // Hantera undantag (t.ex. logga felet)
            Console.WriteLine(ex.Message);
        }

        return new NoteViewModel
        {
            Notes = notes
        };
    }



    [HttpPost]
    public IActionResult Delete(int id)
    {

        using (SqliteConnection con = new SqliteConnection("Data Source=db.sqlite"))
        {
            con.Open();
            using (var tableCmd = con.CreateCommand())
            {
                tableCmd.CommandText = $"DELETE FROM Notes WHERE Id = @Id";
                tableCmd.Parameters.AddWithValue("@Id", id);
                try
                {
                    tableCmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        return RedirectToAction("Home");
    }

}

