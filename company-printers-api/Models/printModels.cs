using System.ComponentModel.DataAnnotations;

namespace company_printers_api.Models
{
    public class printModels
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
    public class PrinterModel
    {
        private const string PathRegex = @"^(C:\\FTPArea\\formidable|\\\\ctdzlp01\\formidable)\\P00\\[A-Za-z0-9]+\\(in|out)$";
        [Key]
        public int PrinterId { get; set; }

        [Required(ErrorMessage = "Printer Name is required.")]
        [StringLength(100, ErrorMessage = "Printer Name cannot exceed 100 characters.")]
        public string PrinterName { get; set; }

        [Required(ErrorMessage = "Make ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Make ID must be a positive integer.")]
        public int MakeID { get; set; }

        [Required(ErrorMessage = "Folder to Monitor is required.")]
        [RegularExpression(PathRegex, ErrorMessage = "Invalid Folder Path format.")]
        public string FolderToMonitor { get; set; }

        [Required(ErrorMessage = "Output Type is required.")]
        [RegularExpression("PDF|TXT|DOCX|FTP", ErrorMessage = "Output Type must be PDF, TXT, DOCX, or FTP.")]
        public string OutputType { get; set; }

        [Required(ErrorMessage = "File Output is required.")]
        [RegularExpression(PathRegex, ErrorMessage = "Invalid File Output format.")]
        public string FileOutput { get; set; }

        public bool Active { get; set; } = true;

        [Required]
        public DateTime CreatedTimeStamp { get; set; } = DateTime.Now;
    }

    public class UserModel
    {
        public int? UserId { get; set; }
        public int? DesignationId { get; set; }

        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Designation is required")]
        public string DesignationName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", 
        ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Username is required")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        [RegularExpression(@"^(?=.*[!@#$%^&*(),.??"":{}|<>]).*$",
        ErrorMessage = "Password must contain at least one special character.")]
        public string Password { get; set; }
    }
    public class DesignationModel
    {
        public string DesignationName { get; set; }
    }
    public class MakeModel
    {
        public string PrinnterMakeName { get; set; }
    }
}



