using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Car
    {
        [Required]
        [JsonPropertyName("id")]
        public int  Id { get; set; }

        [Required]
        [JsonPropertyName("dealerId")]
        public int DealerId { get; set; }

        [Required]
        [JsonPropertyName("make")]
        public required string Make { get; set; }

        [Required]
        [JsonPropertyName("model")]
        public required string Model { get; set; }

        [Required]
        [JsonPropertyName("year")]
        public int Year { get; set; }
    }
}