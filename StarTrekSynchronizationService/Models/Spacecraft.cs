using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarTrekSynchronizationService.Models
{
    public class Spacecraft
    {
        [Key]
        public string UID { get; set; }
        public string Name { get; set; }
        public string Registry { get; set; }
        public string? Status { get; set; }
        public string? DateStatus { get; set; }
        public DateTime SystemDate { get; set; }
        public DateTime? LastChange { get; set; }
        public bool Deleted { get; set; }

    }
}
