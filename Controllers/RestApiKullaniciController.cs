using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using berber.Models;

namespace berber.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RestApiKullaniciController : ControllerBase
    {
        private readonly Context  c;

        public RestApiKullaniciController(Context context)
        {
            c = context;
        }

        // GET: api/RestApiKullanici
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Kullanici>>> GetKullanicilar()
        {
            return await c.Kullanicilar.ToListAsync();
        }
    }
}