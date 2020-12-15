using Chess.v4.Engine.Interfaces;
using Chess.v4.Models;
using Common.Responses;
using Microsoft.AspNetCore.Mvc;
using Omu.ValueInjecter;
using Website.Models;

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

        [HttpGet]
        [Route("state-info")]
        public OperationResult<GameStateDTO> GetStateInfo([FromQuery] string fen)
        {
            var result = _gameStateService.Initialize(fen);
            if (result.Success)
            {
                var gameStateDTO = ToGameStateDTO(result.Result);
                return OperationResult<GameStateDTO>.Ok(gameStateDTO);
            }
            else
            {
                return OperationResult<GameStateDTO>.Fail(result.Message);
            }
        }

        [HttpPost]
        [Route("move")]
        public OperationResult<GameStateDTO> MakeMove([FromBody] MoveRequest moveRequest)
        {
            var gameState = ToGameState(moveRequest.GameState);
            var result = _gameStateService.MakeMove(gameState, moveRequest.Beginning, moveRequest.Destination, moveRequest.PiecePromotionType);
            if (result.Success)
            {
                var gameStateDTO = ToGameStateDTO(result.Result);
                return OperationResult<GameStateDTO>.Ok(gameStateDTO);
            }
            else
            {
                return OperationResult<GameStateDTO>.Fail(result.Message);
            }
        }

        private static GameStateDTO ToGameStateDTO(GameState gameState)
        {
            var gameStateDTO = new GameStateDTO();
            gameStateDTO.InjectFrom(gameState);
            gameStateDTO.FEN = gameState.ToString();
            return gameStateDTO;
        }

        private static GameState ToGameState(GameStateDTO gameStateDTO)
        {
            var gameState = new GameState();
            gameState.InjectFrom(gameStateDTO);
            return gameState;
        }
    }
}