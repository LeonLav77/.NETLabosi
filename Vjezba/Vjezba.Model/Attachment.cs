using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Vjezba.Model
{
    public class Attachment
    {
        [Key]
        public int ID { get; set; }
        
        [Required]
        [StringLength(255)]
        public string FileName { get; set; }
        
        [Required]
        [StringLength(500)]
        public string FilePath { get; set; }
        
        [Required]
        public DateTime UploadDate { get; set; }
        
        // Foreign key for Client
        [Required]
        [ForeignKey("Client")]
        public int ClientID { get; set; }
        
        [JsonIgnore]
        public virtual Client Client { get; set; }
    }
}