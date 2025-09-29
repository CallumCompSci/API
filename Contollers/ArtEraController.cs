using Microsoft.AspNetCore.Mvc;
using Interfaces;
using Last_Api.Models;
using Microsoft.AspNetCore.Authorization;
namespace Last_Api.Contollers
{
    [ApiController]
    [Route("api/arteras")]
    [Tags("Art Era Endpoints:")]
    public class ArtErasContoller : ControllerBase
    {
        private readonly IArtEra _ArtEraRepo;

        public ArtErasContoller(IArtEra ArtEraRepo)
        {
            _ArtEraRepo = ArtEraRepo;
        }


        [Authorize(Policy = "User")]
        [HttpGet()]
        [EndpointName("Get Art Eras")]
        [EndpointSummary("Returns all Art Eras")]
        [EndpointDescription("Returns a list containing objects of Art Era type")]
        [ProducesResponseType(typeof(ActionResult<IEnumerable<ArtEra>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<IEnumerable<ArtEra>> GetArtEras()
        {
            return Ok(_ArtEraRepo.GetArtEras());
        }



        [Authorize(Policy = "User")]
        [HttpGet("{name}")]
        [EndpointName("Get Art Era")]
        [EndpointSummary("Returns one Art Era")]
        [EndpointDescription("Returns an object of Art Era type that matches the name given. Example request: /api/arteras/Contact Art")]
        [ProducesResponseType(typeof(ActionResult<ArtEra>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<ArtEra> GetArtEra(string name)
        {
            var artera = _ArtEraRepo.GetArtEra(name);
            if (artera == null)
            {
                return NotFound($"No Art Era with name: {name} found");
            }
            return Ok(artera);
        }

        [Authorize(Policy = "User")]
        [HttpGet("{startYear}/{endYear}")]
        [EndpointName("Get Era by years")]
        [EndpointSummary("Returns list of art eras")]
        [EndpointDescription("Returns list of Art Eras that fall between 2 years. Start year and End year must be specified. Example Request: api/arteras/1002/1100")]
        [ProducesResponseType(typeof(ActionResult<IEnumerable<ArtEra>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<IEnumerable<ArtEra>> GetByYear(int startYear, int endYear)
        {
            if (startYear < 0 || endYear < 0 || startYear > DateTime.Now.Year || endYear > DateTime.Now.Year)
            {
                return BadRequest("Years must be above 0 and lower than the current year");
            }
            if (startYear > endYear) { return BadRequest("Start year must be lower than End year"); }

            List<ArtEra> eras = new List<ArtEra>();
            eras = _ArtEraRepo.GetByYears(startYear, endYear);
            if (eras.Count == 0)
            {
                return NotFound($"No era within the time period of: {startYear} - {endYear} found");
            }
            return Ok(eras);
        }

        [Authorize(Policy = "Staff")]
        [HttpPost()]
        [EndpointName("Add Era")]
        [EndpointSummary("Inserts Era into Database")]
        [EndpointDescription(@"Authorization limited to Staff and Admins. Adds new Art Era.
        Body should look like:
        {
        ""name"": ""NAME"",
        ""description"": ""DESCRIPTION"",
        ""startyear"": 2,
        ""endyear"": 44
        }
        ")]
        [ProducesResponseType(typeof(ActionResult<IEnumerable<ArtEra>>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<ArtEra> AddArtEra(ArtEra NewEra)
        {
            if (NewEra.name == null) { return BadRequest("Name cannot be null"); }
            if (NewEra.startyear > DateTime.Now.Year || NewEra.endyear > DateTime.Now.Year) { return BadRequest("start and end year cannot be higher than current year"); }

            var result = _ArtEraRepo.AddArtEra(NewEra);
            return Created();
        }

        [Authorize(Policy = "Staff")]
        [HttpPut("{name}")]
        [EndpointName("Update Art Era")]
        [EndpointSummary("Updates art era")]
        [EndpointDescription(@"Authorization limited to Staff and Admins. Updates Art Era.
        Body should look like:
        {
        ""name"": ""UPDATED NAME"",
        ""description"": ""UPDATED DESCRIPTION"",
        ""startyear"": 2,
        ""endyear"": 44
        }
        ")]
        [ProducesResponseType(typeof(ActionResult<IEnumerable<ArtEra>>), StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<ArtEra> UpdateArtEra(ArtEra updatedEra, string name)
        {
            var _artEra = _ArtEraRepo.GetArtEra(name);
            if (_artEra == null) { return NotFound($"No era with name '{name}' found"); }

            if (updatedEra.name == null) { return BadRequest("Name cannot be null"); }
            if (updatedEra.startyear > DateTime.Now.Year || updatedEra.endyear > DateTime.Now.Year) { return BadRequest("start and end year cannot be higher than current year"); }

            var result = _ArtEraRepo.UpdateArtEra(updatedEra, name);
            return NoContent();
        }

        [Authorize(Policy = "Staff")]
        [HttpDelete("{name}")]
        [EndpointName("Deletes Art Era")]
        [EndpointSummary("Deletes art era from database")]
        [EndpointDescription(@"Authorization limited to Staff and Admins. Deletes Art Era.
        Example Request: /api/arteras/EraName
        ")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult DeleteArtEra(string name)
        {
            var era = _ArtEraRepo.GetArtEra(name);
            if (era == null) { return NotFound("No era with that name found"); }
            _ArtEraRepo.DeleteArtEra(name);
            return NoContent();
        }


    }
}
