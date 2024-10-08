﻿namespace Fhi.ClientCredentialsKeypairs;

public partial class ClientCredentialsConfiguration
{
    public string Authority => authority;
    public string ClientId => clientId;
    public string Scopes => scopes == null ? "" : string.Join(" ", scopes);

    public string PrivateKey => privateJwk;

    /// <summary>
    /// Set this lower than the lifetime of the access token
    /// </summary>
    public int RefreshTokenAfterMinutes { get; set; } = 8;

    /// <summary>
    /// Indicates if authentication towards API-endpoints can fallback
    /// to Bearer tokens in the event they do not support DPoP tokens.
    /// This is to ensure backwards compatibility with existing endpoints
    /// and is disabled by default.
    /// </summary>
    public bool CanFallbackToBearerToken { get; set; } = false;

    public List<Api> Apis { get; set; } = new();

    public Api GetApi(string apiName) => 
        Apis.FirstOrDefault(o => o.Name == apiName) ?? throw new InvalidApiNameException($"No API with name {apiName} registered in {nameof(ClientCredentialsConfiguration)}");
}

public class Api
{
    /// <summary>
    /// User friendly name of the Api, prefer using nameof(WhateverApiService)
    /// </summary>
    public string Name { get; set; } = "";
    
    /// <summary>
    /// Actual Url to Api
    /// </summary>
    public string Url { get; set; } = "";

    /// <summary>
    /// DPoP (Demonstrating Proof of Posssession in the Application Layer)
    /// Must be supported in the receving api.
    /// https://utviklerportal.nhn.no/informasjonstjenester/helseid/bruksmoenstre-og-eksempelkode/bruk-av-helseid/docs/dpop/dpop_no_nbmd/
    /// </summary>
    public bool UseDpop { get; set; } = false;

    /// <summary>
    /// The scope that the client will use towards the specific API. Multiple scopes can be set with space after each scope.
    /// If this property is not set, the scopes defined in the ClientCredentialsConfiguration will be used instead.
    /// </summary>
    public string? Scope { get; set; }
}