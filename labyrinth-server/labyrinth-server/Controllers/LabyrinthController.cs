using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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
        public IEnumerable<LeaderBoardViewModel> Get()
        {
            var groups = _context.ScoreBoard
                .ToList()
                .GroupBy(x =>
                {
                    var shortDate = x.Created.ToShortDateString();
                    return new { shortDateString = shortDate, x.UserEmail };
                });

            var leaderBoardViewModels = groups.Select(userGroups => userGroups
                .OrderByDescending(x => x.Score)
                .FirstOrDefault())
                .Select(scoreBoard => new LeaderBoardViewModel
                {
                    Created = scoreBoard?.Created.ToString("yyyy-MM-dd"),
                    Email = scoreBoard?.UserEmail,
                    Score = scoreBoard.Score.ToString()
                }).ToList();

            return leaderBoardViewModels;
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            var scoreBoard = new ScoreBoard { Created = DateTime.Now, Score = 99, UserEmail = "test@gmail.com" };

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

    public class LeaderBoardViewModel
    {
        public string Score { get; set; }
        public string Email { get; set; }
        public string Created { get; set; }
    }
}
