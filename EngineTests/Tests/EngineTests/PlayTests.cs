using Chess.v4.Engine.Interfaces;
using Chess.v4.Engine.Reference;
using Chess.v4.Models;
using Common.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Omu.ValueInjecter;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tests.Models;

namespace Tests
{
    [TestClass]
    public class PlayTests : TestBase
    {
        private readonly IGameStateService _gameStateService;
        private readonly IPGNFileService _pgnFileService;
        private readonly IPGNService _pgnService;
        private readonly Regex _endgamePattern = new Regex(@"\d\-\d");

        public PlayTests()
        {
            this._gameStateService = this.ServiceProvider.GetService<IGameStateService>();
            this._pgnService = this.ServiceProvider.GetService<IPGNService>();
            this._pgnFileService = this.ServiceProvider.GetService<IPGNFileService>();
        }

        [TestMethod]
        public void PlayAllGamesFromTheDatabase_ArriveAtSameResult()
        {
            var games = import();
            foreach (var game in games)
            {
                playGame_ArriveAtSameresult(game);
            }
        }

        private void playGame_ArriveAtSameresult(Game game)
        {
            var playGameInfo = playGamePrep(game);
            var count = playGameInfo.GameData.Moves.Count();
            var moveCount = 0;
            foreach (var move in playGameInfo.GameData.Moves)
            {
                moveCount++;
                if (moveCount == count)
                {
                    playGameInfo.FinalMove = move.Value;
                }
                try
                {
                    playGameInfo.GameState = playMove(playGameInfo.GameState, game, move.Value, moveCount);
                }
                catch (System.Exception ex)
                {
                    Assert.IsTrue(false, $"{ ex.Message }\r\nGame engine failed to play a PGN move. Move: { move.Key }. { move.Value }\r\n{ game.FEN }\r\n{ playGameInfo.GameString }");
                }
                if (moveCount != count)
                {
                    //check to see if we're in checkmate as long as this isn't the last move.
                    Assert.IsFalse(playGameInfo.GameState.StateInfo.IsCheckmate, $"The engine thinks this is checkmate, though it is not. Move: { move.Key }. { move.Value }\r\n{ game.FEN }\r\n{ playGameInfo.GameString }");
                }
            }
            //I wanted to make more assertions around whether or not the game was a draw,
            //but the engine doesn't currently recognize a draw because it
            //is an agreement between players, not a game state.
            if (playGameInfo.IsDraw)
            {
                //right now this is failing sometimes because for example, on game #16, there is a pawn move that
                //will capture the queen that is checking the king, but the IsCheckmate calculation doesn't
                //understand
                Assert.IsFalse(playGameInfo.GameState.StateInfo.IsCheckmate, $"Game should not be marked as checkmate. This game has ended in a draw. Final move was { playGameInfo.FinalMove }.\r\n{ game.FEN }\r\n{ playGameInfo.GameString }");
            }
            if (playGameInfo.HasCheckmate)
            {
                Assert.IsTrue(playGameInfo.GameState.StateInfo.IsCheckmate, $"Game should be marked as checkmate. Final move was { moveCount }. { playGameInfo.FinalMove }\r\n{ game.FEN }\r\n{ playGameInfo.GameString }");
                Assert.AreEqual(game.Result, playGameInfo.GameState.StateInfo.Result, $"Game Result should be the same.\r\n{ game.FEN }\r\n{ playGameInfo.GameString }");
            }
            game.FEN = playGameInfo.GameState.ToString();
            
        }

        private PlayGameInfo playGamePrep(Game game)
        {
            var playGameInfo = new PlayGameInfo();
            playGameInfo.GameString = game.GameToString();
            FileUtility.WriteFile<PlayTests>("playGame.pgn", "Output", playGameInfo.GameString);
            var result = playGameInfo.GameString.Split(" ").Last();
            var gameStateResult = this._gameStateService.Initialize();
            playGameInfo.GameState = gameStateResult.Result;
            game.FEN = GeneralReference.Starting_FEN_Position;
            playGameInfo.HasCheckmate = playGameInfo.GameString.Split("\r\n\r\n")[1].Contains('#');
            playGameInfo.IsDraw = game.Result == "1/2-1/2";
            playGameInfo.FinalMove = string.Empty;
            playGameInfo.GameData = this._pgnFileService.ParsePGNData(playGameInfo.GameString);
            return playGameInfo;
        }

        private GameState playMove(GameState gameState, Game game, string move, int moveCount)
        {
            var test = "";
            var moveBreak = 15;
            var xs = move.Split(' ');
            var a = xs[0];
            if (_endgamePattern.Matches(a).Any())
            {
                return (gameState);
            }
            if (moveCount >= moveBreak)
            {
                test = "";
                //fool the compiler into not giving me warnings about "test"
                if (string.IsNullOrEmpty(test)) { }
            }
            var gameStateResult = this._gameStateService.MakeMove(gameState, a);
            Assert.IsTrue(gameStateResult.Success, $"Move should have been successful. { a } | { game.FEN }");
            //record and save the FEN at every step so I can figure out where things went wrong.
            game.FEN = gameStateResult.Result.ToString();
            if (xs.Length == 1)
            {
                return (gameStateResult.Result);
            }
            var b = xs[1];
            if (string.IsNullOrEmpty(b))
            {
                return (gameStateResult.Result);
            }
            if (_endgamePattern.Matches(b).Any())
            {
                return (gameStateResult.Result);
            }
            if (moveCount >= moveBreak)
            {
                test = "";
            }
            gameStateResult = this._gameStateService.MakeMove(gameStateResult.Result, b);
            Assert.IsTrue(gameStateResult.Success, $"Move should have been successful. { b } | { game.FEN } \r\n{ gameStateResult.Message }");
            //record and save the FEN at every step so I can figure out where things went wrong.
            game.FEN = gameStateResult.Result.ToString();
            return gameStateResult.Result;
        }

        private List<Game> import()
        {
            var games = new List<Game>();
            var data = FileUtility.ReadTextFile<PlayTests>("December1.pgn", "Data\\Games");
            var groups = data.Split("\r\n\r\n");
            for (int i = 0; i < groups.Length; i = i + 2)
            {
                var metadata = groups[i];
                var regex = new Regex("[ ]{2,}", RegexOptions.None);
                var moves = regex.Replace(groups[i + 1].Replace('\r', ' ').Replace('\n', ' '), " ");
                var regexResult = moves.Split(" ").Last();
                var gameData = this._pgnFileService.ParsePGNData($"{ metadata }\r\n\r\n{ moves }");
                var game = new Game();
                game.InjectFrom(gameData);
                var gameStateResult = this._gameStateService.Initialize();
                var gameState = gameStateResult.Result;
                game.FEN = gameState.ToString();
                game.PGN = moves;
                game.Result = regexResult;
                games.Add(game);
            }
            return games;
        }
    }
}