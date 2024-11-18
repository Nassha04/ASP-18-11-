using Homework2Library;
using DatabaseLogic;
using System.Security.Claims;


var builder = WebApplication.CreateBuilder(args);

// NEED THIS (to allow all operations for client to make request to cloud services like firebase, a ruleset for application to talk to each other)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder
            .AllowAnyMethod()
            .AllowAnyHeader()
            .SetIsOriginAllowed(origin => true) // allow any origin
            .AllowCredentials();
        });
});

// Add services to the container.

var app = builder.Build();

DatabaseAccountrix _databaselogic = new DatabaseAccountrix();
_databaselogic.initDatabase();

//routing codes
//test code
app.MapGet("/", () => "Connection successful!");

//map to endpoint for data saving 


//in  /register endpoint, this will be the API process carried out (for saving userdetail into firebase in the /register page)
app.MapPost("/register", async (UserDetail a_userdetail) =>
{
    await _databaselogic.saveUserData(a_userdetail);
    Console.WriteLine("User data saved successfully");
    return Results.NoContent();
});
//to check email if used or not, fetches userdata with particular user_ID from database, then checks the info of the fetched data from database with the one that current user created, if same email == not valid 
app.MapGet("/register/{user_ID}", async (string a_user_ID) =>
{
    UserDetail user_fetched = await _databaselogic.retrieveUserEmail(a_user_ID);
    return Results.Ok(user_fetched);
});


app.MapPost("/register/{user_ID}", async (string a_user_ID) =>
{
    await _databaselogic.deleteUserData(a_user_ID);
    return Results.NoContent();
});

//in /login endpoint , this will be the API process carried out (getting userdetail for validation for logging in in the/login page)
app.MapGet("/login", async () =>
{
    UserDetail userDetailsFetched = await _databaselogic.retrieveUserDataAsDoc();
    return Results.Ok(userDetailsFetched);
});

//retrieve transaction data
app.MapGet("/transactions", async c =>
{
    await _databaselogic.retrieveTransaction();
    c.Response.WriteAsJsonAsync(_databaselogic.tmp_trans.Transactions);
  
});

//save latest user info
app.MapPost("/usersettings", async (UserDetail userDetail) =>
{
    await _databaselogic.saveUserData(userDetail);
    return Results.NoContent();
});

//retrieve user data
app.MapGet("/usersettings", async c =>
{
    UserDetail userDetailsFetched = await _databaselogic.retrieveUserDataAsDoc();
    c.Response.WriteAsJsonAsync(_databaselogic.userDetails);

});

//to save user transactions
app.MapPost("/transactions", async (UserTransaction a_transcation) =>
{
    await _databaselogic.saveTransaction(a_transcation);
    return Results.NoContent();
});

//to delete transaction
app.MapPost("/transactions/{data}", async (string data) =>
{
    await _databaselogic.deleteTransaction(data);
    return Results.NoContent();
});

//save items
app.MapPost("/assets", async (Item a_item) =>
{
    await _databaselogic.saveAsset(a_item);
    return Results.NoContent();
});

//retrieve items
app.MapGet("/assets", async c =>
{
    await _databaselogic.retrieveItem();
    c.Response.WriteAsJsonAsync(_databaselogic.tmp_items.items);
});

//delete items
app.MapPost("/assets/{data}", async (string data) =>
{
    await _databaselogic.deleteAsset(data);
    return Results.NoContent();
});

//delete user data
app.MapPost("/usersettings/{a_user}", async (string a_user) =>
{
    await _databaselogic.deleteUserData(a_user);
    return Results.NoContent();

});


//delete items


app.UseCors("AllowAll");
// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
// NEED THIS

// ... rest of your code ...



app.Run();
