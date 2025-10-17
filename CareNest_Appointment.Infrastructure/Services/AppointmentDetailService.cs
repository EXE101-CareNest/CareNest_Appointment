using CareNest_Appointment.Application.DTOs;
using CareNest_Appointment.Application.Features.Commands.Create;
using CareNest_Appointment.Application.Interfaces.Services;
using CareNest_Appointment.Infrastructure.ApiEndpoints;
using System.Linq;

namespace CareNest_Appointment.Infrastructure.Services
{
    public class AppointmentDetailService : IAppointmentDetailService
    {
        private readonly IAPIService _apiService;
        private readonly IShopService _shopService;

        public AppointmentDetailService(IAPIService apiService, IShopService shopService)
        {
            _apiService = apiService;
            _shopService = shopService;
        }

        public async Task<AppointmentDetailDto> CreateAppointmentDetailAsync(string appointmentId, AppointmentDetailInput detail)
        {
            var request = new
            {
                AppointmentId = appointmentId,
                detail.ServiceDetailId,
                detail.Note,
                detail.PetQuantity
            };

            var result = await _apiService.PostAsync<AppointmentDetailDto>("appointmentDetail", AppointmentDetailEndpoints.Create(), request);

            if (!result.IsSuccess || result.Data == null)
            {
                throw new Exception($"Failed to create appointment detail: {result.Message}");
            }

            return result.Data.Data!;
        }

        public async Task<List<AppointmentDetailDto>> GetAppointmentDetailsAsync(string appointmentId)
        {
            try
            {
                var result = await _apiService.GetAsync<AppointmentDetailResponse>("appointmentDetail", AppointmentDetailEndpoints.GetByAppointmentIds(appointmentId));

                if (!result.IsSuccess || result.Data == null)
                {
                    Console.WriteLine($"Failed to get appointment details: {result.Message}");
                    return new List<AppointmentDetailDto>();
                }

                return result.Data.Data?.Items ?? new List<AppointmentDetailDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting appointment details: {ex.Message}");
                return new List<AppointmentDetailDto>();
            }
        }

