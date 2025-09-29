using System;
using Interfaces;
using Last_Api.Models;

using Npgsql;
using RepoDb;

namespace Last_Api.Repos;

public class ArtifactRepo : IArtifact
{

    private readonly IConfiguration _configuration;
    private readonly string ConnectionString;

    public ArtifactRepo(IConfiguration configuration)
    {
        ConnectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public List<Artifact> GetArtifacts()
    {

        List<Artifact> artifacts = new List<Artifact>();
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var result = connection.QueryAll<Artifact>("artifacts");
            artifacts = result.OrderBy(a => a.id).ToList();
        }
        
        return artifacts;

    }

    public Artifact GetArtifact(int _id)
    {
        var artifact = new Artifact();
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            artifact = connection.Query<Artifact>("artifacts", e => e.id == _id).FirstOrDefault();
        }
        return artifact;
    }

    public List<Artifact> GetByTribe(string name)
    {
        List<Artifact> artifacts = new List<Artifact>();
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var result = connection.QueryAll<Artifact>("artifacts");
            var filteredResult = result.Where(e => e.tribe == name).ToList();
            artifacts = filteredResult.ToList();
        }

        return artifacts;
    }

    public List<Artifact> GetByEra(int start, int end)
    {
        List<Artifact> artifacts = new List<Artifact>();
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var result = connection.QueryAll<Artifact>("artifacts");
            artifacts = result.Where(e => e.startyear < end && e.endyear > start).ToList();
        }

        return artifacts;
    }

    public Artifact AddArtifact(Artifact newArtifact)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            connection.Insert<Artifact>(tableName: "artifacts", entity: newArtifact);
        }
        return newArtifact;
    }

    public Artifact UpdateArtifact(Artifact updatedArtifact, int _id)
    {
        Console.WriteLine("Hello");
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            updatedArtifact.id = _id;
            connection.Update<Artifact>(tableName: "artifacts", entity: updatedArtifact);
        }
        return updatedArtifact;
    }

    public void DeleteArtifact(int _id)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            connection.Delete("artifacts", _id);
        }
        
    }



}
