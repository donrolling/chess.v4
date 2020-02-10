using Chess.v4.Engine.Interfaces;
using Chess.v4.Models;
using Microsoft.AspNetCore.Mvc;

namespace Website.Controllers
{
    public class GameController : Controller
    {
        private readonly IGameStateService _gameStateService;

        public GameController(IGameStateService gameStateService)
        {
            _gameStateService = gameStateService;
        }

        [HttpGet]
        public GameState Get(string fen)
        {
            var x = _gameStateService.Initialize(fen);
            if (x.Failure)
            {
                throw new System.Exception(x.Message);
            }
            return x.Result;
        }
    }
}