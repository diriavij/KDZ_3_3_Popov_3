using System.Text.Encodings.Web;
using System.Text.Json;

namespace AttractionLibrary;

public class JsonProcessing
{
    public Stream? OutputStream { get; private set; }
    
    /// <summary>
    /// Gets data from opened stream.
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    public List<AttractionTc> Read(StreamReader stream)
    {
        string? line = stream.ReadLine();
        List<string> linesInFile = new List<string>();
        while (line != null)
        {
            linesInFile.Add(line);
            line = stream.ReadLine();
        }
        List<AttractionTc> result = JsonSerializer.Deserialize<List<AttractionTc>>(String.Join('\n', linesInFile))!;
        return result;
    }

    /// <summary>
    /// Writes serialized list of attractions into stream.
    /// </summary>
    /// <param name="attractions"></param>
    /// <returns></returns>
    public Stream Write(List<AttractionTc> attractions)
    {
        var options = new JsonSerializerOptions { WriteIndented = true, 
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping};
        var lines = JsonSerializer.Serialize(attractions, options);
        using (var file = new StreamWriter("../../../../../DownloadedFiles/fileToSend.csv"))
        {
            file.WriteLine(lines);
        }
        OutputStream = File.Open("../../../../../DownloadedFiles/fileToSend.csv", FileMode.Open);
        return OutputStream;
    }
}