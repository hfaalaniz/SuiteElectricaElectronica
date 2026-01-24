using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Linq;
using BuckConverterCalculator.Services;

namespace BuckConverterCalculator.Database
{
    public class ComponentScraperService
    {
        private const string CLIENT_ID = "qNkQG5VYcAKhfExlxodSvzX1wngTanbyvTsRAr8Np96KGVdl";
        private const string CLIENT_SECRET = "yfJzVNtIDpk9neVRh7xmnOT2QAgPoYLwmTA5d6hBkckb99QJmmV8GB65Q2pDZnAN";
        private const string TOKEN_URL = "https://api.digikey.com/v1/oauth2/token";
        private const string SEARCH_URL = "https://api.digikey.com/products/v4/search/keyword";

        private static HttpClient _httpClient = new HttpClient();
        private static string _accessToken;
        private static DateTime _tokenExpiration = DateTime.MinValue;
        private readonly LoggerService _logger = LoggerService.Instance;

        /// <summary>
        /// Busca componentes en DigiKey usando criterios específicos
        /// </summary>
        public async Task<List<ElectronicComponent>> SearchDigiKeyAsync(ComponentSearchCriteria criteria)
        {
            _logger.LogDivider();
            _logger.Info("Starting DigiKey search", "ScraperService");
            _logger.Debug($"Search criteria: Type={criteria.Type}, Manufacturer={criteria.Manufacturer}, PartNumber={criteria.PartNumber}", "ScraperService");

            try
            {
                _logger.Info("Ensuring valid access token...", "ScraperService");
                await EnsureValidToken();

                var keywords = BuildSearchKeywords(criteria);
                _logger.Info($"Search keywords: '{keywords}'", "ScraperService");

                var digikeyResults = await SearchDigiKey(keywords, 50);
                _logger.Info($"DigiKey returned {digikeyResults.Count} results", "ScraperService");

                // Convertir resultados de DigiKey a ElectronicComponent
                var components = digikeyResults.Select(ConvertToElectronicComponent).ToList();
                _logger.Info($"Converted to {components.Count} ElectronicComponent objects", "ScraperService");

                // Log primeros 3 resultados para verificar
                for (int i = 0; i < Math.Min(3, components.Count); i++)
                {
                    var c = components[i];
                    _logger.Debug($"Result {i + 1}: {c.Manufacturer} {c.PartNumber} - ${c.UnitPrice}", "ScraperService");
                }

                return components;
            }
            catch (Exception ex)
            {
                _logger.Error($"Error searching DigiKey", "ScraperService", ex);
                return new List<ElectronicComponent>();
            }
        }

        /// <summary>
        /// Asegura que tenemos un token de acceso válido
        /// </summary>
        private async Task EnsureValidToken()
        {
            if (string.IsNullOrEmpty(_accessToken) || DateTime.Now >= _tokenExpiration)
            {
                _logger.Info("Token expired or not available, requesting new token...", "ScraperService");
                await GetAccessToken();
            }
            else
            {
                _logger.Debug($"Using cached token (expires: {_tokenExpiration:yyyy-MM-dd HH:mm:ss})", "ScraperService");
            }
        }

        /// <summary>
        /// Obtiene un nuevo token de acceso de DigiKey
        /// </summary>
        private async Task GetAccessToken()
        {
            _logger.Info($"Requesting token from: {TOKEN_URL}", "ScraperService");
            _logger.Debug($"Client ID: {CLIENT_ID.Substring(0, 10)}...", "ScraperService");

            try
            {
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("client_id", CLIENT_ID),
                    new KeyValuePair<string, string>("client_secret", CLIENT_SECRET),
                    new KeyValuePair<string, string>("grant_type", "client_credentials")
                });

                var response = await _httpClient.PostAsync(TOKEN_URL, content);
                var responseBody = await response.Content.ReadAsStringAsync();

                _logger.Debug($"Token response status: {response.StatusCode}", "ScraperService");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.Error($"Token request failed. Status: {response.StatusCode}, Body: {responseBody}", "ScraperService");
                    throw new Exception($"Failed to get token: {response.StatusCode}");
                }

                var tokenData = JsonSerializer.Deserialize<TokenResponse>(responseBody);
                _accessToken = tokenData.access_token;
                _tokenExpiration = DateTime.Now.AddSeconds(tokenData.expires_in - 300); // 5 min buffer

