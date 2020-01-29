using Chess.v4.Models;
using System.Text;

public static class GameExtensions
{
    public static string GameToString(this Game game)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"[Event \"{ game.Event }\"]");
        sb.AppendLine($"[Site \"{ game.Site }\"]");
        sb.AppendLine($"[Date \"{ game.Date }\"]");
        sb.AppendLine($"[Round \"{ game.Round }\"]");
        sb.AppendLine($"[White \"{ game.White }\"]");
        sb.AppendLine($"[Black \"{ game.Black }\"]");
        sb.AppendLine($"[Result \"{ game.Result }\"]");
        sb.AppendLine($"[ECO \"{ game.ECO }\"]");
        sb.AppendLine($"[WhiteElo \"{ game.WhiteElo }\"]");
        sb.AppendLine($"[BlackElo \"{ game.BlackElo }\"]");
        sb.AppendLine($"[ID \"{ game.NaturalKey }\"]");
        sb.AppendLine($"[FileName \"{ game.FileName }\"]");
        sb.AppendLine($"[Annotator \"{ game.Annotator }\"]");
        sb.AppendLine($"[Source \"{ game.Source }\"]");
        sb.AppendLine($"[Remark \"{ game.Remark }\"]");
        sb.AppendLine("");
        sb.AppendLine(game.PGN);
        return sb.ToString().Trim('\n').Trim('\r').Trim();
    }
}