namespace FioRino_NewProject.DataTransferObjects
{
    public class UpdateUserDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        //public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public int PositionId { get; set; }
        public int RoleId { get; set; }
    }
}
