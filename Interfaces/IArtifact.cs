using Last_Api.Models;


namespace Interfaces
{
    public interface IArtifact
    {
        public List<Artifact> GetArtifacts();

        public Artifact GetArtifact(int id);

        public List<Artifact> GetByTribe(string tribe);

        public List<Artifact> GetByEra(int start, int end);

        public Artifact AddArtifact(Artifact artifact);

        public Artifact UpdateArtifact(Artifact artifact, int _id);

        public void DeleteArtifact(int id);

    }
}