using ZTACS.Shared.Entities;
namespace ZTACS.Shared.Models;

public class LogResponse
{
    public int Total { get; set; }=0;
    public List<LoginEvent> Logs { get; set; } = new();
}
