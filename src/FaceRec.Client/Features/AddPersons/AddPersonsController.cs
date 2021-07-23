using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FaceRec.Client.Features.AddPersons
{
    [Route("persons")]
    [ApiController]
    public class AddPersonsController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        

        public AddPersonsController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("PersonsAdderClient");
            
        }

        [HttpGet]
        public async Task<IActionResult> AddPersons([FromQuery] int numberOfPersons = 10)
        {
            try
            {
                if (numberOfPersons <= 0)
                {
                    numberOfPersons = 10;
                }
                var handler = new AddPersonsHandler();

                var tasks = Enumerable.Range(0, numberOfPersons).Select(i => handler.CreateAndAddPerson(_httpClient));
                var result = await Task.WhenAll(tasks);

                return result.All(res => res) ? Ok() : StatusCode(StatusCodes.Status500InternalServerError);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }
    }

    public class AddPersonsHandler
    {
        private readonly Uri _faceRecUri;
        private readonly Random _random = new Random();

        public AddPersonsHandler()
        {
            _faceRecUri = new UriBuilder("http", "localhost", 5000, "person").Uri;
        }

        public double GenerateRandomNumber(double minimum, double maximum)
        {
            return _random.NextDouble() * (maximum - minimum) + minimum;
        }

        public async Task<bool> CreateAndAddPerson(HttpClient client)
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

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
