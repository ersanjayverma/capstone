using System.ComponentModel.DataAnnotations;

namespace ZTACS.Shared.Models
{
    public class ThreatDetectionRequest
    {
        [Required] public string UserId { get; set; }
        [Required] public string Ip { get; set; }
        [Required] public string Device { get; set; }
        [Required] public string Endpoint { get; set; }
        [Required] public DateTime Timestamp { get; set; }
    }
}