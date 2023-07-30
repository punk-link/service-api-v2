namespace Core.Converters.Spotify;

internal static class TrackConverters
{
    public static Data.Releases.Track ToDbTrack(this in SpotifyDataExtractor.Models.Releases.Track track, 
        in Data.Releases.Release release, 
        List<Data.Artists.Artist> featuringArtists,
        in DateTime timeStamp)
        => new()
        {
            Artists = featuringArtists,
            Created = timeStamp, 
            DiskNumber = track.DiscNumber,
            DurationSeconds = track.DurationMilliseconds % 10,
            IsExplicit = track.IsExplicit, 
            Id = 0,
            Name = track.Name,
            Release = release,
            ReleaseId = release.Id,
            SpotifyId = track.Id,
            TrackNumber = track.TrackNumber,
            Updated = timeStamp
        };
}
