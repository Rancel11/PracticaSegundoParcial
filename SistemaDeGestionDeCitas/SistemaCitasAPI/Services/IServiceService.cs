using SistemaCitasAPI.DTOs;

namespace SistemaCitasAPI.Services
{
    public interface IServiceService
    {
        Task<IEnumerable<ServiceDTO>> GetAllServicesAsync();
        Task<ServiceDTO?> GetServiceByIdAsync(int serviceId);
        Task<ServiceDTO?> CreateServiceAsync(CreateServiceDTO dto);
        Task<ServiceDTO?> UpdateServiceAsync(int serviceId, CreateServiceDTO dto);
        Task<bool> DeleteServiceAsync(int serviceId);
    }
}
