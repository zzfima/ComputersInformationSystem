public class IP
{
    public int Id { get; set; }
    public string Address { get; set; } = string.Empty;
    public bool IsAlive { get; set; } = false;
    public DateTime LastUpdate { get; set; }
}