using Core.Models.Presentations;

namespace Core.Services.Presentations;

public interface IPresentationConfigService
{
    public Task<PresentationConfig> Get(int artistId);
}
