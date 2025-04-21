using System;
using Vjezba.Model;
using Vjezba.Model.DTO;

namespace Vjezba.Model.DTO
{
    public class ClientDTO
    {
        public int ID { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }
        public CityDTO City { get; set; }
        public string Email { get; set; }
    }
}