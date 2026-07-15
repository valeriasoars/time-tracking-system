using SistemaPontos.Dto;
using SistemaPontos.models;

namespace SistemaPontos.Services.Interfaces
{
    public interface ISettingsService
    {
        Task<Settings?> GetSettings();
        Task<Settings> SaveSettings(SettingsDto dto);
    }
}
