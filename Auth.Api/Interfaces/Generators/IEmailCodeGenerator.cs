using Auth.Api.Models.Entities;

namespace Auth.Api.Interfaces.Generators;

public interface IEmailCodeGenerator
{
    public string GenerateCode();
}