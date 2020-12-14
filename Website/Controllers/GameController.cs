using Chess.v4.Engine.Interfaces;
using Chess.v4.Models;
using Common.Responses;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Website
{
    [Route("api/[controller]")]
    public class GameController : Controller
    {
        private readonly IGameStateService _gameStateService;

        public GameController(IGameStateService gameStateService)
        {
            _gameStateService = gameStateService;
        }

        [HttpPost]
        public async Task<OperationResult<GameState>> Post()
        {
            using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                var fen = await reader.ReadToEndAsync();
                return _gameStateService.Initialize(fen);
            }
        }
   }
}