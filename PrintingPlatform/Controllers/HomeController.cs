using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PrintingPlatform.Models;
using PrintingPlatform.Shared;

namespace PrintingPlatform.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
        {
            return View();
        }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult AccessDenied()
    {
        Response.StatusCode = 403;
        return View();
    }

     public IActionResult Error404()
    {
        Response.StatusCode = 404;
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
