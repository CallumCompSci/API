using Last_Api.Models;


namespace Interfaces
{
    public interface IArtEra
    {
        public List<ArtEra> GetArtEras();

        public ArtEra GetArtEra(string name);

        public List<ArtEra> GetByYears(int start, int end);

        public ArtEra AddArtEra(ArtEra era);

        public ArtEra UpdateArtEra(ArtEra era, string name);

        public void DeleteArtEra(string name);

    }
}