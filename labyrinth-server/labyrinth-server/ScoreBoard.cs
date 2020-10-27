using System;
using Microsoft.AspNetCore.Mvc;

namespace labyrinth_server
{
    public class ScoreBoard
    {
        public int Id { get; set; }
        public int Score { get; set; }
        public string UserEmail { get; set; }
        public DateTime Created { get; set; }
    }
}