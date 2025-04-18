// Equipment Service Interface
// ========================================

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoriCore.DTOs;
using CoriCore.Models;

namespace CoriCore.Interfaces;

public interface IEquipmentService
{
    /// <summary>
    /// Gets all equipment assigned to a specific employee
    /// </summary>
    /// <param name="employeeId">The ID of the employee</param>
    /// <returns>A list of equipment DTOs</returns>
    Task<List<EquipmentDTO>> GetEquipmentByEmployeeId(int employeeId);

    Task<IEnumerable<Equipment>> CreateEquipmentItemsAsync(List<CreateEquipmentDTO> equipmentDTOs);

    Task<Equipment> EditEquipmentItemAsync(int equipmentId, EquipmentDTO equipmentDTO);

    Task<bool> DeleteEquipmentItemAsync(int equipmentId);

    Task<(int Code, string Message)> AssignEquipmentAsync(int employeeId, List<int> equipmentIds);
    
    // Force Assign: Used for when we want to change the employee that an equipment item is assigned to
    /// <summary>
    /// Force assign equipment to an employee, regardless of whether it's already assigned to another employee
    /// </summary>
    /// <param name="employeeId">The ID of the employee</param>
    /// <param name="equipmentIds">The IDs of the equipment items to assign</param>
    /// <returns>The status code and a message</returns>
    Task<(int Code, string Message)> ForceAssignEquipmentAsync(int employeeId, List<int> equipmentIds);

    /// <summary>
    /// Gets all assigned equipment items with additional info about user/employee (if assigned)
    /// </summary>
    /// <returns>A list of equipment DTOs</returns>
    Task<List<EmpEquipItemDTO>> GetAllAssignedEquipItems();


    /// <summary>
    /// Gets all unassigned equipment items
    /// </summary>
    /// <returns>A list of equipment DTOs</returns>
    Task<List<EquipmentDTO>> GetAllUnassignedEquipItems();

    /// <summary>
    /// Unlink an equipment item from an employee (set EmployeeId & AssignmentDate to null)
    /// </summary>
    /// <param name="equipmentId">The ID of the equipment item</param>
    /// <returns>A boolean value indicating whether the operation was successful</returns>
    Task<(int Code, string Message)> UnlinkEquipmentFromEmployee(int equipmentId);


    
}