        public async Task<AppointmentDetailDashboardDto> GetDashboardAsync(string? shopId, string? fromDate, string? toDate, int? top)
        {
            var endpoint = AppointmentDetailEndpoints.Dashboard(shopId, fromDate, toDate, top);
            var result = await _apiService.GetAsync<AppointmentDetailDashboardDto>("appointmentDetail", endpoint);

            if (!result.IsSuccess || result.Data == null)
            {
                throw new Exception($"Failed to get dashboard: {result.Message}");
            }

            var dashboard = result.Data.Data!;

            // Áp dụng top (phòng khi BE chưa cắt)
            if (top.HasValue && top.Value > 0)
            {
                if (dashboard.ServiceDetailStats != null)
                {
                    dashboard.ServiceDetailStats = dashboard.ServiceDetailStats
                        .OrderByDescending(x => x.Count)
                        .Take(top.Value)
                        .ToList();
                }
                if (dashboard.ServiceStats != null)
                {
                    dashboard.ServiceStats = dashboard.ServiceStats
                        .OrderByDescending(x => x.Count)
                        .Take(top.Value)
                        .ToList();
                }
            }

            // Enrich ServiceName cho ServiceStats nếu thiếu
            if (dashboard.ServiceStats != null && dashboard.ServiceStats.Any(s => string.IsNullOrWhiteSpace(s.ServiceName) && !string.IsNullOrWhiteSpace(s.ServiceId)))
            {
                try
                {
                    // 1) Lấy tất cả category theo shop (nếu có)
                    var catEndpoint = ServiceCategoryEndpoints.Get(1, 100, "asc", shopId);
                    var catRes = await _apiService.GetAsync<PagedItems<ServiceCategoryItem>>("servicecategory", catEndpoint);

                    var categoryIds = new List<string>();
                    if (catRes.IsSuccess && catRes.Data?.Data?.Items != null)
                    {
                        categoryIds = catRes.Data.Data.Items
                            .Where(c => !string.IsNullOrWhiteSpace(c.Id))
                            .Select(c => c.Id!)
                            .Distinct()
                            .ToList();
                    }

                    var serviceMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                    if (categoryIds.Count > 0)
                    {
                        // 2) Ưu tiên POST /service/by-categories
                        var postRes = await _apiService.PostAsync<List<ServiceItem>>("service", ServiceEndpoints.GetByCategories(), categoryIds);
                        if (postRes.IsSuccess && postRes.Data?.Data != null)
                        {
                            foreach (var s in postRes.Data.Data)
                            {
                                if (!string.IsNullOrWhiteSpace(s.Id) && !string.IsNullOrWhiteSpace(s.Name))
                                {
                                    serviceMap[s.Id!] = s.Name!;
                                }
                            }
                        }
                        else
                        {
                            // 3) Fallback: GET theo từng category (pageSize=100)
                            foreach (var cid in categoryIds)
                            {
                                var svcEndpoint = ServiceEndpoints.GetByCategory(cid, 1, 100, "asc");
                                var svcRes = await _apiService.GetAsync<PagedItems<ServiceItem>>("service", svcEndpoint);
                                if (svcRes.IsSuccess && svcRes.Data?.Data?.Items != null)
                                {
                                    foreach (var s in svcRes.Data.Data.Items)
                                    {
                                        if (!string.IsNullOrWhiteSpace(s.Id) && !string.IsNullOrWhiteSpace(s.Name))
                                        {
                                            serviceMap[s.Id!] = s.Name!;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (serviceMap.Count > 0)
                    {
                        foreach (var item in dashboard.ServiceStats)
                        {
                            if (!string.IsNullOrWhiteSpace(item.ServiceId) && string.IsNullOrWhiteSpace(item.ServiceName))
                            {
                                if (serviceMap.TryGetValue(item.ServiceId!, out var name))
                                {
                                    item.ServiceName = name;
                                }
                            }
                        }
                    }
                }
                catch
                {
                    // Bỏ qua enrich nếu có lỗi dịch vụ ngoài
                }
            }

            // Enrich top-level shop info when filtering by a specific shop
            if (!string.IsNullOrWhiteSpace(shopId))
            {
                try
                {
                    var shopRes = await _shopService.GetShopById(shopId);
                    if (shopRes.IsSuccess && shopRes.Data?.Data != null)
                    {
                        dashboard.ShopId = shopId;
                        dashboard.ShopName = shopRes.Data.Data.Name;
                        // Gán cho từng item trong danh sách
                        if (dashboard.ServiceDetailStats != null)
                        {
                            foreach (var item in dashboard.ServiceDetailStats)
                            {
                                item.ShopId = shopId;
                                item.ShopName = shopRes.Data.Data.Name;
                            }
                        }
                        if (dashboard.ServiceStats != null)
                        {
                            foreach (var item in dashboard.ServiceStats)
                            {
                                item.ShopId = shopId;
                                item.ShopName = shopRes.Data.Data.Name;
                            }
                        }
                    }
                    else
                    {
                        dashboard.ShopId = shopId;
                        dashboard.ShopName = null;
                        if (dashboard.ServiceDetailStats != null)
                        {
                            foreach (var item in dashboard.ServiceDetailStats)
                            {
                                item.ShopId = shopId;
                                item.ShopName = null;
                            }
                        }
                        if (dashboard.ServiceStats != null)
                        {
                            foreach (var item in dashboard.ServiceStats)
                            {
                                item.ShopId = shopId;
                                item.ShopName = null;
                            }
                        }
                    }
                }
                catch
                {
                    dashboard.ShopId = shopId;
                    dashboard.ShopName = null;
                    if (dashboard.ServiceDetailStats != null)
                    {
                        foreach (var item in dashboard.ServiceDetailStats)
                        {
                            item.ShopId = shopId;
                            item.ShopName = null;
                        }
                    }
                    if (dashboard.ServiceStats != null)
                    {
                        foreach (var item in dashboard.ServiceStats)
                        {
                            item.ShopId = shopId;
                            item.ShopName = null;
                        }
                    }
                }
            }

            return dashboard;
        }

        // Local DTOs to parse external API formats
        private class PagedItems<T>
        {
            public List<T> Items { get; set; } = new();
        }

        private class ServiceCategoryItem
        {
            public string? Id { get; set; }
            public string? Name { get; set; }
            public string? ShopId { get; set; }
        }

        private class ServiceItem
        {
            public string? Id { get; set; }
            public string? Name { get; set; }
            public string? ServiceCategoryId { get; set; }
            public bool Status { get; set; }
        }
    }
}
