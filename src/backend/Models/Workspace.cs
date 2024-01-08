﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FunctionApp.Models
{
    public class Workspace
    {
        [JsonPropertyName("name")]
        public required string Name { get; init; }
        [JsonPropertyName("id")]
        public required string Id { get; init; }
    }
}
