using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using Application.Exceptions;
using Application.Interfaces.IClients;
using Application.Interfaces.IRepositories;
using Application.Interfaces.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Application.Services;

public class ProxyService : IProxyService
{
    private readonly string _baseUrl;
    private readonly IApiClient _apiClient;
    private readonly ILogsRepository _logsRepository;
    private readonly ILogsService _logsService;

    public ProxyService(IConfiguration config, IApiClient apiClient,
            ILogsRepository logsRepository, ILogsService logsService)
    {
        _baseUrl = config["BaseApiUrl"] ??
            throw new UndefinedBaseUrlException("Base api url undefined in configuration");
        _apiClient = apiClient;
        _logsRepository = logsRepository;
        _logsService = logsService;
    }

    public async Task<HttpResponseMessage> ForwardAsync(HttpRequest incomingRequest, string targetPath, string? body)
    {
        string method = incomingRequest.Method;

        targetPath = targetPath.Replace("%2F", "/");
        string uri = $"{_baseUrl}/{targetPath}";

        var req = new HttpRequestMessage(new HttpMethod(method), uri);

        if (body != null)
            req.Content = new StringContent(body, Encoding.UTF8, "application/json");

        if (incomingRequest.Headers.TryGetValue("Authorization", out var authVals))
        {
            // Prefer robust parse; falls back to raw add if needed
            if (AuthenticationHeaderValue.TryParse(authVals.ToString(), out var auth))
            {
                req.Headers.Authorization = auth;
            }
            else
            {
                // If someone sent a non-standard value, still forward it
                req.Headers.TryAddWithoutValidation("Authorization", authVals.ToArray());
            }
        }

        Stopwatch sw = Stopwatch.StartNew();
        try
        {
            var httpResponse = await _apiClient.SendRequestAsync(req);
            sw.Stop();

            var log = await _logsService.AddLogAsync(req, httpResponse, sw);

            return httpResponse;
        }
        catch (TimeoutException ex)
        {
            await _logsService.AddLogAsync(req, new HttpResponseMessage() { StatusCode = 0 }, sw);

            throw ex;
        }
    }

}
