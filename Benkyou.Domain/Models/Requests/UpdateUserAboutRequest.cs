using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Benkyou.Domain.Models.Requests;

public class UpdateUserAboutRequest
{
    [JsonPropertyName("about")] [Required] public string About { get; set; }
}