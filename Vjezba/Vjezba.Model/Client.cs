using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Vjezba.Model
{
    public class Client
    {
        [Key]
        public int ID { get; set; }

        [Required(ErrorMessage = "First name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 50 characters")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 50 characters")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Email must be between 5 and 100 characters")]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "Please select a gender")]
        public char Gender { get; set; }
        
        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; }
        
        [Required(ErrorMessage = "Phone number is required")]
        [Display(Name = "Phone Number")]
        [Phone(ErrorMessage = "Please enter a valid phone number")]
        public string PhoneNumber { get; set; }
        
        [Required(ErrorMessage = "Please select a city")]
        [ForeignKey("City")]
        [Display(Name = "City")]
        public int? CityID { get; set; }
        
        [JsonIgnore]
        [ValidateNever] 
        public virtual City City { get; set; }
        
        [Required(ErrorMessage = "Please Enter number")]
        [Range(0, 100, ErrorMessage = "Working experience must be between 0 and 100 years")]
        [Display(Name = "Working Experience (years)")]
        public int? WorkingExperience { get; set; }
        
        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }
        
        [JsonIgnore]
        [InverseProperty("Client")]
        public virtual ICollection<Meeting> Meetings { get; set; } = new List<Meeting>();
        
        [JsonIgnore]
        [InverseProperty("Client")]
        public virtual ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
        
        [Display(Name = "Created By")]
        public string? CreatedById { get; set; }

        [Display(Name = "Updated By")]
        public string? UpdatedById { get; set; }

        // Concatenated FullName property
        public string FullName => $"{FirstName} {LastName}";
    }
}
