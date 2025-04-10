// Iné Smith

using System;
using CoriCore.DTOs;

namespace CoriCore.Interfaces;

public interface IApplyForLeaveService
{
    // ApplyForLeave(ApplyForLeaveDTO leaveRequest)
    Task<ApplyForLeaveDTO> ApplyForLeave(ApplyForLeaveDTO leaveRequest);

    // Creates a new leave request entry for an employee, based on provided leave details.
    // This method will also check if the employee has enough leave balance before applying.

    // Returns the created leave request object or confirmation of successful submission.

}
