using System.Collections.Generic;

#nullable disable

namespace FioRino_NewProject.Entities
{
    public partial class DmUser
    {
        public DmUser()
        {
            DmCodesForResetPasswords = new HashSet<DmCodesForResetPassword>();
            DmFileWzs = new HashSet<DmFileWz>();
            DmOrderArchievumRecievers = new HashSet<DmOrderArchievum>();
            DmOrderArchievumSenders = new HashSet<DmOrderArchievum>();
            DmOrderReceivers = new HashSet<DmOrder>();
            DmOrderSenders = new HashSet<DmOrder>();
            DmUsersAccesses = new HashSet<DmUsersAccess>();
            DmWzMagazynReceivers = new HashSet<DmWzMagazyn>();
            DmWzMagazynSenders = new HashSet<DmWzMagazyn>();
        }

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public int? RoleId { get; set; }
        public int? PositionId { get; set; }
        public string Image { get; set; }

        public virtual DmPosition Position { get; set; }
        public virtual DmRole Role { get; set; }
        public virtual ICollection<DmCodesForResetPassword> DmCodesForResetPasswords { get; set; }
        public virtual ICollection<DmFileWz> DmFileWzs { get; set; }
        public virtual ICollection<DmOrderArchievum> DmOrderArchievumRecievers { get; set; }
        public virtual ICollection<DmOrderArchievum> DmOrderArchievumSenders { get; set; }
        public virtual ICollection<DmOrder> DmOrderReceivers { get; set; }
        public virtual ICollection<DmOrder> DmOrderSenders { get; set; }
        public virtual ICollection<DmUsersAccess> DmUsersAccesses { get; set; }
        public virtual ICollection<DmWzMagazyn> DmWzMagazynReceivers { get; set; }
        public virtual ICollection<DmWzMagazyn> DmWzMagazynSenders { get; set; }
    }
}
