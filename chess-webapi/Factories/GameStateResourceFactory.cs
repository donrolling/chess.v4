using chess_engine.Engine.Interfaces;
using chess_engine.Models;
using Common.Responses;
using Omu.ValueInjecter;
using System.Linq;
using chess_webapi.Models;
using Common.Factories;

namespace chess_webapi.Factories
{
    public static class GameStateResourceFactory
    {
        public static OperationResult<GameState> ToGameState(GameStateResource gameStateResource)
        {
            var gameState = new GameState();
            gameState.InjectFrom(gameStateResource);
            return OperationResultFactory.Ok(gameState);
        }

        public static OperationResult<GameStateResource> ToGameStateResource(GameState gameState)
        {
            var gameStateResource = new GameStateResource();
            gameStateResource.InjectFrom(gameState);
            gameStateResource.FEN = gameState.ToString();
            return OperationResultFactory.Ok(gameStateResource);
        }

        public static OperationResult<GameStateResource> MoveToHistoryIndex(IGameStateService gameStateService, GameState gameState, int historyIndex)
        {
            if (historyIndex == 0)
            {
                return ToGameStateResource(gameStateService.Initialize().Result);
            }
            if (gameState.History.Count < historyIndex)
			{
				return OperationResultFactory.Fail<GameStateResource>("Index was lower than history count.");
            }
            var initialFen = gameState.History.FirstOrDefault();
            if (initialFen == null)
            {
                return OperationResultFactory.Fail<GameStateResource>("Couldn't find initial position.");
            }
            var newGameState =  playFromHistoryToAPoint(gameStateService, gameState, historyIndex, initialFen);
            return ToGameStateResource(newGameState.Result);
        }

        private static OperationResult<GameState> playFromHistoryToAPoint(IGameStateService gameStateService, GameState gameState, int historyIndex, Snapshot initialFen)
        {
            var nextGameState = gameStateService.Initialize(initialFen.ToString());
            for (int i = 0; i < historyIndex; i++)
            {
                var pgnMove = gameState.PGNMoves[i];
                nextGameState = gameStateService.MakeMove(nextGameState.Result, pgnMove);
                if (nextGameState.Failure)
                {
                    return OperationResultFactory.Fail<GameState>($"Couldn't make the moves that were recorded. { nextGameState.Message }");
                }
            }
            return OperationResultFactory.Ok(nextGameState.Result);
        }
    }
}