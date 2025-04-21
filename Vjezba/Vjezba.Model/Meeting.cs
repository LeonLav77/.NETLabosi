using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Vjezba.Model.Enums;

namespace Vjezba.Model
{
    public class Meeting
    {
        [Key]
        public int Id { get; set; }

        public MeetingType Type { get; set; }

        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        public MeetingStatus Status { get; set; }

        public string? Location { get; set; }
        public string? Comments { get; set; }

        public int ClientID { get; set; }

        [ForeignKey("ClientID")]
        public virtual Client Client { get; set; }
    }
}
