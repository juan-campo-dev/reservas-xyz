namespace ReservasXYZ.Domain.Entities;

public class Season
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal PriceMultiplier { get; set; } = 1.0m;
    public bool IsActive { get; set; } = true;

    public ICollection<Rate> Rates { get; set; } = new List<Rate>();
}
