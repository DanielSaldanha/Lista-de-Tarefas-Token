using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CloneListaTarefas.Data;
using CloneListaTarefas.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace lista_de_tarefas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Protege todos os métodos do controlador
    public class TasksController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMemoryCache _cache;
        public TasksController(IMemoryCache cache,AppDbContext context)
        {
            _context = context;
            _cache = cache;
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Tarefa tarefa)
        {
            if (tarefa == null)
            {
                return BadRequest("Dados inválidos.");
            }

            _context.lista.Add(tarefa);
            await _context.SaveChangesAsync();
            string key = "cacheTarefa";
            _cache.Remove(key);
            return CreatedAtAction(nameof(Post), new { id = tarefa.Id }, tarefa);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tarefa>>> Get()
        {
            string key = "cacheTarefa";
            if(!_cache.TryGetValue(key, out List<Tarefa> Vget))
            {
                Vget = await _context.lista.ToListAsync();
                var cacheoptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30),
                    SlidingExpiration = TimeSpan.FromMinutes(10)
                };
                _cache.Set(key, Vget, cacheoptions);
            }
            return Ok(Vget);


        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] Tarefa tarefa)
        {
            if (id != tarefa.Id)
            {
                return BadRequest("ID da tarefa não corresponde.");
            }
            if (tarefa.Id == 1)
            {
                return BadRequest("Você nao pode mudar a tarefa primordial.");
            }

            _context.Entry(tarefa).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            string key = "cacheTarefa";
            _cache.Remove(key);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var tarefa = await _context.lista.FindAsync(id);
            if (tarefa == null)
            {
                return NotFound("Tarefa não encontrada.");
            }
            if (tarefa.Id == 1)
            {
                return BadRequest("Você nao pode deletar a tarefa primordial.");
            }
            _context.lista.Remove(tarefa);
            await _context.SaveChangesAsync();
            string key = "cacheTarefa";
            _cache.Remove(key);
            return NoContent();
        }
    }
}
