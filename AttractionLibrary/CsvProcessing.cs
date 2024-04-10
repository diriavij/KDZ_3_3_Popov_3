namespace AttractionLibrary;

public class CsvProcessing
{
    private const string RightHeadingEng = "\"Name\";\"Photo\";\"AdmArea\";\"District\";\"Location\";" +
                                           "\"RegistrationNumber\";\"State\";\"LocationType\";\"global_id\";" +
                                           "\"geodata_center\";\"geoarea\";";
    private const string RightHeadingRus = "\"Название объекта\";\"Фотография\";\"Административный округ по адресу\";" +
                                           "\"Район\";\"Месторасположение\";\"Государственный регистрационный знак\";" +
                                           "\"Состояние регистрации\";\"Тип места расположения\";\"global_id\";" +
                                           "\"geodata_center\";\"geoarea\";";
    private const int ColumnsQuantity = 11;
    public Stream? OutputStream { get; private set; }
    
    /// <summary>
    /// Checks format of data in file.
    /// </summary>
    /// <param name="linesInFile"></param>
    /// <returns></returns>
    private static bool CheckFormat(List<string> linesInFile)
    {
        if (linesInFile[0] != RightHeadingEng || linesInFile[1] != RightHeadingRus) // Compares two first lines
                                                                                    // to correct title.
        {
            return false;
        }
        foreach (var line in linesInFile)
        {
            if (line.Length > 0)
            {
                int countVal = 0;
                bool quoteFlag = line[0] == '"'; // Variable indicating whether the quote is currently running or not.
                for (int j = 0; j < line.Length; j++)
                {
                    if (!quoteFlag)
                    {
                        if (line[j] == ';')
                        {
                            countVal++;
                            if (j < line.Length - 1 && line[j + 1] == '"') // If there is a quote after the quotes,
                                                                           // change the value of quoteFlag and
                                                                           // shift it by one element.
                            {
                                j++;
                                quoteFlag = true;
                            }
                            else if (j < line.Length - 2 && line[j + 1] == '=' && line[j + 2] == '"') // If after
                                // the quotation marks a strengthened quotation begins, we shift it by two elements (=").
                            {
                                j += 2;
                                quoteFlag = true;
                            }
                            else
                            {
                                quoteFlag = false;
                            }
                        }
                    }
                    else if (line[j] == '"' && line[j + 1] == ';')
                    {
                        quoteFlag = false;
                    }
                }
                if (countVal != ColumnsQuantity) // If the number of fields in a row does not match the number of columns,
                    // then the record format is broken, we notify the calling code.
                {
                    return false;
                }
            }
        }
        return true;
    }
    
    /// <summary>
    /// Gets data from opened stream.
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    /// <exception cref="FormatException"></exception>
    public List<AttractionTc> Read(StreamReader stream)
    {
        string? line = stream.ReadLine();
        List<string> linesInFile = new List<string>();
        while (line != null)
        {
            linesInFile.Add(line);
            line = stream.ReadLine();
        }
        if (CheckFormat(linesInFile))
        {
            var objectsList = new List<AttractionTc>();
            for (int i = 2; i < linesInFile.Count; i++)
            {
                var values = (from val in linesInFile[i].Split(';') 
                    select val.Trim('"')).ToList();
                objectsList.Add(new AttractionTc(values[0], values[1], values[2],
                    values[3], values[4], values[5], values[6],
                    values[7], values[8], values[9], values[10]));
            }
            return objectsList;
        }
        throw new FormatException();
    }

    /// <summary>
    /// Writes serialized list of attractions into stream.
    /// </summary>
    /// <param name="attractions"></param>
    /// <returns></returns>
    public Stream Write(List<AttractionTc> attractions)
    {
        var attractionsString = (from attraction in attractions select attraction.ToScv()).ToList();
        var linesToFile = 
            new List<string>(new [] {RightHeadingEng, RightHeadingRus}).Concat(attractionsString).ToList();
        using (StreamWriter file = new StreamWriter("../../../../../DownloadedFiles/fileToSend.csv"))
        {
            foreach (var line in linesToFile)
            {
                file.WriteLine(line);
            }
        }
        OutputStream = File.Open("../../../../../DownloadedFiles/fileToSend.csv", FileMode.Open);
        return OutputStream;
    }
}