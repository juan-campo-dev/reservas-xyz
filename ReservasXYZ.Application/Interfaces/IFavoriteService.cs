using ReservasXYZ.Application.DTOs;

namespace ReservasXYZ.Application.Interfaces;

public interface IFavoriteService
{
    Task<IReadOnlyCollection<int>> GetFavoriteRoomIdsAsync(string userId, IEnumerable<int> roomIds);
    Task<bool> IsRoomFavoriteAsync(string userId, int roomId);
    Task<FavoriteToggleResultDto> ToggleRoomFavoriteAsync(string userId, int roomId);
}