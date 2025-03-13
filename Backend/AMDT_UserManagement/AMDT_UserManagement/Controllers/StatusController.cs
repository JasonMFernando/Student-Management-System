using AMDT_UserManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Input;

namespace AMDT_UserManagement.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        private readonly String connectionString;
        public StatusController(IConfiguration configuration)
        {
            connectionString = configuration["ConnectionStrings:DefaultConnection"] ?? "";
        }

        [HttpPost("CreateStatus")]
        public IActionResult CreateStatus(StatusDto status)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = @"INSERT INTO Status (StatusName, CreatedAt, ModifiedAt) 
                           VALUES (@StatusName, @CreatedAt, @ModifiedAt)";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@StatusName", status.StatusName);
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

        [HttpGet("GetAllStatuses")]
        public IActionResult GetAllStatuses()
        {
            try
            {
                var statuses = new List<Status>();
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT StatusID, StatusName, CreatedAt, ModifiedAt FROM Status";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                statuses.Add(new Status
                                {
                                    StatusID = reader.GetInt32(0),
                                    StatusName = reader.GetString(1),
                                    CreatedAt = reader.GetDateTime(2),
                                    ModifiedAt = reader.GetDateTime(3)
                                });
                            }
                        }
                    }
                }
                return Ok(statuses);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Internal server error", Error = ex.Message });
            }
        }


        [HttpGet("GetStatus/{id}")]
        public IActionResult GetStatus(int id)
        {
            try
            {
                Status status = null;
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT StatusID, StatusName, CreatedAt, ModifiedAt FROM Status WHERE StatusID = @Id";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                status = new Status
                                {
                                    StatusID = reader.GetInt32(0),
                                    StatusName = reader.GetString(1),
                                    CreatedAt = reader.GetDateTime(2),
                                    ModifiedAt = reader.GetDateTime(3)
                                };
                            }
                        }
                    }
                }

                if (status == null)
                {
                    return NotFound(new { Message = "Status not found" });
                }

                return Ok(status);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Internal server error", Error = ex.Message });
            }
        }

        [HttpPut("UpdateStatus/{id}")]
        public IActionResult UpdateStatus(int id, StatusDto status)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = @"UPDATE Status
                                   SET StatusName = @StatusName, 
                                       ModifiedAt = @ModifiedAt
                                   WHERE StatusID = @Id";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        command.Parameters.AddWithValue("@StatusName", status.StatusName);
                        command.Parameters.AddWithValue("@ModifiedAt", DateTime.Now);

                        var rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected == 0)
                        {
                            return NotFound(new { Message = "Status not found" });
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

        [HttpDelete("DeleteStatus/{id}")]
        public IActionResult DeleteStatus(int id)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "DELETE FROM Status WHERE StatusID = @Id";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        var rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected == 0)
                        {
                            return NotFound(new { Message = "Status not found" });
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
