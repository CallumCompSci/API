using Last_Api.Models;


namespace Interfaces
{
    public interface ITribe
    {
        public List<Tribe> GetTribes();

        public Tribe GetTribe(string name);

        public List<Tribe> GetByRegion(string region);

        public Tribe AddTribe(Tribe tribe);

        public Tribe UpdateTribe(Tribe tribe, string name);

        public void DeleteTribe(string name);

    }
}