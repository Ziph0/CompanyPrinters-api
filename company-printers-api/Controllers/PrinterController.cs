using Microsoft.AspNetCore.Mvc;
using company_printers_api.Models;
using CompanyPrinters.DAL;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Reflection.Metadata.Ecma335;
using Microsoft.AspNetCore.Http.HttpResults;

namespace company_printers_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrinterController : ControllerBase
    {
        private readonly DALPrint _dal;

        public PrinterController(IConfiguration config)
        {
            _dal = new DALPrint(config);
        }

        // GET METHODS

        [HttpGet("login")]
        public IActionResult Login([FromQuery] string username, [FromQuery] string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return BadRequest("Username and password are required.");

            DataTable dt = _dal.LoginUser(username, password);

            if (dt.Rows.Count > 0)
            {
                return Ok(new { designationId = dt.Rows[0]["DesignationID"] });
            }

            return Unauthorized("Invalid username or password.");
        }

        [HttpGet("printers")]
        public IActionResult GetAllPrinters()
        {
            DataTable dt = _dal.GetAllPrinters();
            var list = dt.AsEnumerable().Select(row => new
            {
                printerId = row["EngenPrintersID"],
                printerName = row["PrinterName"],
                makeID = row["PrinterMakeID"],
                makeName = row["PrinterMakeName"],
                folderToMonitor = row["FolderToMonitor"],
                outputType = row["OutputType"],
                fileOutput = row["FileOutput"],
                active = row["Active"],
                createdTimeStamp = row["CreatedTimeStamp"]
            }).ToList();

            return Ok(list);
        }

        [HttpGet("users")]
        public IActionResult GetAllUsers()
        {
            DataTable dt = _dal.GetAllUsers();
            var list = dt.AsEnumerable().Select(row => new
            {
                userId = row["UserID"],
                firstName = row["FirstName"],
                lastName = row["LastName"],
                designation = row["DesignationName"],
                email = row["Email"],
                username = row["UserName"],
                password = row["Password"],
                designationId = row["DesignationID"]
            }).ToList();

            return Ok(list);
        }
        [HttpGet("designation")]
        public IActionResult GetDesignationName()
        {
            DataTable dt = _dal.GetDesignationName();
            var list = new List<object>();

            foreach (DataRow row in dt.Rows)
            {
                list.Add(new
                {
                    designationId = row["DesignationID"],
                    designationName = row["DesignationName"],
                });
            }
            return Ok(list);
        }

        [HttpGet("makes")]
        public IActionResult GetPrinterMake()
        {
            try
            {
                DataTable dt = _dal.GetPrinterMake();
                var list = new List<object>();

                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new
                    {
                        printerMakeId = row["PrinterMakeID"],
                        printerMakeName = row["PrinterMakeName"]
                    });
                }

                return Ok(list);  
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
        [HttpGet("GetPrintersFiltered")]
        public IActionResult GetPrintersFiltered(int? makeId, DateTime? fromDate, DateTime? toDate)
        {
            var dt = _dal.GetPrintersFiltered(makeId, fromDate, toDate);

            var result = dt.Rows.Cast<DataRow>().Select(row =>
                dt.Columns.Cast<DataColumn>()
                  .ToDictionary(col => col.ColumnName, col => row[col])
            ).ToList();

            return Ok(result);
        }

        [HttpGet("filtered")]
        public IActionResult GetUsersFiltered([FromQuery] int? designationId)
        {
            var dt = _dal.GetUsersFiltered(designationId);


            var users = dt.AsEnumerable().Select(row => new UserModel
            {
               
                UserId = row.Table.Columns.Contains("UserId") ? row.Field<int>("UserId") : 0,
                DesignationId = row.Table.Columns.Contains("DesignationId") ? row.Field<int>("DesignationId") : 0,

                FirstName = row.Field<string>("FirstName"),
                LastName = row.Field<string>("LastName"),
                Email = row.Field<string>("Email"),
                UserName = row.Field<string>("UserName"),
                Password = row.Field<string>("Password"),
                DesignationName = row.Field<string>("DesignationName")
            }).ToList();

            return Ok(users);
        }

        //POST METHODS

        [HttpPost]
        public IActionResult AddPrinter([FromBody] PrinterModel model)
        {
            if (model.MakeID <= 0)
                return BadRequest(new { success = false, message = "Invalid printer make selected." });

            bool success = _dal.AddPrinter(
                model.PrinterName,
                model.MakeID,
                model.FolderToMonitor,
                model.OutputType,
                model.FileOutput,
                model.Active
            );

            return success
                ? Ok(new { success = true, message = "Printer added successfully." })
                : StatusCode(500, new { success = false, message = "Failed to add printer." });
        }


        [HttpPost("users")]
        public IActionResult AddUser([FromBody] UserModel model)
        {
            try
            {
                var designations = _dal.GetDesignationName();
                var desigRow = designations.AsEnumerable()
                    .FirstOrDefault(r => string.Equals(
                        r["DesignationName"].ToString().Trim(),
                        model.DesignationName?.Trim(),
                        StringComparison.OrdinalIgnoreCase));

                if (desigRow == null)
                    return BadRequest(new { message = "Invalid designation selected." });

                int designationId = Convert.ToInt32(desigRow["DesignationID"]);

                
                bool success = _dal.AddUser(
                    model.FirstName,
                    model.LastName,
                    designationId,
                    model.Email,
                    model.UserName,
                    model.Password
                );

                return Ok(new { success = true, message = "User added successfully." });
            }
            catch (Microsoft.Data.SqlClient.SqlException ex)
            {
               
                if (ex.Number == 2627 || ex.Number == 2601)
                {
                    return Conflict(new { message = $"The username '{model.UserName}' or email is already in use." });
                }

                return StatusCode(500, new { message = "Database error: " + ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }


        [HttpPost("designation")]
        public IActionResult AddDesignation([FromBody] DesignationModel model)
        {
            try
            {
               
                DataTable dt = _dal.GetDesignationName();
                bool exists = dt.AsEnumerable().Any(r =>
                    string.Equals(r["DesignationName"].ToString().Trim(),
                    model.DesignationName.Trim(),
                    StringComparison.OrdinalIgnoreCase));

                if (exists)
                    return BadRequest("This designation already exists.");

                _dal.AddDesignation(model.DesignationName);
                return Ok(new { success = true, message = "Designation added successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        // PUT METHODS

        [HttpPut("{id}")]
        public IActionResult UpdatePrinter(int id, [FromBody] PrinterModel model)
        {
            if (model.MakeID <= 0)
                return BadRequest(new { success = false, message = "Invalid printer make selected." });

            bool success = _dal.UpdatePrinter(
                id,
                model.PrinterName,
                model.MakeID,
                model.FolderToMonitor,
                model.OutputType,
                model.FileOutput,
                model.Active
            );

            return success
                ? Ok(new { success = true, message = "Printer updated successfully." })
                : NotFound(new { success = false, message = $"Printer with ID {id} not found." });
        }



        [HttpPut("users/{id}")]
        public IActionResult UpdateUser(int id, [FromBody] UserModel model)
        {
            try
            {
                var designations = _dal.GetDesignationName();
                var desigRow = designations.AsEnumerable()
                    .FirstOrDefault(r => string.Equals(
                        r["DesignationName"].ToString().Trim(),
                        model.DesignationName?.Trim(),
                        StringComparison.OrdinalIgnoreCase));

                if (desigRow == null)
                    return BadRequest(new { message = "Invalid designation selected." });

                int designationId = Convert.ToInt32(desigRow["DesignationID"]);

                
                _dal.UpdateUser(id, model.FirstName, model.LastName, designationId, model.Email, model.UserName ,model.Password);

                return Ok(new { success = true, message = "User updated successfully." });
            }
            catch (Microsoft.Data.SqlClient.SqlException ex)
            {
                // 2627/2601 = Unique Key violation (Username/Email collision)
                // 547 = Foreign Key violation (Designation ID doesn't exist)
                if (ex.Number == 2627 || ex.Number == 2601)
                {
                    return Conflict(new { message = "Update failed: That username or email is already taken by another user." });
                }
                if (ex.Number == 547)
                {
                    return BadRequest(new { message = "Update failed: The selected designation is invalid." });
                }

                return StatusCode(500, new { message = "Database error during update." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }




        [HttpPut("designation/{id}")]
        public IActionResult UpdateDesignation(int id, [FromBody] DesignationModel model)
        {
            try
            {
                _dal.UpdateDesignation(id, model.DesignationName);
                return Ok("Designation updated successfully.");
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message); 
            }
            catch
            {
                return StatusCode(500, "Internal server error");
            }
        }


        // DELETE METHODS

        [HttpDelete("printer/{id}")]
        public IActionResult DeletePrinter(int id)
        {
            try
            {
                bool deleted = _dal.DeletePrinter(id);

                if (!deleted)
                {
                    return NotFound(new { message = $"Printer with ID {id} not found." });
                }

                return Ok(new { message = $"Printer with ID {id} deleted successfully." });
            }
            catch (Microsoft.Data.SqlClient.SqlException ex)
            {
                
                if (ex.Number == 547)
                {
                    return Conflict(new
                    {
                        message = "Cannot delete this printer because it is currently assigned to one or more users. Please reassign the users first."
                    });
                }

                return StatusCode(500, new { message = "A database error occurred while deleting." });
            }
            
        }

        [HttpDelete("user/{id}")]
        public IActionResult DeleteUser(int id)
        {
            return _dal.DeleteUser(id) ? Ok("User deleted successfully.") : BadRequest("Failed to delete user.");
        }

       
        [HttpDelete("designation/{id}")]
        public IActionResult DeleteDesignation(int id)
        {
            try
            {
                bool deleted = _dal.DeleteDesignation(id);
                if (!deleted)
                    return NotFound(new { message = "Designation not found." });

                return Ok(new { message = "Designation deleted successfully." });
            }
            catch (Microsoft.Data.SqlClient.SqlException ex)
            {
                // 547 = Foreign Key Constraint violation
                if (ex.Number == 547)
                {
                    return Conflict(new
                    {
                        message = "Cannot delete this designation because users are currently assigned to it. Please reassign the users first."
                    });
                }
                return StatusCode(500, new { message = "A database error occurred." });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }
    }
}