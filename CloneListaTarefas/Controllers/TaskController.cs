using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CloneListaTarefas.Data;
using CloneListaTarefas.Model;
using Microsoft.EntityFrameworkCore;

namespace lista_de_tarefas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Protege todos os métodos do controlador
    public class TasksController : ControllerBase
    {
        private readonly AppDbContext _context;
        public TasksController(AppDbContext context)
        {
            _context = context;
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
            return CreatedAtAction(nameof(Post), new { id = tarefa.Id }, tarefa);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tarefa>>> Get([FromQuery] int? id = null, string? tarefa = null)
        {
            if (!string.IsNullOrEmpty(tarefa))
            {
                var tarefasFiltradas = await _context.lista.Where(t => t.tarefa.Contains(tarefa)).ToListAsync();
                if (tarefasFiltradas.Count == 0)
                {
                    return NotFound("Nenhuma tarefa encontrada.");
                }
                return Ok(tarefasFiltradas);
            }
            else
            {
                var tarefas = await _context.lista.ToListAsync();
                return Ok(tarefas);
            }
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

            return NoContent();
        }
    }
}
