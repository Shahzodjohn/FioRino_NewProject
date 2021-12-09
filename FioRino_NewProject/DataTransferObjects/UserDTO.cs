using FioRino_NewProject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FioRino_NewProject.DataTransferObjects
{
    public partial class UserDTO
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string PositionName { get; set; }
        public string RoleName { get; set; }
        public string ImagePath { get; set; }
        public bool? HurtAccess { get; set; }
        public bool? MagazynAccess { get; set; }
        public bool? ArchiveAccess { get; set; }
    }
}
