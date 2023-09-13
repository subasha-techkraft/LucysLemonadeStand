using Mom;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

float chanceOf429 = 0f;

app.MapGet("/makelemonade", (float cash, HttpContext context) =>
{
    if (cash < 3.20)
    {
        return Results.BadRequest("Cash provided must be $3.20 for a pitcher.");
    }
    if (Random.Shared.NextSingle() <= chanceOf429) //10 % chance of failure
    {
        chanceOf429 = 0f;
        var response = Results.StatusCode((int)HttpStatusCode.TooManyRequests);
        context.Response.Headers.Add("Retry-After", 5.ToString());
        return response;
    }
    chanceOf429 += 0.1f;
    return Results.Ok(new Pitcher (){ Cups = 8 });
})
.WithName("MakeLemonade")
.WithSummary("Ask tired mom to make a pitcher of lemonade for $3.20.")
.WithDescription("""
This is the endpoint for mom to make a pitcher of lemonade. 
She requires at least $3.20 in cash per pitcher.
She will keep the change so do your math properly.
She gets more and more tired after every request. 
When she needs a break she will return a 429 Too Many Requests response and will need a short break.
""")
.Produces<Pitcher>()
.Produces((int)HttpStatusCode.BadRequest)
.Produces((int)HttpStatusCode.TooManyRequests)
.WithOpenApi(c =>
{
    c.Parameters[0].Description = "How much money was given to mom to make the lemonade.";
    c.Responses[((int)HttpStatusCode.OK).ToString()].Description = "A successfully made pitcher of 8 cups.";
    c.Responses[((int)HttpStatusCode.BadRequest).ToString()].Description = "Mom needs at least $3.20 per pitcher to break even.";
    c.Responses[((int)HttpStatusCode.TooManyRequests).ToString()].Description = "Too many requests for lemonade. Mom needs a break.";
    return c;
});

app.Run();