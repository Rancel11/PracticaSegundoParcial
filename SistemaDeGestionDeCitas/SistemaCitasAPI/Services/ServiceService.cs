using Microsoft.EntityFrameworkCore;
using SistemaCitasAPI.Data;
using SistemaCitasAPI.DTOs;
using SistemaCitasAPI.Models;

namespace SistemaCitasAPI.Services
{
    public class ServiceService : IServiceService
    {
        private readonly AppDbContext _context;

        public ServiceService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ServiceDTO>> GetAllServicesAsync()
        {
            return await _context.Services
                .Include(s => s.Category)
                .Where(s => s.IsActive)
                .Select(s => new ServiceDTO
                {
                    ServiceId = s.ServiceId,
                    ServiceName = s.ServiceName,
                    Description = s.Description,
                    DurationMinutes = s.DurationMinutes,
                    Price = s.Price,
                    CategoryName = s.Category.CategoryName,
                    IsActive = s.IsActive
                })
                .ToListAsync();
        }

        public async Task<ServiceDTO?> GetServiceByIdAsync(int serviceId)
        {
            var service = await _context.Services
                .Include(s => s.Category)
                .FirstOrDefaultAsync(s => s.ServiceId == serviceId && s.IsActive);

            if (service == null)
                return null;

            return new ServiceDTO
            {
                ServiceId = service.ServiceId,
                ServiceName = service.ServiceName,
                Description = service.Description,
                DurationMinutes = service.DurationMinutes,
                Price = service.Price,
                CategoryName = service.Category.CategoryName,
                IsActive = service.IsActive
            };
        }

        public async Task<ServiceDTO?> CreateServiceAsync(CreateServiceDTO dto)
        {
            var category = await _context.ServiceCategories.FindAsync(dto.CategoryId);
            if (category == null)
                return null;

            var service = new Service
            {
                ServiceName = dto.ServiceName,
                Description = dto.Description,
                DurationMinutes = dto.DurationMinutes,
                Price = dto.Price,
                CategoryId = dto.CategoryId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Services.Add(service);
            await _context.SaveChangesAsync();

            return new ServiceDTO
            {
                ServiceId = service.ServiceId,
                ServiceName = service.ServiceName,
                Description = service.Description,
                DurationMinutes = service.DurationMinutes,
                Price = service.Price,
                CategoryName = category.CategoryName,
                IsActive = service.IsActive
            };
        }

        public async Task<ServiceDTO?> UpdateServiceAsync(int serviceId, CreateServiceDTO dto)
        {
            var service = await _context.Services
                .Include(s => s.Category)
                .FirstOrDefaultAsync(s => s.ServiceId == serviceId);

            if (service == null)
                return null;

            service.ServiceName = dto.ServiceName;
            service.Description = dto.Description;
            service.DurationMinutes = dto.DurationMinutes;
            service.Price = dto.Price;
            service.CategoryId = dto.CategoryId;
            service.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new ServiceDTO
            {
                ServiceId = service.ServiceId,
                ServiceName = service.ServiceName,
                Description = service.Description,
                DurationMinutes = service.DurationMinutes,
                Price = service.Price,
                CategoryName = service.Category.CategoryName,
                IsActive = service.IsActive
            };
        }

        public async Task<bool> DeleteServiceAsync(int serviceId)
        {
            var service = await _context.Services.FindAsync(serviceId);
            if (service == null)
                return false;

            service.IsActive = false;
            service.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
