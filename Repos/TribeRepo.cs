using System;
using Interfaces;
using Last_Api.Models;

using Npgsql;
using RepoDb;

namespace Last_Api.Repos;

public class TribeRepo : ITribe
{

    private readonly IConfiguration _configuration;
    private readonly string ConnectionString;

    public TribeRepo(IConfiguration configuration)
    {
        ConnectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public List<Tribe> GetTribes()
    {

        List<Tribe> tribe = new List<Tribe>();
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var result = connection.QueryAll<Tribe>("tribes");
            tribe = result.OrderBy(a => a.name).ToList();
        }
        
        return tribe;

    }

    public Tribe GetTribe(string _name)
    {
        Tribe tribe = new Tribe();
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            tribe = connection.Query<Tribe>("tribes", e => e.name == _name).FirstOrDefault();
        }
        return tribe;
    }



    public List<Tribe> GetByRegion(string _region)
    {
        List<Tribe> tribes = new List<Tribe>();
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var result = connection.QueryAll<Tribe>("tribes");
            tribes = result.Where(e => e.region == _region).ToList();
        }

        return tribes;
    }

    public Tribe AddTribe(Tribe newTribe)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            connection.Insert<Tribe>(tableName: "tribes", entity: newTribe);
        }
        return newTribe;
    }

    public Tribe UpdateTribe(Tribe updatedTribe, string _name)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            connection.Update<Tribe>(tableName: "tribes", entity: updatedTribe, where: e => e.name == _name);
        }
        return updatedTribe;
    }

    public void DeleteTribe(string _name)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            //Delete uses Queryfields instead of lambda......
            connection.Delete(tableName: "tribes", where: new QueryField("name", _name));
        }
        
    }



}
