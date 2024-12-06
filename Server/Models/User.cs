﻿using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace Server.Models
{
    public class User : IdentityUser
    {
        public string Name { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;


        // navigation properties:
        public Vendor Vendor { get; set; }
        public Customer Customer { get; set; }
        public virtual RefreshToken RefreshToken { get; set; }
    }
}