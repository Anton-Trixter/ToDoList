using System.Globalization;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoList.Db;
using ToDoList.Dto;
using Task = ToDoList.Db.Task;

namespace ToDoList.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController: ControllerBase
{
    private readonly ToDoListContext _context;
    private readonly IMapper _mapper;

    public TasksController(ToDoListContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> Read()
    {
        var tasks = await _context.Tasks.ToListAsync();
        return Ok(tasks);
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> Create(string name, TaskState status)
    {
        TextInfo myTI = CultureInfo.CurrentCulture.TextInfo;
        
        var task = new Task()
        {
            Name = myTI.ToTitleCase(name),
            State = status
        };

        var entity = await _context.Tasks.FirstOrDefaultAsync(x => x.Name == name);
        if (entity != null)
        {
            return BadRequest("The task with this name is already exist!");
        }
        
        await _context.Tasks.AddAsync(task);
        if (await _context.SaveChangesAsync() == 1)
        {
            return Ok("The task is added.");    
        }

        return StatusCode(StatusCodes.Status500InternalServerError,"The task is not added");
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> Delete(string name)
    {
        var task = await _context.Tasks.FirstOrDefaultAsync(x => x.Name == name);
        if (task == null)
        {
            return BadRequest("The task with this name does not exist!");
        }

        await System.Threading.Tasks.Task.Run(() =>
        {
            _context.Tasks.Remove(task);
        });
        await _context.SaveChangesAsync();

        return Ok("The task has been deleted.");
        
        // 2 метода (изменение статуса + переименование задачи)
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> Update(UpdateTaskDto input)
    {
        var task = _mapper.Map<Task>(input);
        _context.Tasks.Update(task);
        
        await _context.SaveChangesAsync();

        return Ok();
    }
}