using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class ItemsController : ControllerBase
{
    private readonly GenericRepository<Item> _repository;

    public ItemsController(GenericRepository<Item> repository)
    {
        _repository = repository;
    }

    [HttpPost]
    public async Task<IActionResult> Upsert([FromBody] Item item)
    {
        await _repository.UpsertAsync(item, "Items", "ItemId");
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _repository.DeleteAsync(id, "Items", "ItemId");
        return Ok();
    }
}
