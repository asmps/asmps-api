﻿namespace ASMPS.Contracts.Authorization;

/// <summary>
/// Модель для обновления токенов
/// </summary>
public class RefreshTokensDto
{
    /// <summary>
    /// Refresh-токен
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;
}