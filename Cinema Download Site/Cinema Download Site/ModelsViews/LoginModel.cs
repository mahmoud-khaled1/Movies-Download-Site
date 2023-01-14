﻿using System.ComponentModel.DataAnnotations;

namespace Cinema_Download_Site.ModelsViews
{
    public class LoginModel
    {
        [StringLength(256), Required, DataType(DataType.EmailAddress)]
        public string Email { get; set; }
      
        [Required]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
