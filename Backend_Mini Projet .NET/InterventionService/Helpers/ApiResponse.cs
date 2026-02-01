namespace InterventionService.Helpers
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
        public Dictionary<string, string[]>? Errors { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        // Renommer la méthode statique
        public static ApiResponse<T> CreateSuccess(T? data, string? message = null)
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message ?? "Opération réussie",
                Data = data
            };
        }

        public static ApiResponse<T> CreateError(string message, Dictionary<string, string[]>? errors = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Errors = errors
            };
        }

        public static ApiResponse<T> CreateError(string message, Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary modelState)
        {
            var errors = new Dictionary<string, string[]>();
            foreach (var key in modelState.Keys)
            {
                var state = modelState[key];
                if (state?.Errors != null && state.Errors.Count > 0)
                {
                    errors[key] = state.Errors.Select(e => e.ErrorMessage).ToArray();
                }
            }

            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Errors = errors
            };
        }
    }
}