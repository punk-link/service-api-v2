namespace PresentationGrpc.Services
{
    public interface IArtistPresentationService
    {
        public Task<Artist> Get(int id);
    }
}