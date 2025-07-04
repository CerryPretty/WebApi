namespace WebApi.Intarface
{
    using WebApi.Models; 

    public interface IServiceCatalogService : IService<ServiceCatalog>
    {
        Task<IEnumerable<ServiceCatalog>> GetAllServiceCatalogs();
        Task<ServiceCatalog> GetServiceCatalogById(int id);
    }
}