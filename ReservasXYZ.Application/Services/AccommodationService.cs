using AutoMapper;
using ReservasXYZ.Application.DTOs;
using ReservasXYZ.Application.Interfaces;
using ReservasXYZ.Domain.Entities;
using ReservasXYZ.Domain.Interfaces;

namespace ReservasXYZ.Application.Services;

public class AccommodationService : IAccommodationService
{
    private readonly IRepository<Accommodation> _repository;
    private readonly IRepository<Site> _siteRepository;
    private readonly IMapper _mapper;

    public AccommodationService(IRepository<Accommodation> repository, IRepository<Site> siteRepository, IMapper mapper)
    {
        _repository = repository;
        _siteRepository = siteRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<AccommodationDto>> GetAllAsync()
    {
        var items = await _repository.GetAllAsync();
        var sites = (await _siteRepository.GetAllAsync()).ToDictionary(s => s.Id, s => s.Name);
        var dtos = _mapper.Map<List<AccommodationDto>>(items);

        foreach (var dto in dtos)
        {
            dto.SiteName = sites.TryGetValue(dto.SiteId, out var siteName) ? siteName : string.Empty;
        }

        return dtos;
    }

    public async Task<IEnumerable<AccommodationDto>> GetBySiteAsync(int siteId)
    {
        var items = await _repository.FindAsync(a => a.SiteId == siteId);
        return _mapper.Map<IEnumerable<AccommodationDto>>(items);
    }

    public async Task<AccommodationDto?> GetByIdAsync(int id)
    {
        var item = await _repository.GetByIdAsync(id);
        if (item is null) return null;

        var dto = _mapper.Map<AccommodationDto>(item);
        var site = await _siteRepository.GetByIdAsync(dto.SiteId);
        dto.SiteName = site?.Name ?? string.Empty;
        return dto;
    }

    public async Task<AccommodationDto> CreateAsync(CreateAccommodationDto dto)
    {
        var entity = _mapper.Map<Accommodation>(dto);
        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();
        return _mapper.Map<AccommodationDto>(entity);
    }

    public async Task UpdateAsync(UpdateAccommodationDto dto)
    {
        var entity = await _repository.GetByIdAsync(dto.Id)
            ?? throw new KeyNotFoundException($"No se encontró el alojamiento #{dto.Id}.");
        _mapper.Map(dto, entity);
        _repository.Update(entity);
        await _repository.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"No se encontró el alojamiento #{id}.");
        entity.IsActive = false;
        _repository.Update(entity);
        await _repository.SaveChangesAsync();
    }
}
