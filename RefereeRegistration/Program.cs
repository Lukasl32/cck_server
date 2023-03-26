using System.Text.Json;

using Accessories.Models;

//globální proměnné
HttpClient client = new HttpClient();
string apiAddress = "https://localhost:7041";
//

//Console.Write("Adresa API: ");
//apiAddress = Console.ReadLine();

string email = "lukasl32@atlas.cz";
string password = "123456789";
client.DefaultRequestHeaders.Add("email", email);
client.DefaultRequestHeaders.Add("password", password);

using HttpResponseMessage response = await client.GetAsync($"{apiAddress}/api/login");
response.EnsureSuccessStatusCode();
string token = await response.Content.ReadAsStringAsync();
client.DefaultRequestHeaders.Add("token", token);

Console.WriteLine("Registrace nového rozhodčího");

//using HttpResponseMessage response = await client.GetAsync($"{apiAddress}/api/competitions");
using HttpResponseMessage response1 = await client.GetAsync($"{apiAddress}/api/competitions");
response.EnsureSuccessStatusCode();
List<Competition> responseBody = JsonSerializer.Deserialize<List<Competition>>(await response.Content.ReadAsStringAsync());

Console.WriteLine();
Console.ReadLine();
