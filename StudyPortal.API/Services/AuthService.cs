using Microsoft.Extensions.Options;
using StudyPortal.API.Configs;

namespace StudyPortal.API.Services;

public class AuthService: AbstractService
{
    public AuthService(IOptions<StudyPortalDatabaseSettings> settings) : base(settings)
    {
    }
}