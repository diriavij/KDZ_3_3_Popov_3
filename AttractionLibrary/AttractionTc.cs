using System.Text.Json.Serialization;

namespace AttractionLibrary;

[Serializable]
public class AttractionTc
{
    [JsonPropertyName("Name")]
    public string? Name { get; set; }
    [JsonPropertyName("Photo")]
    public string? Photo { get; set; }
    [JsonPropertyName("AdmArea")]
    public string? AdmArea { get; set; }
    [JsonPropertyName("District")]
    public string? District { get; set; }
    [JsonPropertyName("Location")]
    public string? Location { get; set; }
    [JsonPropertyName("RegistrationNumber")]
    public string? RegistrationNumber { get; set; }
    [JsonPropertyName("State")]
    public string? State { get; set; }
    [JsonPropertyName("LocationType")]
    public string? LocationType { get; set; }
    [JsonPropertyName("global_id")]
    public string? GlobalId { get; set; }
    [JsonPropertyName("geodata_center")]
    public string? GeodataCenter { get; set; }
    [JsonPropertyName("geoarea")]
    public string? Geoarea { get; set; }

    /// <summary>
    /// Returns the CSV format representation of the object.
    /// </summary>
    /// <returns></returns>
    public string ToScv()
    {
        var fieldsValues = new [] { Name, Photo, AdmArea, District, Location, RegistrationNumber, State,
            LocationType, GlobalId, GeodataCenter, Geoarea};
        fieldsValues = (from value in fieldsValues select "\"" + value + "\"").ToArray();
        return String.Join(';', fieldsValues) + ";";
    }
    
    public AttractionTc(string name, string photo, string admArea, string district, string location,
        string registrationNumber, string state, string locationType, string globalId,
        string geodataCenter, string geoarea)
    {
        Name = name;
        Photo = photo;
        AdmArea = admArea;
        District = district;
        Location = location;
        RegistrationNumber = registrationNumber;
        State = state;
        LocationType = locationType;
        GlobalId = globalId;
        GeodataCenter = geodataCenter;
        Geoarea = geoarea;
    }
    public AttractionTc() { }
}