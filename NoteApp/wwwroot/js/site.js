// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function deleteNote(i) {
    $.ajax({
        url: 'Home/Delete',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({ id: i }),
        success: function() {
            window.location.reload();
        }
    });
}
