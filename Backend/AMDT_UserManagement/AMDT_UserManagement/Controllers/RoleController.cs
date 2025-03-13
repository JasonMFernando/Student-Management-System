using AMDT_UserManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;

namespace AMDT_UserManagement.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly String connectionString;
        public RoleController(IConfiguration configuration) 
        {
            connectionString = configuration["ConnectionStrings:DefaultConnection"] ?? "";
        }

        [HttpPost("CreateRole")]
        public IActionResult CreateRole(RoleDto role)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = @"INSERT INTO RoleType (RoleName, Status, CreatedAt, ModifiedAt) 
                               VALUES (@RoleName, @Status, @CreatedAt, @ModifiedAt)";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@RoleName", role.RoleName);
                        command.Parameters.AddWithValue("@Status", role.Status);
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

            return Ok();
        }

        [HttpGet("GetAllRoles")]
        public IActionResult GetAllRoles()
        {
            try
            {
                var roles = new List<Role>();
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = @"
                SELECT r.RoleID, r.RoleName, r.Status, r.CreatedAt, r.ModifiedAt, 
                       s.StatusID, s.StatusName, s.CreatedAt AS StatusCreatedAt, s.ModifiedAt AS StatusModifiedAt
                FROM RoleType r
                INNER JOIN Status s ON r.Status = s.StatusID";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Role role = new Role
                                {
                                    RoleID = reader.GetInt32(0),
                                    RoleName = reader.GetString(1),
                                    CreatedAt = reader.GetDateTime(3),
                                    ModifiedAt = reader.GetDateTime(4),
                                    Status = new Status
                                    {
                                        StatusID = reader.GetInt32(5),
                                        StatusName = reader.GetString(6),
                                        CreatedAt = reader.GetDateTime(7),
                                        ModifiedAt = reader.GetDateTime(8)
                                    }
                                };
                                roles.Add(role);
                            }
                        }
                    }
                }
                return Ok(roles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Internal server error", Error = ex.Message });
            }
        }

        // Get Role by ID
        [HttpGet("GetRole/{id}")]
        public IActionResult GetRole(int id)
        {
            try
            {
                Role role = null;
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = @"
                SELECT r.RoleID, r.RoleName, r.Status, r.CreatedAt, r.ModifiedAt, 
                       s.StatusID, s.StatusName, s.CreatedAt AS StatusCreatedAt, s.ModifiedAt AS StatusModifiedAt
                FROM RoleType r
                INNER JOIN Status s ON r.Status = s.StatusID
                WHERE r.RoleID = @RoleID";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@RoleID", id);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                role = new Role
                                {
                                    RoleID = reader.GetInt32(0),
                                    RoleName = reader.GetString(1),
                                    CreatedAt = reader.GetDateTime(3),
                                    ModifiedAt = reader.GetDateTime(4),
                                    Status = new Status
                                    {
                                        StatusID = reader.GetInt32(5),
                                        StatusName = reader.GetString(6),
                                        CreatedAt = reader.GetDateTime(7),
                                        ModifiedAt = reader.GetDateTime(8)
                                    }
                                };
                            }
                        }
                    }
                }

                if (role == null)
                {
                    return NotFound(new { Message = "Role not found" });
                }

                return Ok(role);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Internal server error", Error = ex.Message });
            }
        }

        // Update Role
        [HttpPut("UpdateRole/{id}")]
        public IActionResult UpdateRole(int id, RoleDto role)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = @"UPDATE RoleType
                                   SET RoleName = @RoleName, 
                                       Status = @Status,
                                       ModifiedAt = @ModifiedAt
                                   WHERE RoleID = @RoleID";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@RoleID", id);
                        command.Parameters.AddWithValue("@RoleName", role.RoleName);
                        command.Parameters.AddWithValue("@Status", role.Status);
                        command.Parameters.AddWithValue("@ModifiedAt", DateTime.Now);

                        var rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected == 0)
                        {
                            return NotFound(new { Message = "Role not found" });
                        }
                    }
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Internal server error", Error = ex.Message });
            }
        }

        // Delete Role
        [HttpDelete("DeleteRole/{id}")]
        public IActionResult DeleteRole(int id)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "DELETE FROM RoleType WHERE RoleID = @RoleID";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@RoleID", id);
                        var rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected == 0)
                        {
                            return NotFound(new { Message = "Role not found" });
                        }
                    }
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Internal server error", Error = ex.Message });
            }
        }
    }
}
