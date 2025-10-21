﻿using IdentityConnectionLib.ConnectionServices.Dtos;

namespace IdentityConnectionLib.ConnectionServices.Interfaces;

public interface IIdentityConnectionService
{
    Task<CheckUserExistsIdentityServiceResponse> CheckUserExistAsync(CheckUserExistsIdentityServiceRequest checkUserExistProfileApiRequest);
}