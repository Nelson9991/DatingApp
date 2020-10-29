using System.Collections.Generic;
using System.Threading.Tasks;
using API.Data;
using API.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class CustomBaseController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public CustomBaseController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        protected async Task<List<TDto>> GetResourceList<TEntidad, TDto>() where TEntidad : class
        {
            var entidades = await context.Set<TEntidad>().AsNoTracking().ToListAsync();

            var dtos = mapper.Map<List<TDto>>(entidades);

            return dtos;
        }

        public async Task<ActionResult<TDto>> GetResource<TEntidad, TDto>(int id) where TEntidad : class, IId
        {
            var entidad = await context.Set<TEntidad>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

            if (entidad == null)
            {
                return NotFound();
            }

            return mapper.Map<TDto>(entidad);
        }

        protected async Task<ActionResult> PostResource<TCreacion, TEntidad, TLectura>
        (TCreacion creacionDto, string nombreRuta) where TEntidad : class, IId
        {
            var entidad = mapper.Map<TEntidad>(creacionDto);

            context.Add((object) entidad);

            await context.SaveChangesAsync();

            var dtoLectura = mapper.Map<TLectura>(entidad);

            return new CreatedAtRouteResult(nombreRuta, new { id = entidad.Id }, dtoLectura);
        }

        public async Task<ActionResult> PutResource<TCreacion, TEntidad>(int id, TCreacion updateDto)
        where TEntidad : class, IId
        {
            var entidad = await context.Set<TEntidad>().FindAsync(id);

            if (entidad == null)
            {
                return NotFound();
            }

            mapper.Map(updateDto, entidad);

            await context.SaveChangesAsync();

            return NoContent();
        }

        public async Task<ActionResult> DeleteResource<TEntidad>(int id) where TEntidad : class, IId, new()
        {
            var existe = await context.Set<TEntidad>().AnyAsync(x => x.Id == id);

            if (!existe)
            {
                return NotFound();
            }

            context.Remove(new TEntidad() { Id = id });
            await context.SaveChangesAsync();

            return NoContent();
        }
        
    }
}