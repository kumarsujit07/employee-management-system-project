using System.Collections.Generic;

namespace EmployeeManagement.API.DTOs
{
    public class PaginatedResponseDto<T>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public IEnumerable<T> Data { get; set; } = new List<T>();
    }
}
