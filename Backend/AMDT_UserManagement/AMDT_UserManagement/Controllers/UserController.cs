using AMDT_UserManagement.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System;
using System.Text;
using System.Security.Cryptography;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace AMDT_UserManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly String connectionString;
        private readonly IConfiguration _configuration;
        public UserController(IConfiguration configuration)
        {
            _configuration = configuration;
            connectionString = _configuration["ConnectionStrings:DefaultConnection"] ?? "";
        }

        [HttpPost("Login")]
        public IActionResult Login([FromBody] LoginDto loginDto)
        {
            try
            {
                // Checking for default Credentials
                if (loginDto.Email == "admin@example.com" && loginDto.Password == "admin")
                { 
                    string token = GenerateJwtToken(0, loginDto.Email, 1);
                    return Ok(new { Token = token, Message = "Login successful" });
                }

                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = @"SELECT UserID, FirstName, LastName, Email, Password, RoleType, Status 
                                   FROM UserDetails WHERE Email = @Email";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Email", loginDto.Email);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int userId = (int)reader["UserID"];
                                string firstName = reader["FirstName"].ToString();
                                string lastName = reader["LastName"].ToString();
                                string email = reader["Email"].ToString();
                                string hashedPassword = reader["Password"].ToString();
                                int roleType = (int)reader["RoleType"];
                                int status = (int)reader["Status"];

                                // Hash the entered password and compare
                                if (hashedPassword == HashPassword(loginDto.Password))
                                {
                                    string token = GenerateJwtToken(userId, email, roleType);
                                    return Ok(new { Token = token, Message = "Login successful" });
                                }
                            }
                        }
                    }
                }

                return Unauthorized(new { Message = "Invalid email or password" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Internal server error", Error = ex.Message });
            }
        }

        [HttpPost("CreateUser")]
        public IActionResult CreateUser(UserDto user)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = @"INSERT INTO UserDetails (FirstName, LastName, Email, Password, DateOfBirth, RoleType, Status, CreatedAt, ModifiedAt) 
                           VALUES (@FirstName, @LastName, @Email, @Password, @DateOfBirth, @RoleType, @Status, @CreatedAt, @ModifiedAt)";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@FirstName", user.FirstName);
                        command.Parameters.AddWithValue("@LastName", user.LastName);
                        command.Parameters.AddWithValue("@Email", user.Email);
                        command.Parameters.AddWithValue("@Password", HashPassword(user.Password));
                        command.Parameters.AddWithValue("@DateOfBirth", user.DateOfBirth);
                        command.Parameters.AddWithValue("@RoleType", user.RoleType);
                        command.Parameters.AddWithValue("@Status", user.Status);
                        command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
                        command.Parameters.AddWithValue("@ModifiedAt", DateTime.Now);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Internal server error", Error = ex.Message });
            }

            return Ok(new { Message = "User created successfully" });
        }

        [Authorize]
        [HttpPut("UpdateUser/{id}")]
        public IActionResult UpdateUser(int id, UserDto user)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = @"UPDATE UserDetails
                           SET FirstName = @FirstName, 
                               LastName = @LastName, 
                               Email = @Email, 
                               Password = @Password, 
                               DateOfBirth = @DateOfBirth, 
                               RoleType = @RoleType, 
                               Status = @Status, 
                               ModifiedAt = @ModifiedAt
                           WHERE UserID = @UserID";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@UserID", id);
                        command.Parameters.AddWithValue("@FirstName", user.FirstName);
                        command.Parameters.AddWithValue("@LastName", user.LastName);
                        command.Parameters.AddWithValue("@Email", user.Email);
                        command.Parameters.AddWithValue("@Password", HashPassword(user.Password)); // assuming you want to hash the password again
                        command.Parameters.AddWithValue("@DateOfBirth", user.DateOfBirth);
                        command.Parameters.AddWithValue("@RoleType", user.RoleType);
                        command.Parameters.AddWithValue("@Status", user.Status);
                        command.Parameters.AddWithValue("@ModifiedAt", DateTime.Now);

                        var rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected == 0)
                        {
                            return NotFound(new { Message = "User not found" });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Internal server error", Error = ex.Message });
            }

            return Ok(new { Message = "User updated successfully" });
        }

        [Authorize]
        [HttpGet("GetAllUsers")]
        public IActionResult GetAllUsers()
        {
            try
            {
                var users = new List<User>();
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = @"
                        SELECT u.UserID, u.FirstName, u.LastName, u.Email, u.DateOfBirth, u.RoleType, r.RoleName, u.Status, s.StatusName, u.CreatedAt, u.ModifiedAt
                        FROM UserDetails u
                        JOIN RoleType r ON u.RoleType = r.RoleID
                        JOIN Status s ON u.Status = s.StatusID";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var user = new User
                                {
                                    UserID = reader.GetInt32(0),
                                    FirstName = reader.GetString(1),
                                    LastName = reader.GetString(2),
                                    Email = reader.GetString(3),
                                    DateOfBirth = reader.IsDBNull(4) ? default(DateTime) : DateTime.TryParse(reader.GetString(4), out var parsedDate) ? parsedDate : default(DateTime),
                                    Role = new Role
                                    {
                                        RoleID = reader.GetInt32(5),
                                        RoleName = reader.GetString(6)
                                    },
                                    Status = new Status
                                    {
                                        StatusID = reader.GetInt32(7),
                                        StatusName = reader.GetString(8)
                                    },
                                    CreatedAt = reader.GetDateTime(9),
                                    ModifiedAt = reader.GetDateTime(10)
                                };
                                users.Add(user);
                            }
                        }
                    }
                }
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Internal server error", Error = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("GetUser/{id}")]
        public IActionResult GetUserById(int id)
        {
            try
            {
                User user = null;
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = @"
                        SELECT u.UserID, u.FirstName, u.LastName, u.Email, u.DateOfBirth, u.RoleType, r.RoleName, u.Status, s.StatusName, u.CreatedAt, u.ModifiedAt
                        FROM UserDetails u
                        JOIN RoleType r ON u.RoleType = r.RoleID
                        JOIN Status s ON u.Status = s.StatusID
                        WHERE u.UserID = @UserID";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@UserID", id);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                user = new User
                                {
                                    UserID = reader.GetInt32(0),
                                    FirstName = reader.GetString(1),
                                    LastName = reader.GetString(2),
                                    Email = reader.GetString(3),
                                    DateOfBirth = reader.IsDBNull(4) ? default(DateTime) : DateTime.TryParse(reader.GetString(4), out var parsedDate) ? parsedDate : default(DateTime),
                                    Role = new Role
                                    {
                                        RoleID = reader.GetInt32(5),
                                        RoleName = reader.GetString(6)
                                    },
                                    Status = new Status
                                    {
                                        StatusID = reader.GetInt32(7),
                                        StatusName = reader.GetString(8)
                                    },
                                    CreatedAt = reader.GetDateTime(9),
                                    ModifiedAt = reader.GetDateTime(10)
                                };
                            }
                        }
                    }
                }

                if (user == null)
                {
                    return NotFound(new { Message = "User not found" });
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Internal server error", Error = ex.Message });
            }
        }


        [Authorize]
        [HttpGet("GetUserByEmail/{email}")]
        public IActionResult GetUserByEmail(string email)
        {
            try
            {
                User user = null;
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = @"
                SELECT u.UserID, u.FirstName, u.LastName, u.Email, u.DateOfBirth, u.RoleType, r.RoleName, u.Status, s.StatusName, u.CreatedAt, u.ModifiedAt
                FROM UserDetails u
                JOIN RoleType r ON u.RoleType = r.RoleID
                JOIN Status s ON u.Status = s.StatusID
                WHERE u.Email = @Email";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Email", email);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                user = new User
                                {
                                    UserID = reader.GetInt32(0),
                                    FirstName = reader.GetString(1),
                                    LastName = reader.GetString(2),
                                    Email = reader.GetString(3),
                                    DateOfBirth = reader.IsDBNull(4) ? default(DateTime) : DateTime.TryParse(reader.GetString(4), out var parsedDate) ? parsedDate : default(DateTime),
                                    Role = new Role
                                    {
                                        RoleID = reader.GetInt32(5),
                                        RoleName = reader.GetString(6)
                                    },
                                    Status = new Status
                                    {
                                        StatusID = reader.GetInt32(7),
                                        StatusName = reader.GetString(8)
                                    },
                                    CreatedAt = reader.GetDateTime(9),
                                    ModifiedAt = reader.GetDateTime(10)
                                };
                            }
                        }
                    }
                }

                if (user == null)
                {
                    return NotFound(new { Message = "User not found" });
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Internal server error", Error = ex.Message });
            }
        }


        [Authorize]
        [HttpDelete("DeleteUser/{id}")]
        public IActionResult DeleteUser(int id)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = @"DELETE FROM UserDetails WHERE UserID = @UserID";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@UserID", id);
                        var rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected == 0)
                        {
                            return NotFound(new { Message = "User not found" });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Internal server error", Error = ex.Message });
            }

            return Ok(new { Message = "User deleted successfully" });
        }



        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        private string GenerateJwtToken(int userId, string email, int roleType)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim("roleType", roleType.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
