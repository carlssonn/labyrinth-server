using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.WebEncoders.Testing;

namespace labyrinth_server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LabyrinthController : ControllerBase
    {
        private LabyrinthContext _context { get; set; }

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<LabyrinthController> _logger;

        public LabyrinthController(ILogger<LabyrinthController> logger, LabyrinthContext context)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            var scoreBoard = new ScoreBoard {Created = DateTime.Now, Score = 11, UserEmail = "test@gmail.com"};

            try
            {
                await _context.ScoreBoard.AddAsync(scoreBoard);
                await _context.SaveChangesAsync();

                return Accepted("Ok");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
