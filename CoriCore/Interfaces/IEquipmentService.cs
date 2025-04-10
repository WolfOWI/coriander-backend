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

    Task<IEnumerable<Equipment>> CreateEquipmentItemsAsync(List<EquipmentDTO> equipmentDTOs);

    Task<Equipment> EditEquipmentItemAsync(int equipmentId, EquipmentDTO equipmentDTO);

    Task<bool> DeleteEquipmentItemAsync(int equipmentId);

} 