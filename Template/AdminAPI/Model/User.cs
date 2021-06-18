namespace Upscript.Services.Admin.API.Model
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public int Employee_Id { get; set; }
        public bool IsActive { get; set; }
    }
}
