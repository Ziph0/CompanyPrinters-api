using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace CompanyPrinters.DAL
{
    public class DALPrint(IConfiguration config)
    {
        private readonly string connectionString = config.GetConnectionString("DefaultConnection");

        public DataTable GetAllPrinters()
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("GetPrinters", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }

            return dt;
        }
        
          public DataTable GetAllUsers()
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("GetAllUsers", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }

            return dt;
        }
         
        public DataTable LoginUser(string username, string password)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("LoginUser", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@Password", password);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }

            return dt;
        }
        public DataTable GetPrinterMake()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("GetMakeName", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                DataTable dt = new DataTable();
                conn.Open();
                dt.Load(cmd.ExecuteReader());

                return dt;
            }

        }
        
          public DataTable GetDesignationName()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("GetDesignation", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                DataTable dt = new DataTable();
                conn.Open();
                dt.Load(cmd.ExecuteReader());

                return dt;
            }

        }
         
        public DataTable GetPrintersFiltered(int? makeId, DateTime? fromDate, DateTime? toDate)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("GetPrintersFiltered", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@MakeId", (object)makeId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FromDate", (object)fromDate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ToDate", (object)toDate ?? DBNull.Value);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }

            return dt;
        }
        public DataTable GetUsersFiltered(int? designationId)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("GetUsersFiltered", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                
                cmd.Parameters.AddWithValue("@DesignationID", (designationId == 0 || designationId == null) ? (object)DBNull.Value : designationId);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }

            return dt;
        }
        public bool UpdatePrinter(int id, string name, int makeId, string folder, string outputType, string fileOutput, bool active)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("UpdatePrinter", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@EngenPrintersID", id);
                    cmd.Parameters.AddWithValue("@PrinterName", name ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@PrinterMakeID", makeId);
                    cmd.Parameters.AddWithValue("@FolderToMonitor", folder ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@OutputType", outputType ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@FileOutput", fileOutput ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Active", active);

                    conn.Open();
                    int rows = cmd.ExecuteNonQuery();
                    return rows > 0;
                }
            }
        }
        public bool UpdateUser(int userId, string firstName, string lastName, int designationId, string email, string username, string password)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // 1️⃣ check if user exists
                using (SqlCommand checkCmd = new SqlCommand("SELECT COUNT(1) FROM Users WHERE UserID=@UserID", conn))
                {
                    checkCmd.Parameters.AddWithValue("@UserID", userId);
                    int exists = (int)checkCmd.ExecuteScalar();
                    if (exists == 0)
                        return false; // user truly doesn't exist
                }

                // 2️⃣ update user
                using (SqlCommand cmd = new SqlCommand("UPDATE Users SET FirstName=@FirstName, LastName=@LastName, Email=@Email, UserName=@UserName, Password=@Password, DesignationID=@DesignationID WHERE UserID=@UserID", conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    cmd.Parameters.AddWithValue("@FirstName", firstName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@LastName", lastName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", email ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@UserName", username ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Password", password ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@DesignationID", designationId);

                    cmd.ExecuteNonQuery();
                }

                return true; // user exists, update attempted
            }
        }

        public bool DeletePrinter(int printerId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("DeletePrinter", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                   
                    cmd.Parameters.AddWithValue("@EngenPrintersID", printerId);

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    

                    return rowsAffected > 0;
                }
            }
        }
        public  bool DeleteUser(int userId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("DeleteUser", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserID", userId);

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                   
                    return rowsAffected > 0;
                }
            }
        }
        public bool AddPrinter(string name, int makeId, string folder,
                       string outputType, string fileOutput, bool active)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("AddPrinter", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@PrinterName", name ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@PrinterMakeID", makeId);
                cmd.Parameters.AddWithValue("@FolderToMonitor", folder ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@OutputType", outputType ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@FileOutput", fileOutput ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Active", active);

                conn.Open();
                int rows = cmd.ExecuteNonQuery();

                if (rows == 0)
                    throw new Exception("Stored procedure executed but no rows were inserted.");

                return true;
            }
        }



        public void AddDesignation(string name)
        {
            
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                   
                    using (SqlCommand cmd = new SqlCommand("AddDesignation", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@DesignationName", name);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }


        public bool UpdateDesignation(int id, string name)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(connectionString);
                using SqlCommand cmd = new SqlCommand("UpdateDesignation", conn);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@DesignationID", SqlDbType.Int).Value = id;
                cmd.Parameters.Add("@DesignationName", SqlDbType.NVarChar, 100).Value = name ?? (object)DBNull.Value;

                conn.Open();
                int rows = cmd.ExecuteNonQuery();

              
                return true;
            }
            catch (SqlException ex)
            {
                if (ex.Number == 50000)
                {
                    throw new ApplicationException(ex.Message);
                }
                throw;
            }
        }



        public bool DeleteDesignation(int id)
        {
            using SqlConnection conn = new SqlConnection(connectionString);
            using SqlCommand cmd = new SqlCommand("DeleteDesignation", conn);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@DesignationID", SqlDbType.Int).Value = id;

            conn.Open();
            int rows = cmd.ExecuteNonQuery();

            return rows > 0; 
        }

        public bool AddUser(string firstName, string lastName, int designationId,
                    string email, string username, string password)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("AddUser", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@FirstName", firstName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@LastName", lastName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Email", email ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@UserName", username ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Password", password ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@DesignationID", designationId);

                conn.Open();
                int rows = cmd.ExecuteNonQuery();

                  if (rows == 0)
                    throw new Exception("Stored procedure executed but no rows were inserted.");

                return true;
            }
        }


    }

}

