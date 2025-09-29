using System;
using System.Collections.Generic;

namespace Last_Api.Models;

public class User
{
    public string email { get; set; } = null!;

    public string password { get; set; } = null!;

    public string role { get; set; } = null!;
}
