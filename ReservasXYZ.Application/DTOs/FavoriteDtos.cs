namespace ReservasXYZ.Application.DTOs;

public class FavoriteToggleResultDto
{
    public int RoomId { get; set; }
    public bool IsFavorite { get; set; }
    public DateTime? CreatedAt { get; set; }
}