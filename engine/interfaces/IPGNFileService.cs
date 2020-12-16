using Chess.v4.Models;
using System.Text;

namespace Chess.v4.Engine.Interfaces
{
    public interface IPGNFileService
    {
        string CurrentDirectory { get; }

        string ConvertToPGN(GameMetaData metaData);

        void DisplayPGNFileHeader(GameMetaData metaData);

        string GetBaseEnvironmentPath(string directory);

        StringBuilder GetGameHeader(GameMetaData gameMetaData);

        string GetPGNFilePath(string filename);

        GameMetaData ParsePGNData(string input);

        GameMetaData ReadPGNFromFile(string fullpath);

        void SaveFile(string filename, string data);
    }
}