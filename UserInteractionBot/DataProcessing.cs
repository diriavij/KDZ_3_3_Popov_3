using BotClientLibrary;

namespace UserInteractionBot;

public static class DataProcessing
{
    /// <summary>
    /// Sorts data by selected field.
    /// </summary>
    /// <param name="currentClient"></param>
    /// <param name="order"></param>
    public static void Sort(Client currentClient, bool order)
    {
        currentClient.Attractions = order ?
            (from attraction in currentClient.Attractions orderby attraction.AdmArea select attraction).ToList() :
            (from attraction in currentClient.Attractions orderby attraction.AdmArea descending
                select attraction).ToList();
    }

    /// <summary>
    /// Filters data by selected field.
    /// </summary>
    /// <param name="currentClient"></param>
    /// <param name="fieldName"></param>
    /// <param name="value"></param>
    /// <param name="secondValue"></param>
    /// <exception cref="ArgumentException"></exception>
    public static void Filter(Client currentClient, string fieldName, string value, string? secondValue = null)
    {
        currentClient.Attractions = fieldName switch
        {
            "District" => (from attraction in currentClient.Attractions
                    where attraction.District != null && attraction.District.Contains(value)
                    select attraction).ToList(),
            "LocationType" => (from attraction in currentClient.Attractions
                    where attraction.LocationType != null && attraction.LocationType.Contains(value)
                    select attraction).ToList(),
            "AdmArea Location" => (from attraction in currentClient.Attractions
                    where attraction.AdmArea != null && attraction.AdmArea.Contains(value) &&
                          attraction.Location != null && attraction.Location.Contains(secondValue)
                    select attraction).ToList(),
            _ => throw new ArgumentException()
        };
    }
}