﻿using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace Server.Models
{
    public class User : IdentityUser
    {
        
        public string Name { get; set; } = string.Empty;

        public string ProfilePicturePath { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } 

        public bool HasShop { get; set; }

        // navigation properties:
        public Vendor Vendor { get; set; }
        public Customer Customer { get; set; }
        public virtual AccessToken AccessToken { get; set; }
    }
}