using System;
using CoriCore.DTOs;

namespace CoriCore.Interfaces;

public interface IEmpUserService
{
    Task<List<EmpUserDTO>> GetAllEmpUsers();

    Task<EmpUserDTO> GetEmpUserById(int id);

}
