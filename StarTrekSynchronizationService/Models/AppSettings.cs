﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarTrekSynchronizationService.Models
{
    public static class AppSettings
    {
        public static string ConnectionString { get; set; }
        public static string StarTrekAPIUrl { get; set; }
    }
}