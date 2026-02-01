using BlazorApp1.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace BlazorApp1.Services
{
    public class InterventionService : IInterventionService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public InterventionService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["ApiBaseUrl"] ?? "http://localhost:5241/gateway";
        }

        public async Task<List<InterventionDto>> GetAllInterventionsAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<InterventionDto>>>(
                    $"{_baseUrl}/interventions");

                return response?.Data ?? new List<InterventionDto>();
            }
            catch (Exception)
            {
                return new List<InterventionDto>();
            }
        }

        public async Task<InterventionDto?> GetInterventionByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ApiResponse<InterventionDto>>(
                    $"{_baseUrl}/interventions/{id}");

                return response?.Data;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<InterventionDto>> GetInterventionsByReclamationIdAsync(int reclamationId)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<InterventionDto>>>(
                    $"{_baseUrl}/interventions/reclamation/{reclamationId}");

                return response?.Data ?? new List<InterventionDto>();
            }
            catch (Exception)
            {
                return new List<InterventionDto>();
            }
        }

        public async Task<List<InterventionDto>> GetInterventionsByTechnicianIdAsync(int technicianId)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<InterventionDto>>>(
                    $"{_baseUrl}/interventions/technician/{technicianId}");

                return response?.Data ?? new List<InterventionDto>();
            }
            catch (Exception)
            {
                return new List<InterventionDto>();
            }
        }

        public async Task<List<InterventionDto>> GetInterventionsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<InterventionDto>>>(
                    $"{_baseUrl}/interventions/date-range?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}");

                return response?.Data ?? new List<InterventionDto>();
            }
            catch (Exception)
            {
                return new List<InterventionDto>();
            }
        }

        public async Task<List<InterventionDto>> GetInterventionsByStatusAsync(string status)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<InterventionDto>>>(
                    $"{_baseUrl}/interventions/status/{status}");

                return response?.Data ?? new List<InterventionDto>();
            }
            catch (Exception)
            {
                return new List<InterventionDto>();
            }
        }

        public async Task<InterventionDto?> CreateInterventionAsync(CreateInterventionDto dto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/interventions", dto);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResponse<InterventionDto>>();
                    return result?.Data;
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<InterventionDto?> UpdateInterventionAsync(int id, UpdateInterventionDto dto)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{_baseUrl}/interventions/{id}", dto);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResponse<InterventionDto>>();
                    return result?.Data;
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> DeleteInterventionAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_baseUrl}/interventions/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<InterventionDto?> UpdateInterventionStatusAsync(int id, string status)
        {
            try
            {
                var request = new { Status = status };
                var response = await _httpClient.PatchAsJsonAsync($"{_baseUrl}/interventions/{id}/status", request);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResponse<InterventionDto>>();
                    return result?.Data;
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<DashboardStatsDto> GetDashboardStatsAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ApiResponse<DashboardStatsDto>>(
                    $"{_baseUrl}/interventions/dashboard/stats");

                return response?.Data ?? new DashboardStatsDto();
            }
            catch (Exception)
            {
                return new DashboardStatsDto();
            }
        }

        public async Task<List<TechnicianDto>> GetAllTechniciansAsync()
        {
            // Note: This would typically call a technicians endpoint
            // For now, we'll simulate it or create it separately
            return new List<TechnicianDto>();
        }

        public async Task<List<string>> GetAvailableStatusesAsync()
        {
            return new List<string>
            {
                "Planifiée", "EnCours", "Terminée", "Annulée", "Reportée", "EnAttente"
            };
        }
    }

    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
        public List<string>? Errors { get; set; }
    }
}