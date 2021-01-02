using chess_engine.Models;
using System.Text;

namespace chess_engine.Engine.Interfaces
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