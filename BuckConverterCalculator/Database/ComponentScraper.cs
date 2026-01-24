using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;
using System.Linq;

// INSTRUCCIONES:
// 1. Registrate en https://developer.digikey.com/
// 2. Crea una app y obtén ClientId y ClientSecret
// 3. Reemplaza los valores CLIENT_ID y CLIENT_SECRET
// 4. Ejecuta: dotnet add package System.Text.Json

namespace BuckConverterCalculator.Database
{
    public class ComponentScraper
    {
        private const string CLIENT_ID = "qNkQG5VYcAKhfExlxodSvzX1wngTanbyvTsRAr8Np96KGVdl"; //"TWdttABpdMqPakW0xbNDeJQClqmAUWmLAsqH4x7bQP4IcuXa";
        private const string CLIENT_SECRET = "yfJzVNtIDpk9neVRh7xmnOT2QAgPoYLwmTA5d6hBkckb99QJmmV8GB65Q2pDZnAN";  // "5hfCrGOdqq05ALLPo5SVlsG6A1z2GjBAGo8bK2tALqrj21CmVitUzsJafQgcm4en";
        private const string TOKEN_URL = "https://api.digikey.com/v1/oauth2/token";
        private const string SEARCH_URL = "https://api.digikey.com/products/v4/search";

        private static HttpClient _httpClient = new HttpClient();
        private static string _accessToken;

        // Categorías para extraer componentes variados
        private static readonly string[] CATEGORIES = new[]
        {
        "Transistors - FETs, MOSFETs - Single",
        "Integrated Circuits (ICs) - Linear - Amplifiers - Instrumentation, OP Amps",
        "Resistors - Fixed - Single",
        "Capacitors - Ceramic - Single",
        "Diodes - Rectifiers - Single",
        "Transistors - Bipolar (BJT) - Single",
        "PMIC - Voltage Regulators - Linear",
        "Microcontrollers - MCU",
        "LEDs - Discrete",
        "Inductors, Coils, Chokes - Fixed"
    };

        static async Task Main(string[] args)
        {
            try
            {
                Console.WriteLine("Obteniendo token de acceso...");
                await GetAccessToken();

                var allComponents = new List<Component>();

                foreach (var category in CATEGORIES)
                {
                    Console.WriteLine($"Buscando en categoría: {category}");
                    var components = await SearchCategory(category, 100); // 100 por categoría
                    allComponents.AddRange(components);
                    await Task.Delay(1000); // Rate limiting
                }

                // Guardar a JSON
                var json = JsonSerializer.Serialize(allComponents, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                File.WriteAllText("components_database.json", json);
                Console.WriteLine($"Exportados {allComponents.Count} componentes a components_database.json");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        static async Task GetAccessToken()
        {
            var content = new FormUrlEncodedContent(new[]
            {
            new KeyValuePair<string, string>("client_id", CLIENT_ID),
            new KeyValuePair<string, string>("client_secret", CLIENT_SECRET),
            new KeyValuePair<string, string>("grant_type", "client_credentials")
        });

            var response = await _httpClient.PostAsync(TOKEN_URL, content);
            var result = await response.Content.ReadAsStringAsync();
            var tokenData = JsonSerializer.Deserialize<TokenResponse>(result);
            _accessToken = tokenData.access_token;
        }

        static async Task<List<Component>> SearchCategory(string category, int limit)
        {
            var components = new List<Component>();

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _accessToken);
            _httpClient.DefaultRequestHeaders.Add("X-DIGIKEY-Client-Id", CLIENT_ID);

            var searchRequest = new
            {
                Keywords = category,
                Limit = limit,
                Offset = 0,
                FilterOptionsRequest = new { }
            };

            var json = JsonSerializer.Serialize(searchRequest);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(SEARCH_URL, content);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Error en búsqueda: {response.StatusCode}");
                return components;
            }

            var result = await response.Content.ReadAsStringAsync();
            var searchResult = JsonSerializer.Deserialize<SearchResponse>(result);

            foreach (var product in searchResult.Products ?? new List<Product>())
            {
                var component = new Component
                {
                    Type = DetermineType(category),
                    Manufacturer = product.Manufacturer?.Name ?? "Unknown",
                    PartNumber = product.ManufacturerPartNumber ?? "N/A",
                    Description = product.ProductDescription ?? "No description",
                    Specifications = ExtractSpecs(product.Parameters),
                    UnitPrice = product.UnitPrice ?? 0,
                    Stock = product.QuantityAvailable ?? 0,
                    Supplier = "DigiKey",
                    DatasheetURL = product.PrimaryDatasheet ?? ""
                };

                components.Add(component);
            }

            return components;
        }

        static string DetermineType(string category)
        {
            if (category.Contains("MOSFET")) return "MOSFET";
            if (category.Contains("OP Amp")) return "OpAmp";
            if (category.Contains("Resistor")) return "Resistor";
            if (category.Contains("Capacitor")) return "Capacitor";
            if (category.Contains("Diode")) return "Diode";
            if (category.Contains("BJT")) return "Transistor";
            if (category.Contains("Regulator")) return "Regulator";
            if (category.Contains("Microcontroller")) return "Microcontroller";
            if (category.Contains("LED")) return "LED";
            if (category.Contains("Inductor")) return "Inductor";
            return "Other";
        }

        static Dictionary<string, string> ExtractSpecs(List<Parameter> parameters)
        {
            var specs = new Dictionary<string, string>();

            if (parameters == null) return specs;

            foreach (var param in parameters.Take(8)) // Max 8 parámetros principales
            {
                if (!string.IsNullOrEmpty(param.ParameterText))
                {
                    specs[param.ParameterText] = param.ValueText ?? "N/A";
                }
            }

            return specs;
        }
    }

    // Clases de modelos
    public class Component
    {
        public string Type { get; set; }
        public string Manufacturer { get; set; }
        public string PartNumber { get; set; }
        public string Description { get; set; }
        public Dictionary<string, string> Specifications { get; set; }
        public decimal UnitPrice { get; set; }
        public int Stock { get; set; }
        public string Supplier { get; set; }
        public string DatasheetURL { get; set; }
    }

    public class TokenResponse
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
    }

    public class SearchResponse
    {
        public List<Product> Products { get; set; }
    }

    public class Product
    {
        public string ManufacturerPartNumber { get; set; }
        public string ProductDescription { get; set; }
        public Manufacturer Manufacturer { get; set; }
        public decimal? UnitPrice { get; set; }
        public int? QuantityAvailable { get; set; }
        public string PrimaryDatasheet { get; set; }
        public List<Parameter> Parameters { get; set; }
    }

    public class Manufacturer
    {
        public string Name { get; set; }
    }

    public class Parameter
    {
        public string ParameterText { get; set; }
        public string ValueText { get; set; }
    }
}
