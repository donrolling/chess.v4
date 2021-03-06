﻿using Chess.v4.Engine.Interfaces;
using Chess.v4.Models;
using Common.Responses;
using Microsoft.AspNetCore.Mvc;
using Omu.ValueInjecter;
using Website.Factories;
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
        public OperationResult<GameStateResource> GetStateInfo([FromQuery] string fen)
        {
            var result = _gameStateService.Initialize(fen);
            if (result.Success)
            {
                var gameStateResult = GameStateResourceFactory.ToGameStateResource(result.Result);
                return OperationResult<GameStateResource>.Ok(gameStateResult.Result);
            }
            else
            {
                return OperationResult<GameStateResource>.Fail(result.Message);
            }
        }

        [HttpPost]
        [Route("move")]
        public OperationResult<GameStateResource> MakeMove([FromBody] MoveRequestResource moveRequest)
        {
            var gameStateResult = GameStateResourceFactory.ToGameState(moveRequest.GameState);
            var result = _gameStateService.MakeMove(gameStateResult.Result, moveRequest.Beginning, moveRequest.Destination, moveRequest.PiecePromotionType);
            if (result.Success)
            {
                var gameStateResourceResult = GameStateResourceFactory.ToGameStateResource(result.Result);
                return OperationResult<GameStateResource>.Ok(gameStateResourceResult.Result);
            }
            else
            {
                return OperationResult<GameStateResource>.Fail(result.Message);
            }
        }

        [HttpPost]
        [Route("goto")]
        public OperationResult<GameStateResource> GoTo([FromBody] GoToMoveResource goToMoveResource)
        {
            var gameStateResult = GameStateResourceFactory.ToGameState(goToMoveResource.GameState);
            var result = GameStateResourceFactory.MoveToHistoryIndex(_gameStateService, gameStateResult.Result, goToMoveResource.HistoryIndex);
            if (result.Success)
            {
                return OperationResult<GameStateResource>.Ok(result.Result);
            }
            else
            {
                return OperationResult<GameStateResource>.Fail(result.Message);
            }
        }
    }
}