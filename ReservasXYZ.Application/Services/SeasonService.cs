using AutoMapper;
using ReservasXYZ.Application.DTOs;
using ReservasXYZ.Application.Interfaces;
using ReservasXYZ.Domain.Entities;
using ReservasXYZ.Domain.Interfaces;

namespace ReservasXYZ.Application.Services;

public class SeasonService : ISeasonService
{
    private readonly IRepository<Season> _repository;
    private readonly IMapper _mapper;

    public SeasonService(IRepository<Season> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<SeasonDto>> GetAllAsync()
    {
        var items = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<SeasonDto>>(items);
    }

    public async Task<SeasonDto?> GetByIdAsync(int id)
    {
        var item = await _repository.GetByIdAsync(id);
        return item is null ? null : _mapper.Map<SeasonDto>(item);
    }

    public async Task<SeasonDto> CreateAsync(CreateSeasonDto dto)
    {
        var entity = _mapper.Map<Season>(dto);
        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();
        return _mapper.Map<SeasonDto>(entity);
    }

    public async Task UpdateAsync(UpdateSeasonDto dto)
    {
        var entity = await _repository.GetByIdAsync(dto.Id)
            ?? throw new KeyNotFoundException($"No se encontró la temporada #{dto.Id}.");
        _mapper.Map(dto, entity);
        _repository.Update(entity);
        await _repository.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"No se encontró la temporada #{id}.");
        entity.IsActive = false;
        _repository.Update(entity);
        await _repository.SaveChangesAsync();
    }
}
