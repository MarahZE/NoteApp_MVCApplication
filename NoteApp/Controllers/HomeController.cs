using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using NoteApp.Models;

namespace NoteApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

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
        return View();
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
                tableCmd.CommandText = $"INSERT INTO Note (Title , Text) VALUES (@title, @text)";
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
        return RedirectToAction("Index");

    }

    [HttpPost]
    public IActionResult AddAccount(Account account)
    {
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
                        return RedirectToAction("Home");
                    }
                    else
                    {
                        // Authentication failed
                        //ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                        return RedirectToAction("Index"); // Return to the login view with the provided account details and error message
                    }

                }

            }
        }

    }
}

