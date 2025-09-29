using Microsoft.AspNetCore.Mvc;
using Interfaces;
using Last_Api.Models;
using Microsoft.AspNetCore.Authorization;
namespace Last_Api.Contollers
{
    [ApiController]
    [Route("api/tribe")]
    [Tags("Tribe Endpoints:")]
    public class TribeContoller : ControllerBase
    {
        private readonly ITribe _TribeRepo;

        public TribeContoller(ITribe TribeRepo)
        {
            _TribeRepo = TribeRepo;
        }


        [Authorize(Policy = "User")]
        [HttpGet()]
        [EndpointName("Get Tribes")]
        [EndpointSummary("Returns all Tribes")]
        [EndpointDescription("Returns a list containing objects of Tribe type")]
        [ProducesResponseType(typeof(ActionResult<IEnumerable<Tribe>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<IEnumerable<Tribe>> GetTribes()
        {
            return Ok(_TribeRepo.GetTribes());
        }

        [Authorize(Policy = "User")]
        [HttpGet("{name}")]
        [EndpointName("Get Tribe")]
        [EndpointSummary("Returns a Tribe")]
        [EndpointDescription("Returns a Tribe based on name specified. Example request: /api/tribe/Wonnarua")]
        [ProducesResponseType(typeof(ActionResult<IEnumerable<Tribe>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<Tribe> GetTribe(string name)
        {
            var tribe = _TribeRepo.GetTribe(name);
            if (tribe == null)
            {
                return NotFound($"No tribe with name: {name} found");
            }
            return Ok(tribe);
        }

        [Authorize(Policy = "User")]
        [HttpGet("region/{region}")]
        [EndpointName("Get tribes by region")]
        [EndpointSummary("Returns tribes by region")]
        [EndpointDescription("Returns a list of tribes based on region specified. Example request: api/tribe/region/Central Coast")]
        [ProducesResponseType(typeof(ActionResult<IEnumerable<Artifact>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<IEnumerable<Tribe>> GetByRegion(string region)
        {

            List<Tribe> tribes = new List<Tribe>();
            tribes = _TribeRepo.GetByRegion(region);
            if (tribes.Count == 0)
            {
                return NotFound($"No tribes within the region: {region} found");
            }
            return Ok(tribes);
        }

        [Authorize(Policy = "Staff")]
        [HttpPost()]
        [EndpointName("Add Tribe")]
        [EndpointSummary("Insert Tribe")]
        [EndpointDescription(@"Authorization limited to Staff and Admins. Adds new Tribe to the database.
        Body should look like:
        {
        ""name"": ""NAME"",
        ""description"": ""DESCRIPTION"",
        ""latitude"": ""32.5644"",
        ""longitude"": ""151.3552""
        ""language"": ""LANGUAGE"",
        ""REGION"": ""LANGUAGE""
        }
        ")]
        [ProducesResponseType(typeof(ActionResult<IEnumerable<Tribe>>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<Tribe> AddTribe(Tribe newTribe)
        {
            if (newTribe.name == null) { return BadRequest("Name cannot be null"); }
            if (newTribe.region == null) { return BadRequest("Region cannot be null"); }

            var result = _TribeRepo.AddTribe(newTribe);
            return Created();
        }



        [Authorize(Policy = "Staff")]
        [HttpPut("{name}")]
        [EndpointName("Update Tribe")]
        [EndpointSummary("Update Tribe")]
        [EndpointDescription(@"Authorization limited to Staff and Admins. Updates existing Tribe.
        Body should look like:
        {
        ""name"": ""NAME"",
        ""description"": ""DESCRIPTION"",
        ""latitude"": ""32.5644"",
        ""longitude"": ""151.3552""
        ""language"": ""LANGUAGE"",
        ""REGION"": ""LANGUAGE""
        }
        ")]
        [ProducesResponseType(typeof(ActionResult<IEnumerable<Tribe>>), StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<Tribe> UpdateTribe(Tribe updatedTribe, string name)
        {
            var _tribe = _TribeRepo.GetTribe(name);
            if (_tribe == null) { return NotFound($"No tribe with name '{name}' found"); }

            if (updatedTribe.name == null) { return BadRequest("Name cannot be null"); }
            if (updatedTribe.region == null) { return BadRequest("Region cannot be null"); }

            var result = _TribeRepo.UpdateTribe(updatedTribe, name);
            return NoContent();
        }

        [Authorize(Policy = "Staff")]
        [HttpDelete("{name}")]
        [EndpointName("Deletes Tribe")]
        [EndpointSummary("Deletes Tribe from database")]
        [EndpointDescription(@"Authorization limited to Staff and Admins. Deletes Tribe.
        Example Request: /api/tribe/tribeName
        ")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult DeleteTribe(string name)
        {
            var tribe = _TribeRepo.GetTribe(name);
            if (tribe == null) { return NotFound("No tribe with that name found"); }
            _TribeRepo.DeleteTribe(name);
            return NoContent();
        }


    }
}
