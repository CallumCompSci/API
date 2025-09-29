using Microsoft.AspNetCore.Mvc;
using Interfaces;
using Last_Api.Models;
using Microsoft.AspNetCore.Authorization;
namespace Last_Api.Contollers
{
    [ApiController]
    [Route("api/artifacts")]
    [Tags("Artifact Endpoints:")]
    public class ArtifactContoller : ControllerBase
    {
        private readonly IArtifact _ArtifactRepo;

        public ArtifactContoller(IArtifact ArtifactRepo)
        {
            _ArtifactRepo = ArtifactRepo;
        }


        [Authorize(Policy = "User")]
        [HttpGet()]
        [EndpointName("Get Artifacts")]
        [EndpointSummary("Returns all Artifacts")]
        [EndpointDescription("Returns a list containing objects of Artifact type")]
        [ProducesResponseType(typeof(ActionResult<IEnumerable<Artifact>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<IEnumerable<Artifact>> GetArtifacts()
        {
            return Ok(_ArtifactRepo.GetArtifacts());
        }

        [Authorize(Policy = "User")]
        [HttpGet("{id}")]
        [EndpointName("Get Artifact")]
        [EndpointSummary("Returns Artifact")]
        [EndpointDescription("Returns an objects of Artifact type. Example request: api/artifacts/2 ")]
        [ProducesResponseType(typeof(ActionResult<IEnumerable<Artifact>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<Artifact> GetArtifact(int id)
        {
            if (id < 0)
            {
                return BadRequest("ID must be above 1");
            }
            var artifact = _ArtifactRepo.GetArtifact(id);
            if (artifact == null)
            {
                return NotFound($"No artifact with ID: {id} found");
            }
            return Ok(artifact);
        }


        [Authorize(Policy = "User")]
        [HttpGet("tribe/{name}")]
        [EndpointName("Get Artifact by tribe")]
        [EndpointSummary("Returns Artifact based on tribes")]
        [EndpointDescription("Returns an objects of Artifact type that is linked to tribe specified. Example request: api/artifacts/tribe/tribename ")]
        [ProducesResponseType(typeof(ActionResult<IEnumerable<Artifact>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<IEnumerable<Artifact>> GetByTribe(string name)
        {
            List<Artifact> artifacts = new List<Artifact>();
            artifacts = _ArtifactRepo.GetByTribe(name);
            if (artifacts.Count == 0)
            {
                return NotFound($"No artifacts belonging to tribe: {name} found");
            }
            return Ok(artifacts);
        }

        [Authorize(Policy = "User")]
        [HttpGet("{startYear}/{endYear}")]
        [EndpointName("Get Artifact by era")]
        [EndpointSummary("Returns Artifact based on the time period it was created")]
        [EndpointDescription("Returns an objects of Artifact type that is linked to a specific time period. Example request: api/artifacts/200/804 ")]
        [ProducesResponseType(typeof(ActionResult<IEnumerable<Artifact>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<IEnumerable<Artifact>> GetByEra(int startYear, int endYear)
        {
            if (startYear < 0 || endYear < 0 || startYear > DateTime.Now.Year || endYear > DateTime.Now.Year)
            {
                return BadRequest("Years must be above 0 and lower than the current year");
            }
            if (startYear > endYear) { return BadRequest("Start year must be lower than End year"); }

            List<Artifact> artifacts = new List<Artifact>();
            artifacts = _ArtifactRepo.GetByEra(startYear, endYear);
            if (artifacts.Count == 0)
            {
                return NotFound($"No artifacts within the time period of: {startYear} - {endYear} found");
            }
            return Ok(artifacts);
        }



        [Authorize(Policy = "Staff")]
        [HttpPost()]
        [EndpointName("Add Artifact")]
        [EndpointSummary("Insert Artifact")]
        [EndpointDescription(@"Authorization limited to Staff and Admins. Adds new Artifact to the database.
        Body should look like:
        {
        ""name"": ""NAME"",
        ""description"": ""DESCRIPTION"",
        ""latitude"": ""32.5644"",
        ""longitude"": ""151.3552""
        ""startyear"": 200,
        ""endyear"": 440
        }
        ")]
        [ProducesResponseType(typeof(ActionResult<IEnumerable<Artifact>>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<Artifact> AddArtifact(Artifact artifact)
        {
            if (artifact.name == null) { return BadRequest("Name cannot be null"); }
            if (artifact.startyear > DateTime.Now.Year || artifact.endyear > DateTime.Now.Year) { return BadRequest("start and end year cannot be higher than current year"); }

            var result = _ArtifactRepo.AddArtifact(artifact);
            return Created();
        }

        [Authorize(Policy = "Staff")]
        [HttpPut("{id}")]
        [EndpointName("Update Artifact")]
        [EndpointSummary("Update Artifact")]
        [EndpointDescription(@"Authorization limited to Staff and Admins. Updates existing Artifact.
        Body should look like:
        {
        ""name"": ""NAME"",
        ""description"": ""DESCRIPTION"",
        ""latitude"": ""32.5644"",
        ""longitude"": ""151.3552""
        ""startyear"": 200,
        ""endyear"": 440
        }
        ")]
        [ProducesResponseType(typeof(ActionResult<IEnumerable<Artifact>>), StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<Artifact> UpdateArtifact(Artifact artifact, int id)
        {
            if (id < 0) { return BadRequest("ID cannot be lower than 0"); }
            var _artifact = _ArtifactRepo.GetArtifact(id);
            if (_artifact == null) { return NotFound("No artifact with that ID found"); }

            if (artifact.name == null) { return BadRequest("Name cannot be null"); }
            if (artifact.startyear > DateTime.Now.Year || artifact.endyear > DateTime.Now.Year) { return BadRequest("start and end year cannot be higher than current year"); }

            var result = _ArtifactRepo.UpdateArtifact(artifact, id);
            return NoContent();
        }

        [Authorize(Policy = "Staff")]
        [HttpDelete("{id}")]
        [EndpointName("Deletes Artifact")]
        [EndpointSummary("Deletes artifact from database")]
        [EndpointDescription(@"Authorization limited to Staff and Admins. Deletes Artifact.
        Example Request: /api/artifacts/7
        ")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult DeleteArtifact(int id)
        {
            if (id < 0) { return BadRequest("ID cannot be lower than 0"); }
            var artifact = _ArtifactRepo.GetArtifact(id);
            if (artifact == null) { return NotFound("No artifact with that ID found"); }
            _ArtifactRepo.DeleteArtifact(id);
            return NoContent();
        }


    }
}
