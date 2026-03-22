using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Dealer
    {
        [Required]
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [Required]
        [JsonPropertyName("name")]
        public required string Name { get; set; }
    }   
}