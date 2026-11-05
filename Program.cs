using System.Text.Encodings.Web;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseStaticFiles();

app.MapGet("/view/{token}", (HttpRequest request, string token) =>
{
    // Sample gallery information for the POC
    var title = "Ananya & Rahul | Wedding Gallery";
    var description = "Relive the beautiful moments from Ananya and Rahul's wedding.";

    var pageUrl = $"{request.Scheme}://{request.Host}/view/{Uri.EscapeDataString(token)}";
    var imageUrl = $"{request.Scheme}://{request.Host}/images/wedding-thumbnail.jpg";

    var safeTitle = HtmlEncoder.Default.Encode(title);
    var safeDescription = HtmlEncoder.Default.Encode(description);
    var safePageUrl = HtmlEncoder.Default.Encode(pageUrl);
    var safeImageUrl = HtmlEncoder.Default.Encode(imageUrl);

    var html = $$"""
    <!DOCTYPE html>
    <html lang="en">
    <head>
        <meta charset="UTF-8">
        <meta name="viewport" content="width=device-width, initial-scale=1.0">

        <title>{{safeTitle}}</title>
        <meta name="description" content="{{safeDescription}}">

        <!-- Open Graph metadata for WhatsApp link previews -->
        <meta property="og:type" content="website">
        <meta property="og:title" content="{{safeTitle}}">
        <meta property="og:description" content="{{safeDescription}}">
        <meta property="og:image" content="{{safeImageUrl}}">
        <meta property="og:image:width" content="1200">
        <meta property="og:image:height" content="630">
        <meta property="og:url" content="{{safePageUrl}}">

        <meta name="twitter:card" content="summary_large_image">
    </head>
    <body>
        <h1>{{safeTitle}}</h1>
        <p>{{safeDescription}}</p>
        <img src="{{safeImageUrl}}" alt="Wedding gallery thumbnail"
             style="max-width: 600px; width: 100%;">
    </body>
    </html>
    """;

    return Results.Content(html, "text/html");
});

app.Run();