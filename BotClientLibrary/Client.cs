using AttractionLibrary;

namespace BotClientLibrary;

public class Client
{
    public long Id { get; set; }
    /// <summary>
    /// Represents the stage that the user is on.
    /// </summary>
    public ClientState State { get; set; }
    /// <summary>
    /// List of attractions received from the user.
    /// </summary>
    public List<AttractionTc>? Attractions { get; set; }
    public string? SelectionField { get; set; }
    /// <summary>
    /// Extra value for selection within two fields.
    /// </summary>
    public string? ExtraValue { get; set; }

    public Client(long id) => (Id, State, Attractions) = (id, ClientState.Introduction, new List<AttractionTc>());
    public Client(){}
}