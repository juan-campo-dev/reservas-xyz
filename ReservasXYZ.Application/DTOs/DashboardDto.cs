namespace ReservasXYZ.Application.DTOs;

public class DashboardDto
{
    public int TotalSites { get; set; }
    public int TotalAccommodations { get; set; }
    public int TotalRooms { get; set; }
    public int TotalReservations { get; set; }
    public int PendingReservations { get; set; }
    public int ConfirmedReservations { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal MonthlyRevenue { get; set; }
    public int OccupiedRoomsToday { get; set; }
    public double OccupancyRate { get; set; }
    public List<ReservationDto> RecentReservations { get; set; } = new();
}
