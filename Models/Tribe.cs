using System;
using System.Collections.Generic;

namespace Last_Api.Models;

public class Tribe
{
    public string name { get; set; } = null!;

    public string description { get; set; } = null!;

    public string region { get; set; } = null!;

    public decimal latitude { get; set; }

    public decimal longitude { get; set; }

    public string language { get; set; } = null!;
}
