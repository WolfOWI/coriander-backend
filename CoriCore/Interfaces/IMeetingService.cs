using System;
using CoriCore.DTOs;

namespace CoriCore.Interfaces;

public interface IMeetingService
{

    // GET
    // ========================================
    /// <summary>
    /// Get all meetings by employee id
    /// </summary>
    /// <param name="employeeId">Employee ID</param>
    /// <returns>List of meetings that the employee is involved in</returns>
    Task<IEnumerable<MeetingDTO>> GetMeetingsByEmployeeId(int employeeId);

    /// <summary>
    /// Get all meetings by admin id
    /// </summary>
    /// <param name="adminId">Admin ID</param>
    /// <returns>List of meetings that the admin is involved in</returns>
    Task<IEnumerable<MeetingDTO>> GetMeetingsByAdminId(int adminId);
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
    /// Admin confirms (accepts) & "creates" (updates) the meeting request with relevant details (MeetingConfirmDTO)
    /// </summary>
    /// <param name="meetingId">Meeting ID</param>
    /// <param name="meetingConfirmDTO">Meeting confirm DTO</param>
    /// <returns>Tuple with code and message</returns>
    Task<(int Code, string Message)> ConfirmAndUpdateMeetingRequest(int meetingId, MeetingConfirmDTO meetingConfirmDTO);
    // ========================================
    
    

}
