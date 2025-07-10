namespace ZTACS.Shared.Models
{
    public class ThreatDetectionResponse
    {
        public int RiskScore { get; set; } // 0 to 100
        public string Status { get; set; } // clean | suspicious | blocked
        public string Reason { get; set; }
    }
}