                _logger.Info($"Token obtained successfully. Expires at: {_tokenExpiration:yyyy-MM-dd HH:mm:ss}", "ScraperService");
                _logger.Debug($"Token preview: {_accessToken.Substring(0, Math.Min(20, _accessToken.Length))}...", "ScraperService");
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to get access token", "ScraperService", ex);
                throw;
            }
        }

        /// <summary>
        /// Construye palabras clave de búsqueda basadas en criterios
        /// </summary>
        private string BuildSearchKeywords(ComponentSearchCriteria criteria)
        {
            var keywords = new List<string>();

            if (!string.IsNullOrWhiteSpace(criteria.Type) && criteria.Type != "All")
            {
                keywords.Add(criteria.Type);
                _logger.Debug($"Added Type to keywords: {criteria.Type}", "ScraperService");
            }

            if (!string.IsNullOrWhiteSpace(criteria.Manufacturer))
            {
                keywords.Add(criteria.Manufacturer);
                _logger.Debug($"Added Manufacturer to keywords: {criteria.Manufacturer}", "ScraperService");
            }

            if (!string.IsNullOrWhiteSpace(criteria.PartNumber))
            {
                keywords.Add(criteria.PartNumber);
                _logger.Debug($"Added PartNumber to keywords: {criteria.PartNumber}", "ScraperService");
            }

            // Si no hay keywords, buscar MOSFETs por defecto
            string result = keywords.Count > 0 ? string.Join(" ", keywords) : "MOSFET";
            _logger.Info($"Final search keywords: '{result}'", "ScraperService");
            return result;
        }

        /// <summary>
        /// Realiza búsqueda en DigiKey API
        /// </summary>
        private async Task<List<DigikeyComponent>> SearchDigiKey(string keywords, int limit)
        {
            _logger.Info($"Searching DigiKey API: '{keywords}', Limit: {limit}", "ScraperService");

            try
            {
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _accessToken);
                _httpClient.DefaultRequestHeaders.Add("X-DIGIKEY-Client-Id", CLIENT_ID);

                _logger.Debug("Headers configured for DigiKey API request", "ScraperService");

                var searchRequest = new
                {
                    Keywords = keywords,
                    Limit = limit,
                    Offset = 0,
                    FilterOptionsRequest = new { }
                };

                var json = JsonSerializer.Serialize(searchRequest);
                _logger.Debug($"Request JSON: {json}", "ScraperService");

                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                _logger.Info($"Sending POST request to: {SEARCH_URL}", "ScraperService");
                var response = await _httpClient.PostAsync(SEARCH_URL, content);
                var result = await response.Content.ReadAsStringAsync();

                _logger.Info($"Response status: {response.StatusCode}", "ScraperService");
                _logger.Debug($"Response length: {result.Length} characters", "ScraperService");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.Error($"DigiKey API error: {response.StatusCode}", "ScraperService");
                    _logger.Error($"Response body: {result}", "ScraperService");
                    throw new Exception($"DigiKey API error: {response.StatusCode}");
                }

                try
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = false,
                        WriteIndented = true
                    };

                    var searchResult = JsonSerializer.Deserialize<SearchResponse>(result, options);

                    if (searchResult == null)
                    {
                        _logger.Warning("SearchResponse deserialized to null", "ScraperService");
                        return new List<DigikeyComponent>();
                    }

                    if (searchResult.Products == null)
                    {
                        _logger.Warning("SearchResponse.Products is null", "ScraperService");
                        return new List<DigikeyComponent>();
                    }

                    _logger.Info($"Found {searchResult.Products.Count} products in response", "ScraperService");

                    // Log detallado del PRIMER producto
                    if (searchResult.Products.Count > 0)
                    {
                        var firstProduct = searchResult.Products[0];
                        _logger.Debug("=== FIRST PRODUCT RAW DATA ===", "ScraperService");
                        _logger.Debug($"ManufacturerProductNumber: '{firstProduct.ManufacturerProductNumber}'", "ScraperService");
                        _logger.Debug($"Description.ProductDescription: '{firstProduct.Description?.ProductDescriptionText}'", "ScraperService");
                        _logger.Debug($"Description.DetailedDescription: '{firstProduct.Description?.DetailedDescription}'", "ScraperService");
                        _logger.Debug($"Manufacturer.Name: '{firstProduct.Manufacturer?.Name}'", "ScraperService");
                        _logger.Debug($"UnitPrice: {firstProduct.UnitPrice}", "ScraperService");
                        _logger.Debug($"QuantityAvailable: {firstProduct.QuantityAvailable}", "ScraperService");
                        _logger.Debug($"DatasheetUrl: '{firstProduct.DatasheetUrl}'", "ScraperService");
                        _logger.Debug($"Parameters count: {firstProduct.Parameters?.Count ?? 0}", "ScraperService");
                        _logger.Debug("=== END FIRST PRODUCT ===", "ScraperService");
                    }

                    var components = searchResult.Products.Select(p => new DigikeyComponent
                    {
                        Type = DetermineType(p.Description?.ProductDescriptionText ?? p.Description?.DetailedDescription ?? ""),
                        Manufacturer = p.Manufacturer?.Name ?? "Unknown",
                        PartNumber = p.ManufacturerProductNumber ?? "N/A",
                        Description = p.Description?.ProductDescriptionText ?? p.Description?.DetailedDescription ?? "No description",
                        UnitPrice = (double)(p.UnitPrice != null ? p.UnitPrice : 0),
                        Stock = p.QuantityAvailable,
                        DatasheetURL = p.DatasheetUrl ?? "",
                        Parameters = p.Parameters ?? new List<Parameter>()
                    }).ToList();

                    _logger.Info($"Successfully converted {components.Count} DigiKey components", "ScraperService");

                    for (int i = 0; i < Math.Min(3, components.Count); i++)
                    {
                        var c = components[i];
                        _logger.Debug($"Result {i + 1}: {c.Manufacturer} {c.PartNumber} - ${c.UnitPrice:F2}", "ScraperService");
                        _logger.Debug($"  Description: {c.Description}", "ScraperService");
                        _logger.Debug($"  Stock: {c.Stock}", "ScraperService");
                    }

                    return components;
                }
                catch (JsonException jsonEx)
                {
                    _logger.Error("Failed to deserialize DigiKey response", "ScraperService", jsonEx);
                    _logger.Debug($"Response sample: {result.Substring(0, Math.Min(1000, result.Length))}", "ScraperService");
                    throw;
                }
            }
            catch (HttpRequestException httpEx)
            {
                _logger.Error("HTTP request failed", "ScraperService", httpEx);
                throw;
            }
            catch (Exception ex)
            {
                _logger.Error("Unexpected error during DigiKey search", "ScraperService", ex);
                throw;
            }
        }

        /// <summary>
        /// Convierte componente de DigiKey a ElectronicComponent
        /// </summary>
        private ElectronicComponent ConvertToElectronicComponent(DigikeyComponent dk)
        {
            var component = new ElectronicComponent
            {
                Type = dk.Type,
                Manufacturer = dk.Manufacturer,
                PartNumber = dk.PartNumber,
                Description = dk.Description,
                UnitPrice = dk.UnitPrice,
                Stock = dk.Stock,
                Supplier = "DigiKey",
                DatasheetURL = dk.DatasheetURL,
                Specifications = new Dictionary<string, string>()
            };

            // Extraer especificaciones de parámetros
            foreach (var param in dk.Parameters.Take(10))
            {
                if (!string.IsNullOrEmpty(param.ParameterText))
                {
                    component.Specifications[param.ParameterText] = param.ValueText ?? "N/A";
                }
            }

            // Extraer valores numéricos específicos si están disponibles
            ExtractNumericSpecs(component, dk.Parameters);

            return component;
        }

        /// <summary>
        /// Extrae especificaciones numéricas de los parámetros
        /// </summary>
        private void ExtractNumericSpecs(ElectronicComponent component, List<Parameter> parameters)
        {
            foreach (var param in parameters)
            {
                var key = param.ParameterText?.ToLower() ?? "";
                var value = param.ValueText ?? "";

                // Voltage
                if (key.Contains("voltage") && key.Contains("drain"))
                {
                    if (TryParseWithUnit(value, out double v))
                        component.Specifications["VoltageRating"] = v.ToString("F2");
                }

                // Current
                if (key.Contains("current") && (key.Contains("continuous") || key.Contains("drain")))
                {
                    if (TryParseWithUnit(value, out double i))
                        component.Specifications["CurrentRating"] = i.ToString("F2");
                }

                // RDSon
                if (key.Contains("rds") || (key.Contains("resistance") && key.Contains("on")))
                {
                    if (TryParseWithUnit(value, out double r))
                        component.Specifications["RDSon"] = r.ToString("F4");
                }
            }
        }

        /// <summary>
        /// Intenta parsear un valor con unidad (ej: "100V", "2.5A", "10mΩ")
        /// </summary>
        private bool TryParseWithUnit(string value, out double result)
        {
            result = 0;
            if (string.IsNullOrWhiteSpace(value)) return false;

            // Remover espacios y obtener solo la parte numérica
            var numPart = new string(value.TakeWhile(c => char.IsDigit(c) || c == '.' || c == ',').ToArray());

            return double.TryParse(numPart.Replace(',', '.'),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out result);
        }

        /// <summary>
        /// Determina el tipo de componente basado en descripción
        /// </summary>
        private string DetermineType(string description)
        {
            var desc = description?.ToLower() ?? "";

            if (desc.Contains("mosfet")) return "MOSFET";
            if (desc.Contains("op amp") || desc.Contains("operational amplifier") || desc.Contains("opamp")) return "IC";
            if (desc.Contains("resistor")) return "Resistor";
            if (desc.Contains("capacitor")) return "Capacitor";
            if (desc.Contains("diode")) return "Diode";
            if (desc.Contains("inductor")) return "Inductor";
            if (desc.Contains("transistor") || desc.Contains("bjt")) return "MOSFET";
            if (desc.Contains("regulator")) return "IC";
            if (desc.Contains("microcontroller")) return "IC";
            if (desc.Contains("led")) return "Diode";

            return "IC";
        }

        // ==================== CLASES INTERNAS CORREGIDAS ====================

        private class DigikeyComponent
        {
            public string Type { get; set; }
            public string Manufacturer { get; set; }
            public string PartNumber { get; set; }
            public string Description { get; set; }
            public double UnitPrice { get; set; }
            public int Stock { get; set; }
            public string DatasheetURL { get; set; }
            public List<Parameter> Parameters { get; set; }
        }

        private class TokenResponse
        {
            [JsonPropertyName("access_token")]
            public string access_token { get; set; }

            [JsonPropertyName("expires_in")]
            public int expires_in { get; set; }
        }

        private class SearchResponse
        {
            [JsonPropertyName("Products")]
            public List<Product> Products { get; set; }

            [JsonPropertyName("ProductsCount")]
            public int ProductsCount { get; set; }
        }

        private class Product
        {
            [JsonPropertyName("ManufacturerProductNumber")]
            public string ManufacturerProductNumber { get; set; }

            [JsonPropertyName("Description")]
            public ProductDescription Description { get; set; }

            [JsonPropertyName("Manufacturer")]
            public Manufacturer Manufacturer { get; set; }

            [JsonPropertyName("UnitPrice")]
            public decimal UnitPrice { get; set; }

            [JsonPropertyName("QuantityAvailable")]
            public int QuantityAvailable { get; set; }

            [JsonPropertyName("DatasheetUrl")]
            public string DatasheetUrl { get; set; }

            [JsonPropertyName("Parameters")]
            public List<Parameter> Parameters { get; set; }
        }

        private class ProductDescription
        {
            [JsonPropertyName("ProductDescription")]
            public string ProductDescriptionText { get; set; } = string.Empty;

            [JsonPropertyName("DetailedDescription")]
            public string DetailedDescription { get; set; } = string.Empty;
        }

        private class Manufacturer
        {
            [JsonPropertyName("Id")]
            public int Id { get; set; }

            [JsonPropertyName("Name")]
            public string Name { get; set; }
        }

        private class Parameter
        {
            [JsonPropertyName("ParameterId")]
            public int ParameterId { get; set; }

            [JsonPropertyName("ParameterText")]
            public string ParameterText { get; set; }

            [JsonPropertyName("ValueText")]
            public string ValueText { get; set; }

            [JsonPropertyName("ValueId")]
            public string ValueId { get; set; }
        }
    }
}