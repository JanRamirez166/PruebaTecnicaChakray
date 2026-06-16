using ML;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<BL.User>();

builder.Services.AddControllers().AddNewtonsoftJson();
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

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<BL.User>();

    initializer.Add(
        new ML.User
        {
            email = "juan@outlook.com",
            name = "Juan Perez",
            phone = "5587654321",
            password = "Juan456",
            tax_id = "ABC850624XYZ",
            addresses = new List<Address>{
                       new Address{
                        id = 1,
                        name = "Workaddress",
                        street = "Street NO 1",
                        country_code = "UK"
                       },
                       new Address{
                        id = 2,
                        name = "Homeaddress",
                        street = "Street NO 2",
                        country_code = "AU"
                       }

                }
        }
            );
    initializer.Add(
        new ML.User
        {
            email = "maria@outlook.com",
            name = "Maria Lopez",
            phone = "5544455566",
            password = "Maria789",
            tax_id = "GODE561231GR8",
            addresses = new List<Address>{
                        new Address{
                            id = 1,
                            name = "Homeaddress",
                            street = "Street NO 3",
                            country_code = "MX"
                       }
                    }
        }
        );
    initializer.Add(
         new ML.User
         {
             email = "abigail.sanchez@ejemplo.com",
             name = "Abigail Sanchez",
             phone = "5512345678",
             password = "Abigail321",
             tax_id = "ÑA&L9901011A2",
             addresses = new List<Address>
    {
        new Address
        {
            id = 1,
            name = "HomeAddress",
            street = "Av. Reforma 100",
            country_code = "MX"
        },
        new Address
        {
            id = 2,
            name = "WorkAddress",
            street = "Insurgentes Sur 250",
            country_code = "MX"
        },
        new Address
        {
            id = 3,
            name = "VacationHouse",
            street = "Ocean Drive 500",
            country_code = "US"
        }
    }
         }
        );
}

app.Run();
