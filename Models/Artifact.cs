using System;
using System.Collections.Generic;

namespace Last_Api.Models;

public class Artifact
{
    public int id { get; set; }
    public string name { get; set; } = null!;

    public string description { get; set; } = null!;

    public double latitude { get; set; }

    public double longitude { get; set; }

    public int? startyear { get; set; }

    public int? endyear { get; set; }

    public string? tribe { get; set; }
}
