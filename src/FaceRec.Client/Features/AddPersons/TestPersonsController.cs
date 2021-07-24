using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace FaceRec.Client.Features.AddPersons
{
    [Route("persons")]
    [ApiController]
    public class TestPersonsController : ControllerBase
    {
        private readonly IHttpClientFactory _clientFactory;

        public TestPersonsController(IHttpClientFactory httpClientFactory)
        {
            _clientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> AddPersons([FromQuery] int numberOfPersons = 10, bool searchPersons = false)
        {
            try
            {
                if (numberOfPersons <= 0)
                {
                    numberOfPersons = 10;
                }
                var handler = new TestPersonsHandler();

                var createPersonClient = _clientFactory.CreateClient("PersonsAdderClient");
                var tasks = Enumerable.Range(0, numberOfPersons).Select(i => handler.CreateAndAddPerson(createPersonClient));
                var result = await Task.WhenAll(tasks);

                var resultsValid = result.All(res => res.Ex == null);

                if (!searchPersons)
                {
                    return resultsValid ? 
                        Ok(result.Select(res => new { res.Name, res.Id })) 
                        : StatusCode(StatusCodes.Status500InternalServerError);
                }

                var findPersonClient = _clientFactory.CreateClient("PersonsFinderClient");
                var findTasks = result.Select(res => handler.FindPerson(findPersonClient, res.Name, res.Id, res.Features));
                var findResult = await Task.WhenAll(findTasks);

                return Ok( findResult.Select(fres => new { fres.Name, fres.Id, fres.Matches }));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }
    }

    public class TestPersonsHandler
    {
        private readonly Uri _faceRecUri;
        private readonly Random _random = new Random();
        private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public TestPersonsHandler()
        {
            _faceRecUri = new UriBuilder("http", "localhost", 5000, "person").Uri;
        }

        public double GenerateRandomNumber(double minimum, double maximum)
        {
            return _random.NextDouble() * (maximum - minimum) + minimum;
        }

        public async Task<(string Name, int Id, double[] Features, Exception Ex)> CreateAndAddPerson(HttpClient client)
        {
            try
            {
                var name = ListOfPersonNames.GetName();
                var features = new double[256];
                for (int i = 0; i < features.Length; i++)
                {
                    features[i] = GenerateRandomNumber(-1, 1);
                }
                var response = await client.PostAsync(_faceRecUri, JsonContent.Create(new { Name = name, Features = features }));
                response.EnsureSuccessStatusCode();
                using var responseStream = await response.Content.ReadAsStreamAsync();
                var createPersonResponse = await JsonSerializer.DeserializeAsync<CreatePersonResponse>(responseStream, _jsonSerializerOptions);

                return (name, createPersonResponse.Id, features, null);
            }
            catch (HttpRequestException hrex)
            {
                return ("", 0, Array.Empty<double>(), hrex);
            }
            catch (Exception ex)
            {
                return ("", 0, Array.Empty<double>(), ex);
            }
        }

        public async Task<(string Name, int Id, string[] Matches)> FindPerson(HttpClient client, string name, int id, double[] features)
        {
            try
            {
                var httpRequest = new HttpRequestMessage()
                {
                    Method = HttpMethod.Get,
                    RequestUri = _faceRecUri,
                    Content = JsonContent.Create(new { Features = features })
                };
                var response = await client.SendAsync(httpRequest);
                response.EnsureSuccessStatusCode();
                using var responseStream = await response.Content.ReadAsStreamAsync();
                var matches = await JsonSerializer.DeserializeAsync<string[]>(responseStream, _jsonSerializerOptions);

                return (name, id, matches);
            } 
            catch (Exception)
            {
                return (name, id, Array.Empty<string>());
            }
        }
    }

    public class CreatePersonResponse
    {
        public int Id { get; set; }
        public string Error { get; set; }
    }

}
