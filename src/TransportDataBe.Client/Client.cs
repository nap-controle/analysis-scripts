using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using TransportDataBe.Client.Models;

namespace TransportDataBe.Client;

public class Client
{
    private readonly ILogger<Client> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ClientSettings _settings;
    private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions()
    {    
        PropertyNameCaseInsensitive = true
    };


    public Client(ILogger<Client> logger, IHttpClientFactory httpClientFactory, ClientSettings settings)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _settings = settings;
    }
    
    /// <summary>
    /// Gets the tags list.
    /// </summary>
    /// <returns>The tag list.</returns>
    /// <exception cref="Exception"></exception>
    public async Task<Response<string[]>> GetTagList()
    {
        var client = _httpClientFactory.CreateClient(ClientSettings.HttpClientName);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        var url = $"{_settings.Api}action/tag_list";
        
        using var response = await client.GetAsync(url, 
            HttpCompletionOption.ResponseHeadersRead);
        if (response.StatusCode == HttpStatusCode.NotFound) throw new Exception("Tags list not found");
        
        return await JsonSerializer.DeserializeAsync<Response<string[]>>(
                   await response.Content.ReadAsStreamAsync(), _jsonSerializerOptions) ?? 
               throw new Exception($"invalid response, cannot parse {nameof(Response<string[]>)}");
    }
    
    /// <summary>
    /// Gets the packages list.
    /// </summary>
    /// <returns>The package list.</returns>
    /// <exception cref="Exception"></exception>
    public async Task<Response<string[]>> GetPackageList()
    {
        var client = _httpClientFactory.CreateClient(ClientSettings.HttpClientName);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        var url = $"{_settings.Api}action/package_list";
        
        using var response = await client.GetAsync(url, 
            HttpCompletionOption.ResponseHeadersRead);
        if (response.StatusCode == HttpStatusCode.NotFound) throw new Exception("Packages list not found");
        
        return await JsonSerializer.DeserializeAsync<Response<string[]>>(
                   await response.Content.ReadAsStreamAsync(), _jsonSerializerOptions) ?? 
               throw new Exception($"invalid response, cannot parse {nameof(Response<string[]>)}");
    }
    
    /// <summary>
    /// Gets the organizations list.
    /// </summary>
    /// <returns>The organizations list.</returns>
    /// <exception cref="Exception"></exception>
    public async Task<Response<string[]>> GetOrganizationList()
    {
        var client = _httpClientFactory.CreateClient(ClientSettings.HttpClientName);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        var url = $"{_settings.Api}action/organization_list";
        
        using var response = await client.GetAsync(url, 
            HttpCompletionOption.ResponseHeadersRead);
        if (response.StatusCode == HttpStatusCode.NotFound) throw new Exception("Packages list not found");
        
        return await JsonSerializer.DeserializeAsync<Response<string[]>>(
                   await response.Content.ReadAsStreamAsync(), _jsonSerializerOptions) ?? 
               throw new Exception($"invalid response, cannot parse {nameof(Response<string[]>)}");
    }

    /// <summary>
    /// Gets the organization with the given name.
    /// </summary>
    /// <param name="organizationName">The organization name.</param>
    /// <returns>The organization.</returns>
    /// <exception cref="Exception"></exception>
    public async Task<Response<Organization>> GetOrganization(string organizationName)
    {
        var client = _httpClientFactory.CreateClient(ClientSettings.HttpClientName);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        var url = $"{_settings.Api}action/organization_show?id={organizationName}";
        
        using var response = await client.GetAsync(url, 
            HttpCompletionOption.ResponseHeadersRead);
        if (response.StatusCode == HttpStatusCode.NotFound) throw new Exception("404 is an invalid response in this api");

        return await JsonSerializer.DeserializeAsync<Response<Organization>>(
                   await response.Content.ReadAsStreamAsync(), _jsonSerializerOptions) ?? 
               throw new Exception($"invalid response, cannot parse {nameof(Response<Organization>)}");
    }

    /// <summary>
    /// Gets the package with the given name.
    /// </summary>
    /// <param name="packageName">The package name.</param>
    /// <returns>The package.</returns>
    /// <exception cref="Exception"></exception>
    public async Task<Response<Package>> GetPackage(string packageName)
    {
        var client = _httpClientFactory.CreateClient(ClientSettings.HttpClientName);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        var url = this.GetPackageUrl(packageName);
        
        using var response = await client.GetAsync(url, 
            HttpCompletionOption.ResponseHeadersRead);
        if (response.StatusCode == HttpStatusCode.NotFound) throw new Exception("404 is an invalid response in this api");
        
        return await JsonSerializer.DeserializeAsync<Response<Package>>(
                   await response.Content.ReadAsStreamAsync(), _jsonSerializerOptions) ?? 
               throw new Exception($"invalid response, cannot parse {nameof(Response<Package>)}");
    }

    /// <summary>
    /// Gets a document from the API.
    /// </summary>
    /// <param name="file">The file.</param>
    /// <param name="organization">The organization.</param>
    /// <returns></returns>
    public async Task<Stream> DownloadDocument(string file, Organization organization)
    {
        var client = _httpClientFactory.CreateClient(ClientSettings.HttpClientName);
        var url = $"{_settings.Website}uploads/organization/{organization.Name}/{file}";
        
        using var response = await client.GetAsync(url, 
            HttpCompletionOption.ResponseHeadersRead);
        if (response.StatusCode == HttpStatusCode.NotFound) throw new Exception("404 is an invalid response in this api");

        var memoryStream = new MemoryStream();
        var responseStream = await response.Content.ReadAsStreamAsync();
        await responseStream.CopyToAsync(memoryStream);
        memoryStream.Seek(0, SeekOrigin.Begin);
        return memoryStream;
    }

    public string GetPackageUrl(string packageName)
    {
        return $"{_settings.Api}action/package_show?id={packageName}";
    }
}
