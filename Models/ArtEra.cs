using System;
using System.Collections.Generic;

namespace Last_Api.Models;

public class ArtEra
{
    public string name { get; set; } = null!;

    public string description { get; set; } = null!;

    public int? startyear { get; set; }

    public int? endyear { get; set; }
}
