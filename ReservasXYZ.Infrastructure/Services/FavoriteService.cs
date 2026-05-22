using Microsoft.EntityFrameworkCore;
using ReservasXYZ.Application.DTOs;
using ReservasXYZ.Application.Interfaces;
using ReservasXYZ.Domain.Entities;
using ReservasXYZ.Infrastructure.Data.Context;

namespace ReservasXYZ.Infrastructure.Services;

public class FavoriteService : IFavoriteService
{
    private readonly ApplicationDbContext _context;

    public FavoriteService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<int>> GetFavoriteRoomIdsAsync(string userId, IEnumerable<int> roomIds)
    {
        var selectedRoomIds = roomIds
            .Where(id => id > 0)
            .Distinct()
            .ToList();

        if (string.IsNullOrWhiteSpace(userId) || selectedRoomIds.Count == 0)
        {
            return Array.Empty<int>();
        }

        return await _context.Favorites
            .AsNoTracking()
            .Where(favorite => favorite.UserId == userId && selectedRoomIds.Contains(favorite.RoomId))
            .Select(favorite => favorite.RoomId)
            .ToListAsync();
    }

    public async Task<bool> IsRoomFavoriteAsync(string userId, int roomId)
    {
        return !string.IsNullOrWhiteSpace(userId)
            && roomId > 0
            && await _context.Favorites.AnyAsync(favorite => favorite.UserId == userId && favorite.RoomId == roomId);
    }

    public async Task<FavoriteToggleResultDto> ToggleRoomFavoriteAsync(string userId, int roomId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new UnauthorizedAccessException("Debes iniciar sesión para guardar favoritos.");
        }

        var roomExists = await _context.Rooms.AnyAsync(room => room.Id == roomId && room.IsActive);
        if (!roomExists)
        {
            throw new KeyNotFoundException("La habitación no está disponible para favoritos.");
        }

        var existingFavorite = await _context.Favorites
            .FirstOrDefaultAsync(favorite => favorite.UserId == userId && favorite.RoomId == roomId);

        if (existingFavorite is not null)
        {
            _context.Favorites.Remove(existingFavorite);
            await _context.SaveChangesAsync();

            return new FavoriteToggleResultDto
            {
                RoomId = roomId,
                IsFavorite = false
            };
        }

        var favorite = new Favorite
        {
            UserId = userId,
            RoomId = roomId
        };

        _context.Favorites.Add(favorite);
        await _context.SaveChangesAsync();

        return new FavoriteToggleResultDto
        {
            RoomId = roomId,
            IsFavorite = true,
            CreatedAt = favorite.CreatedAt
        };
    }
}