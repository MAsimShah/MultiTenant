using DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Interfaces
{
    public interface IAccountServices
    {
        Task<SignupDTO> SignupUserAsync(SignupDTO model);
    }
}
