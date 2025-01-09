using System.Text.Json;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Random_User_Generator.Data;
using Random_User_Generator.Models;
using Random_User_Generator.ViewModels;

namespace Random_User_Generator.Controllers;

[ApiController]
[Route("v1/users")]
public class UserController : ControllerBase
{
    //GetAll
    [HttpGet("skip/{skip:int}/take/{take:int}")]
    public async Task<IActionResult> GetAllAsync([FromServices] RandonUserGeneratorDataContext context,[FromQuery] string? name ,[FromRoute] int skip = 0, [FromRoute] int take = 25)
    {
        try
        {
            var vm = new List<VisualizarUserVM>();
            
            var users = new List<User>();
            
            if (name != null)
            {
                users = await context.Users.Where(x => x.Name.ToLower().Contains(name.ToLower())).Skip(skip).Take(take).OrderBy(x => x.Id).ToListAsync();
            }
            else
            { 
                users = await context.Users.Skip(skip).Take(take).OrderBy(x => x.Id).ToListAsync();
            }

            if (!users.Any())
                return NotFound(new { message = "Nenhum usuário cadastrado" });
            
            var total = users.Count();
            
            foreach (var i in users)
            {
                var user = new VisualizarUserVM()
                {
                    Id = i.Id,
                    Gender = i.Gender,
                    Name = i.Name,
                    Email = i.Email
                };

                vm.Add(user);
            }
            
            var retornoUsers = new ResultViewModel<List<VisualizarUserVM>>(vm);

            return Ok(new
            {
                retornoUsers,
                total
            });
        }
                
        catch (Exception)
        {
            return StatusCode(500, new ResultViewModel<VisualizarUserVM>("01x01 - Falha interna no servidor"));
        }
    }

    //GetById
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetAsync([FromServices] RandonUserGeneratorDataContext context, [FromRoute] int id)
    {
        try
        {
            var vm = new VisualizarUserVM();
            var user = await context.Users.FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
                return NotFound(new { message = "Nenhum usuário encontrado" });

            vm.Id = user.Id;
            vm.Gender = user.Gender;
            vm.Name = user.Name;
            vm.Email = user.Email;

            return Ok(new ResultViewModel<VisualizarUserVM>(vm));
        }
        catch (Exception)
        {
            return StatusCode(500, new ResultViewModel<VisualizarUserVM>("01x02- Falha interna no servidor"));
        }
    }

    //Post não random
    [HttpPost("")]
    public async Task<IActionResult> PostAsync([FromServices] RandonUserGeneratorDataContext context,
        [FromBody] CadastrarUserVM vm)
    {
        try
        {
            var userCadastrado = await context.Users.FirstOrDefaultAsync(x => x.Email == vm.Email);

            if (userCadastrado != null)
                return BadRequest(new { message = "Usuário já cadastrado" });

            var userNovo = new User()
            {
                Gender = vm.Gender,
                Name = vm.Name,
                Email = vm.Email,
            };

            await context.Users.AddAsync(userNovo);
            await context.SaveChangesAsync();

            return Created($"v1/users/{userNovo.Id}", new ResultViewModel<User>(userNovo));
        }
        catch (Exception)
        {
            return StatusCode(500, new ResultViewModel<VisualizarUserVM>("01x03- Falha interna no servidor"));
        }
    }

    //Editar usuário pelo id
    [HttpPut("{id:int}")]
    public async Task<IActionResult> PutAsync([FromServices] RandonUserGeneratorDataContext context, [FromRoute] int id,
        [FromBody] EditarUserVM vm)
    {
        try
        {
            var user = await context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
                return NotFound(new ResultViewModel<User>("Usuário não encontrado"));

            user.Gender = vm.Gender;
            user.Name = vm.Name;
            user.Email = vm.Email;

            context.Users.Update(user);
            await context.SaveChangesAsync();

            return Ok(new ResultViewModel<EditarUserVM>(vm));
        }
        catch (DbUpdateException)
        {
            return StatusCode(500, new ResultViewModel<User>("01x04 - Não foi possivel alterar o usuário"));
        }
        catch (Exception)
        {
            return StatusCode(500, new ResultViewModel<User>("01x05 - Falha interna no servidor"));
        }
    }

    //Deletar usuário
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAsync([FromServices] RandonUserGeneratorDataContext context,
        [FromRoute] int id)
    {
        try
        {
            var user = await context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
                return NotFound(new ResultViewModel<User>("Cliente não localizado"));

            context.Users.Remove(user);

            await context.SaveChangesAsync();

            var vm = new VisualizarUserVM()
            {
                Id = user.Id,
                Gender = user.Gender,
                Name = user.Name,
                Email = user.Email
            };
            return Ok(new ResultViewModel<VisualizarUserVM>(vm));
        }
        catch (DbUpdateException)
        {
            return StatusCode(500, new ResultViewModel<User>("01x06 - Não voi possivel excluir o cliente"));
        }
        catch (Exception)
        {
            return StatusCode(500, new ResultViewModel<User>("01x07 - Falha interna no servidor"));
        }
    }

    //Post Random (consumir api Random User Generator e inserir no banco)
    [HttpPost("/random")]
    public async Task<IActionResult> PostRandom([FromServices] RandonUserGeneratorDataContext context)
    {
        using var httpClient = new HttpClient();
        string endpoint = "https://randomuser.me/api/?results=1";

        try
        {
            HttpResponseMessage response = await httpClient.GetAsync(endpoint);

            if (response.IsSuccessStatusCode)
            {
                string retornoApi = await response.Content.ReadAsStringAsync();

                var json = JsonDocument.Parse(retornoApi);

                var result = json.RootElement.GetProperty("results").EnumerateArray().First();

                if (result.GetProperty("name").GetProperty("first").GetString() == "" || result.GetProperty("name").GetProperty("first").GetString() == null || 
                    result.GetProperty("gender").GetString() == "" || result.GetProperty("gender").GetString() == null || 
                    result.GetProperty("email").GetString() == "" || result.GetProperty("email").GetString() == null )
                {
                    return StatusCode(500, new ResultViewModel<User>("01x08 - Falha ao consultar api Random User Generator"));
                }

                var userNovo = new User
                {
                    Gender = result.GetProperty("gender").GetString()!,
                    Name = result.GetProperty("name").GetProperty("first").GetString() + " " +
                           result.GetProperty("name").GetProperty("last").GetString(),
                    Email = result.GetProperty("email").GetString()!
                };

                await context.Users.AddAsync(userNovo);

                await context.SaveChangesAsync();

                return Created($"v1/users/{userNovo.Id}", new ResultViewModel<User>(userNovo));
            }
            else
            {
                return StatusCode(500, new ResultViewModel<User>("01x09 - Falha interna no servidor"));
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ResultViewModel<User>("01x10 - Falha interna no servidor"));
        }
    }
}