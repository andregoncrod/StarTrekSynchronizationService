using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarTrekSynchronizationService.Models
{
    public class SpacecraftsPagedAPIDto
    {
        public PagedAPIDto page { get; set; }
        public IEnumerable<SpacecraftAPIDto> spacecrafts { get; set; }
    }

    public class SpacecraftAPIDto
    {
        public string uid { get; set; }
        public string name { get; set; }
        public string registry { get; set; }
        public string status { get; set; }
        public string dateStatus { get; set; }
    }

    public class PagedAPIDto
    {
        public int pageNumber { get; set; }
        public int pageSize { get; set; }
        public int totalPages { get; set; }
    }
}
