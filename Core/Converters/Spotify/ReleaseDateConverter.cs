using SpotifyDataExtractor.Models.Enums;

namespace Core.Converters.Spotify;

internal static class ReleaseDateConverter
{
    public static DateOnly ToDate(string date)
    {
        var dateSegments = date.Split('-');
        
        var day = 0;
        var month = 0;
        var year = int.Parse(dateSegments[0]);
        
        if (2 <= dateSegments.Length)
            month = int.Parse(dateSegments[1]);
        
        if (3 == dateSegments.Length)
            day = int.Parse(dateSegments[2]);
                
        return new DateOnly(year, month, day);
    }
}