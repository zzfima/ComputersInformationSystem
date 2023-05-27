using System.ComponentModel.DataAnnotations.Schema;

public class InstalledVersion
{
    public int Id { get; set; }

    public string Path { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int ConfigurationId { get; set; } // Required foreign key property

}