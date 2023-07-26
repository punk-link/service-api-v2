﻿using Core.Models.Releases;

namespace Core.Models.Artists;

public readonly record struct Artist
{
    public Artist()
    {
    }

    public int Id { get; init; }
    public ImageDetails ImageDetails { get; init; } = default!;
    public int LabelId { get; init; }
    public string Name { get; init; } = default!;
    public List<Release> Releases { get; init; } = default!;
}
