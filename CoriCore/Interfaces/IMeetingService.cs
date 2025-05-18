using System;
using CoriCore.DTOs;
using CoriCore.Models;

namespace CoriCore.Interfaces;

public interface IMeetingService
{

    // GET
    // ========================================
    // EMPLOYEE RELATED 
    // ------------------------
    /// <summary>
    /// Get all meetings by employee id
    /// </summary>
    /// <param name="employeeId">Employee ID</param>
    /// <returns>List of ALL meetings that the employee is involved in</returns>
    Task<IEnumerable<MeetingDTO>> GetAllMeetingsByEmployeeId(int employeeId);


    /// <summary>
    /// Get meetings by employee id and status
    /// </summary>
    /// <param name="employeeId">Employee ID</param>
    /// <param name="status">Meeting status</param>
    /// <returns>List of meetings that the employee is involved in with the given status</returns>
    Task<IEnumerable<MeetingDTO>> GetMeetingsByEmployeeIdAndStatus(int employeeId, MeetStatus status);
    // ------------------------

    // ADMIN RELATED 
    // ------------------------
    /// <summary>
    /// Get all meetings by admin id
    /// </summary>
    /// <param name="adminId">Admin ID</param>
    /// <returns>List of ALL meetings that the admin is involved in</returns>
    Task<IEnumerable<MeetingDTO>> GetAllMeetingsByAdminId(int adminId);
    // ------------------------
    // ========================================

    // CREATE
    // ========================================
    /// <summary>
    /// Create a pending meeting request (usually by employee - without all the meeting details (to be created by admin later on))
    /// </summary>
    /// <param name="meetingRequestCreateDTO">Meeting request create DTO</param>
    /// <returns>Meeting request</returns>
    Task<MeetingRequestCreateDTO> CreateMeetingRequest(MeetingRequestCreateDTO meetingRequestCreateDTO);
    // ========================================

    // UPDATE
    // ========================================
    /// <summary>
    /// Admin confirms (accepts) & "creates" (updates) the meeting request with relevant details (MeetingUpdateDTO)
    /// </summary>
    /// <param name="meetingId">Meeting ID</param>
    /// <param name="dto">Meeting update DTO</param>
    /// <returns>Tuple with code and message</returns>
    Task<(int Code, string Message)> ConfirmAndUpdateMeetingRequest(int meetingId, MeetingUpdateDTO dto);

    /// <summary>
    /// Admin updates the meeting request details
    /// </summary>
    /// <param name="meetingId">Meeting ID</param>
    /// <param name="dto">Meeting update DTO</param>
    /// <returns>Tuple with code and message</returns>
    Task<(int Code, string Message)> UpdateMeeting(int meetingId, MeetingUpdateDTO dto);

    /// <summary>
    /// Employee edits their meeting request
    /// </summary>
    /// <param name="meetingId">Meeting ID</param>
    /// <param name="dto">Meeting request update DTO</param>
    /// <returns>Tuple with code and message</returns>
    Task<(int Code, string Message)> UpdateMeetingRequest(int meetingId, MeetingRequestUpdateDTO dto);

    /// <summary>
    /// Admin rejects the meeting request
    /// </summary>
    /// <param name="meetingId">Meeting ID</param>
    /// <returns>Tuple with code and message</returns>
    Task<(int Code, string Message)> RejectMeetingRequest(int meetingId);


    /// <summary>
    /// Admin marks the meeting as completed
    /// </summary>
    /// <param name="meetingId">Meeting ID</param>
    /// <returns>Tuple with code and message</returns>
    Task<(int Code, string Message)> MarkMeetingAsCompleted(int meetingId);
    // ========================================

    // DELETE
    // ========================================
    /// <summary>
    /// Delete a meeting
    /// </summary>
    /// <param name="meetingId">Meeting ID</param>
    /// <returns>Tuple with code and message</returns>
    Task<(int Code, string Message)> DeleteMeeting(int meetingId);
    // ========================================
}
