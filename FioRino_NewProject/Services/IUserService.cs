﻿using FioRino_NewProject.DataTransferObjects;
using FioRino_NewProject.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FioRino_NewProject.Services
{
    public interface IUserService
    {
        Task<Response> UpdateUserById(int id, UpdateUserDTO dmUsers);
        Task<Response> CheckValidityEmail(int id, UpdateUserDTO dmUsers);
    }
}
