using Microsoft.EntityFrameworkCore;
using SistemaPontos.data;
using SistemaPontos.Dto;
using SistemaPontos.models;
using SistemaPontos.Services.Interfaces;

namespace SistemaPontos.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly AppDbContext _context;
        public SettingsService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Settings?> GetSettings()
        {
            var settings = await _context.Settings.FirstOrDefaultAsync();
            return settings;
        }

        public async Task<Settings> SaveSettings(SettingsDto dto)
        {
            var settingsExistente = await _context.Settings.FirstOrDefaultAsync();

            if (settingsExistente != null)
            {
                settingsExistente.WorkdayHours = dto.WorkdayHours;
                settingsExistente.OvertimeRate = dto.OvertimeRate;
            }
            else
            {
                settingsExistente = new Settings
                {
                    Id = Guid.NewGuid(),
                    WorkdayHours = dto.WorkdayHours,
                    OvertimeRate = dto.OvertimeRate,
                };

                _context.Settings.Add(settingsExistente);
 
            }
            await _context.SaveChangesAsync();
            return settingsExistente;
        }
    }
}
