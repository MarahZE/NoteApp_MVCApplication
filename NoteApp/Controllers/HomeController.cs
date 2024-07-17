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
}

