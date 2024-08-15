using System.IO;

namespace SenhaixFreqWriter.DataModels.Interfaces;

public interface IBackupable
{
    void SaveToFile(Stream stream);
}