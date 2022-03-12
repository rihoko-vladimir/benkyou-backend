using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Benkyou.Domain.Models.Requests;

public class UpdateUserFirstNameRequest
{
    [JsonPropertyName("firstName")]
    [Required]
    public string FirstName { get; set; }
}