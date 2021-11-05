using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Marten;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using simple_martendb_api.Models;

namespace simple_martendb_api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MartenTestController : ControllerBase
    {
        private readonly ILogger<MartenTestController> _logger;
        private readonly IDocumentStore _documentStore;

        public MartenTestController(ILogger<MartenTestController> logger, IDocumentStore documentStore)
        {
            _logger = logger;
            _documentStore = documentStore;
        }

        [HttpPost("CreateTestPeople")]
        public async Task<IActionResult> Create(int how_many_records = 1)
        {
            // read more about session types https://martendb.io/documents/sessions.html
            var docStore = _documentStore.LightweightSession();

            var list = new List<Person>();
            for (int i = 0; i < how_many_records; i++)
            {                
                list.Add(createPerson()); createPerson();                
            }

            docStore.InsertObjects(list);
            await docStore.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("GetTop100")]
        public async Task<IActionResult> GetTop100()
        {
            await using var docStore = _documentStore.QuerySession();
            var items = await docStore.Query<Person>().Take(100).ToListAsync();

            return Ok(items);
        }

        [HttpGet("GetByName/{name}")]
        public async Task<IActionResult> GetByName(string name)
        {
            await using var docStore = _documentStore.QuerySession();
            var item = await docStore.Query<Person>()
                .FirstOrDefaultAsync(x => x.FullName == name);

            if (item == null)
            {
                return NotFound();
            }

            return Ok(item);
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            // Open a document session with the identity map
            await using (var session = _documentStore.OpenSession())
            {
                var item = await session.LoadAsync<Person>(id);

                if (item == null)
                {
                    return NotFound();
                }

                return Ok(item);
            }
        }

        private Person createPerson()
        {
            var person = new Person()
            {
                Id = Guid.NewGuid(),
                BirthDate = DateTime.Now.AddYears(32),
                Phone = "+98654213545",
                //FullName = GenerateName(12),
                Username = "admin",
                Accounts = new List<BankAccount>()
                {
                    new BankAccount(){ Id = Guid.NewGuid(), BankName =GenerateName(10) , Iban = "IR541242124731274167421"},
                    new BankAccount(){ Id = Guid.NewGuid(), BankName =GenerateName(10), Iban ="IR3423424112132423452452" }
                },
                Addresses = new List<Address>()
                {
                    new Address(){Id = Guid.NewGuid(), Street ="،Tehran, Valiasr St, Farid alley, 14 No" },
                    new Address(){Id = Guid.NewGuid(), Street = "Tehran, Sattarkhan St, Baghekhan St, 22 No"}
                }
            };

            return person;
        }

        public static string GenerateName(int len)
        {
            Random r = new Random();
            string[] consonants = { "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "l", "n", "p", "q", "r", "s", "sh", "zh", "t", "v", "w", "x" };
            string[] vowels = { "a", "e", "i", "o", "u", "ae", "y" };
            string Name = "";
            Name += consonants[r.Next(consonants.Length)].ToUpper();
            Name += vowels[r.Next(vowels.Length)];
            int b = 2; //b tells how many times a new letter has been added. It's 2 right now because the first two letters are already in the name.
            while (b < len)
            {
                Name += consonants[r.Next(consonants.Length)];
                b++;
                Name += vowels[r.Next(vowels.Length)];
                b++;
            }

            return Name;


        }
    }
}
