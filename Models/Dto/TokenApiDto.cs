﻿namespace MaaranTicketingSystemAPI.Models.Dto
{
    public class TokenApiDto
    {
        
        public string AccessToken { get; set; } = string.Empty;

        public string RefreshToken { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

    }
}
