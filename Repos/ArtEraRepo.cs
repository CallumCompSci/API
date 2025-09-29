using System;
using Interfaces;
using Last_Api.Models;
using Npgsql;
using RepoDb;

namespace Last_Api.Repos;

public class ArtEraRepo : IArtEra
{
    private readonly IConfiguration _configuration;
    private readonly string ConnectionString;

    public ArtEraRepo(IConfiguration configuration)
    {
        ConnectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public List<ArtEra> GetArtEras()
    {

        List<ArtEra> era = new List<ArtEra>();
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var result = connection.QueryAll<ArtEra>("art_era");
            era = result.OrderBy(a => a.name).ToList();
        }
        
        return era;

    }

    public ArtEra GetArtEra(string _name)
    {
        ArtEra era = new ArtEra();
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            era = connection.Query<ArtEra>("art_era", e => e.name == _name).FirstOrDefault();
        }
        return era;
    }



    public List<ArtEra> GetByYears(int start, int end)
    {
        List<ArtEra> eras = new List<ArtEra>();
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var result = connection.QueryAll<ArtEra>("art_era");
            eras = result.Where(e => e.startyear < end && e.endyear > start).ToList();
        }

        return eras;
    }

    public ArtEra AddArtEra(ArtEra newEra)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            connection.Insert<ArtEra>(tableName: "art_era", entity: newEra);
        }
        return newEra;
    }

    public ArtEra UpdateArtEra(ArtEra updatedEra, string _name)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            connection.Update<ArtEra>(tableName: "art_era", entity: updatedEra, where: e => e.name == _name);
        }
        return updatedEra;
    }

    public void DeleteArtEra(string _name)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            //Delete uses Queryfields instead of lambda......
            connection.Delete(tableName: "art_era", where: new QueryField("name", _name));
        }
        
    }



}
