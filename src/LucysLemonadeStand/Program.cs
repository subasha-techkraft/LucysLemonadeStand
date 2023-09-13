using FastEndpoints;
using FastEndpoints.Swagger;
using FastEndpoints.Swagger.Swashbuckle;
using LucysLemonadeStand;
using LucysLemonadeStand.Core;
using LucysLemonadeStand.Infrastructure;
using LucysLemonadeStand.SharedKernel;
using NJsonSchema.Generation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<DateTimeProvider>();
builder.Services.AddDatabase(); //my extension method
builder.Services.AddMomClient(builder.Configuration);
builder.Services.AddRepositories(); //my extension method
builder.Services.AddRepositoriesWithInfrastructure();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//This custom name generator trims off a lot of unnecessary parts of schema names and operation IDs that come from the namespace.
//If available, it also appends version numbers from the namespace to the end.
//E.g. SphereTaskAPIWebEndpointsV1ConnectorEndpointsStartNewScansAsync => StartNewScansV1Async, SphereTaskAPIWebEndpointsV1ConnectorEndpointsStartNewScansRequest => StartNewScansRequestV1
CustomSchemaNameGenerator _schemaNameGenerator = new();
builder.Services.AddSingleton<ISchemaNameGenerator>(_schemaNameGenerator);

builder.Services.AddFastEndpoints();

builder.Services.AddSwaggerDoc(maxEndpointVersion: 1, settings: openApiInfo =>
{
    openApiInfo.DocumentName = "Lucy's Lemonade Stand API v1.0";
    openApiInfo.Title = "Lucy's Lemonade Stand API v1.0";
    openApiInfo.Version = "v1.0"; //this should not be updated by a script
    openApiInfo.Description = "This API provides operations relating to the activities of Lucy's Lemonade Stand.";
    openApiInfo.SchemaNameGenerator = _schemaNameGenerator;
});

builder.Services.AddSwaggerGen(c =>
{
    c.OperationFilter<FastEndpointsOperationFilter>();
});

var app = builder.Build();

app.UseFastEndpoints(options =>
{
    options.Versioning.DefaultVersion = 1;
    options.Versioning.Prefix = "v";
    options.Versioning.PrependToRoute = true;
    options.Endpoints.Configurator = (EndpointDefinition ep) =>
    {
        //override the normal long operation ID generation with a custom operation ID generator.
        //This allows generated client code to have simpler method signatures.
        //FastEndpoints operation IDs are defined in a different place than swagger would normally do it.
        ep.Description(x => x.WithName(_schemaNameGenerator.Generate(ep.EndpointType)));
        /* INFO (from Fast Endpoints docs):
        When you manually specify a name for an endpoint like above and you want to point to that endpoint when using SendCreatedAtAsync() method, you must use the overload that takes a string argument with which you can specify the name of the target endpoint. I.e. you lose the convenience/type-safety of being able to simply point to another endpoint using the class type like so:
          await SendCreatedAtAsync<GetInvoiceEndpoint>(...);
        Instead you must do this:
          await SendCreatedAtAsync("GetInvoice", ...);
        */
    };
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    
}

app.UseHttpsRedirection();

// Enable middleware to serve generated Swagger as a JSON endpoint.
app.UseSwaggerGen();

app.Run();

//This is used to allow a Testing suite to run the app using WebApplicationFactory<Program>
public partial class Program { }