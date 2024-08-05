// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


function showLoginAlert() {
    alert("Du behöver logga in först.");
}

document.addEventListener("DOMContentLoaded", function() {
    var loginRequired = '@TempData["LoginRequired"]';
    if (loginRequired === "true") {
        showLoginAlert();
    }
});
