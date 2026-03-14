using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSqlServer<ApplicationDbContext>(builder.Configuration["ConnectionStrings:SqlServer"]);

// Add services to the container.

builder.Services.AddControllers();
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

app.MapPost("/products", (ProductDto request, ApplicationDbContext context) =>
{
    var category = context.Categories.FirstOrDefault(c => c.Id == request.CategoryId);

    if (category == null)
    {
        return Results.BadRequest($"Category with id {request.CategoryId} does not exist.");
    }

    var product = new Product
    {
        Name = request.Name,
        Code = request.Code,
        Description = request.Description,
        Category = category
    };

    if (request.Tags != null)
    {
        product.Tags = [];

        foreach (var tag in request.Tags)
        {
            product.Tags.Add(new Tag { Name = tag });
        }
    }

    context.Products.Add(product);
    context.SaveChanges();

    return Results.Created($"/products/{product.Id}", product.Id);
});

app.MapGet("/products/{id}", ([FromRoute] int id, ApplicationDbContext context) =>
{
    var product = context.Products
        .Include(p => p.Category)
        .Include(p => p.Tags)
        .FirstOrDefault(p => p.Id == id && p.Active);

    if (product == null)
    {
        return Results.NotFound();
    }
    
    return Results.Ok(product);
});

app.MapGet("/products", (ApplicationDbContext context) =>
{
    var products = context.Products
        .Where(p => p.Active)
        .Include(p => p.Category)
        .Include(p => p.Tags)
        .ToList();

    return Results.Ok(products);
});

app.MapPut("/products/{id}", ([FromRoute] int id, ProductDto request, ApplicationDbContext context) => {
    var product = context.Products
        .Include(p => p.Tags)
        .FirstOrDefault(p => p.Id == id);

    if (product == null)
    {
        return Results.NotFound();
    }

    product.Code = request.Code;
    product.Name = request.Name;
    product.Description = request.Description;

    var category = context.Categories.FirstOrDefault(c => c.Id == request.CategoryId);
    
    if (category == null)
    {
        return Results.BadRequest($"Category with id {request.CategoryId} does not exist.");
    }

    product.Category = category;

    if (request.Tags != null)
    {
        product.Tags.Clear();

        foreach (var tag in request.Tags)
        {
            product.Tags.Add(new Tag { Name = tag });
        }
    }

    context.SaveChanges();
    return Results.Ok();
});

app.MapDelete("/products/{id}", ([FromRoute] int id, ApplicationDbContext context) =>
{
    var product = context.Products.FirstOrDefault(p => p.Id == id);

    if (product == null)
    {
        return Results.NotFound();
    }

    product.Active = false;

    context.SaveChanges();
    return Results.Ok();
});

app.Run();