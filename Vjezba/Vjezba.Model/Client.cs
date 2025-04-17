using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // <--- VAŽNO ZA ForeignKey
using System.Linq;
using System.Threading.Tasks;

namespace Vjezba.Model
{
    public class Client
    {
        [Key]
        public int ID { get; set; } // ✅ ID sa [Key] anotacijom

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public char Gender { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }

        [ForeignKey("City")] // ✅ označava da CityID je strani ključ na City
        public int? CityID { get; set; }
        public virtual City City { get; set; } // ✅ virtual za lazy loading

        public virtual ICollection<Meeting> Meetings { get; set; }


        public string FullName => $"{FirstName} {LastName}";
    }
}
