﻿using System.Security.Claims;
using Benkyou.Application.Common;
using Benkyou.Application.Services;
using Benkyou.Domain.Entities;
using Benkyou.Domain.Enums;
using Benkyou.Domain.Models;

namespace Benkyou.Infrastructure.Services;

public class JwtAccessTokenService : IAccessTokenService
{
    private readonly JwtProperties _jwtProperties;
    private readonly ITokenGenerator _tokenGenerator;

    public JwtAccessTokenService(ITokenGenerator tokenGenerator, JwtProperties jwtProperties)
    {
        _tokenGenerator = tokenGenerator;
        _jwtProperties = jwtProperties;
    }

    public string GetToken(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Role, Roles.User),
            new(ClaimTypes.Name, user.FirstName),
            new(ClaimTypes.Email, user.Email)
        };
        var token = _tokenGenerator.GenerateToken(_jwtProperties.AccessSecret, _jwtProperties.Issuer,
            _jwtProperties.Audience, _jwtProperties.AccessTokenExpirationTime, claims);
        return token;
    }
}