using System;
using CoriCore.DTOs;

namespace CoriCore.Interfaces;

public interface IEmpUserService
{
    Task<List<EmpUserDTO>> GetAllEmpUsers();

    Task<EmpUserDTO> GetEmpUserByEmpId(int id);

}
