﻿namespace KioskService.Core.DTO
{

    public class Response
    {
        public int statusCode { get; set; }
        public string? message { get; set; }
        public string? errorType { get; set; }
        public string? stackTrace { get; set; }
        public DateTime? date { get; set; }
        public object? data { get; set; }
    }
}
