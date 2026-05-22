using AutoMapper;
using ReservasXYZ.Application.DTOs;
using ReservasXYZ.Application.Interfaces;
using ReservasXYZ.Domain.Entities;
using ReservasXYZ.Domain.Interfaces;

namespace ReservasXYZ.Application.Services;

public class SiteService : ISiteService
{
    private readonly IRepository<Site> _repository;
    private readonly IRepository<Accommodation> _accommodationRepository;
    private readonly IMapper _mapper;

    public SiteService(IRepository<Site> repository, IRepository<Accommodation> accommodationRepository, IMapper mapper)
    {
        _repository = repository;
        _accommodationRepository = accommodationRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<SiteDto>> GetAllAsync()
    {
        var sites = await _repository.GetAllAsync();
        var counts = (await _accommodationRepository.GetAllAsync())
            .GroupBy(a => a.SiteId)
            .ToDictionary(g => g.Key, g => g.Count());

        var dtos = _mapper.Map<List<SiteDto>>(sites);
        foreach (var dto in dtos)
        {
            dto.AccommodationCount = counts.TryGetValue(dto.Id, out var count) ? count : 0;
        }
        return dtos;
    }

    public async Task<SiteDto?> GetByIdAsync(int id)
    {
        var site = await _repository.GetByIdAsync(id);
        if (site is null) return null;
        var dto = _mapper.Map<SiteDto>(site);
        dto.AccommodationCount = (await _accommodationRepository.GetAllAsync()).Count(a => a.SiteId == id);
        return dto;
    }

    public async Task<SiteDto> CreateAsync(CreateSiteDto dto)
    {
        var site = _mapper.Map<Site>(dto);
        await _repository.AddAsync(site);
        await _repository.SaveChangesAsync();
        return _mapper.Map<SiteDto>(site);
    }

    public async Task UpdateAsync(UpdateSiteDto dto)
    {
        var site = await _repository.GetByIdAsync(dto.Id)
            ?? throw new KeyNotFoundException($"No se encontró la sede #{dto.Id}.");
        _mapper.Map(dto, site);
        _repository.Update(site);
        await _repository.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var site = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"No se encontró la sede #{id}.");
        site.IsActive = false;
        _repository.Update(site);
        await _repository.SaveChangesAsync();
    }
}
