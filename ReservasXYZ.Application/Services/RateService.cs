using AutoMapper;
using ReservasXYZ.Application.DTOs;
using ReservasXYZ.Application.Interfaces;
using ReservasXYZ.Domain.Entities;
using ReservasXYZ.Domain.Interfaces;

namespace ReservasXYZ.Application.Services;

public class RateService : IRateService
{
    private readonly IRateRepository _rateRepository;
    private readonly IMapper _mapper;

    public RateService(IRateRepository rateRepository, IMapper mapper)
    {
        _rateRepository = rateRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<RateDto>> GetAllAsync()
    {
        var items = await _rateRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<RateDto>>(items);
    }

    public async Task<IEnumerable<RateDto>> GetByRoomAsync(int roomId)
    {
        var items = await _rateRepository.GetRatesByRoomAsync(roomId);
        return _mapper.Map<IEnumerable<RateDto>>(items);
    }

    public async Task<RateDto?> GetByIdAsync(int id)
    {
        var item = await _rateRepository.GetByIdAsync(id);
        return item is null ? null : _mapper.Map<RateDto>(item);
    }

    public async Task<RateDto> CreateAsync(CreateRateDto dto)
    {
        var entity = _mapper.Map<Rate>(dto);
        await _rateRepository.AddAsync(entity);
        await _rateRepository.SaveChangesAsync();
        return _mapper.Map<RateDto>(entity);
    }

    public async Task UpdateAsync(UpdateRateDto dto)
    {
        var entity = await _rateRepository.GetByIdAsync(dto.Id)
            ?? throw new KeyNotFoundException($"No se encontró la tarifa #{dto.Id}.");
        _mapper.Map(dto, entity);
        _rateRepository.Update(entity);
        await _rateRepository.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _rateRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"No se encontró la tarifa #{id}.");
        entity.IsActive = false;
        _rateRepository.Update(entity);
        await _rateRepository.SaveChangesAsync();
    }

    public async Task<decimal> CalculateTotalAsync(int roomId, DateTime checkIn, DateTime checkOut, int totalGuests = 1)
    {
        return await _rateRepository.CalculateTotalRateAsync(roomId, checkIn, checkOut, totalGuests);
    }
}
