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
    public async Task<string> GetTagList()
    {
        var client = _httpClientFactory.CreateClient(ClientSettings.HttpClientName);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        var url = $"{_settings.Api}action/tag_list";
        
        using var response = await client.GetAsync(url, 
            HttpCompletionOption.ResponseHeadersRead);
        if (response.StatusCode == HttpStatusCode.NotFound) throw new Exception("Tags list not found");
        
        return await response.Content.ReadAsStringAsync();
        // return await JsonSerializer.DeserializeAsync<Response<string[]>>(
        //            await response.Content.ReadAsStreamAsync(), _jsonSerializerOptions) ?? 
        //        throw new Exception($"invalid response, cannot parse {nameof(Response<string[]>)}");
    }
    
    /// <summary>
    /// Gets the packages list.
    /// </summary>
    /// <returns>The package list.</returns>
    /// <exception cref="Exception"></exception>
    public async Task<string> GetPackageList()
    {
        var client = _httpClientFactory.CreateClient(ClientSettings.HttpClientName);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        var url = $"{_settings.Api}action/package_list";
        
        using var response = await client.GetAsync(url, 
            HttpCompletionOption.ResponseHeadersRead);
        if (response.StatusCode == HttpStatusCode.NotFound) throw new Exception("Packages list not found");

        return await response.Content.ReadAsStringAsync();
        // return await JsonSerializer.DeserializeAsync<Response<string[]>>(
        //            await response.Content.ReadAsStreamAsync(), _jsonSerializerOptions) ?? 
        //        throw new Exception($"invalid response, cannot parse {nameof(Response<string[]>)}");
    }
    
    /// <summary>
    /// Gets the organizations list.
    /// </summary>
    /// <returns>The organizations list.</returns>
    /// <exception cref="Exception"></exception>
    public async Task<string> GetOrganizationList()
    {
        var client = _httpClientFactory.CreateClient(ClientSettings.HttpClientName);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        var url = $"{_settings.Api}action/organization_list";
        
        using var response = await client.GetAsync(url, 
            HttpCompletionOption.ResponseHeadersRead);
        if (response.StatusCode == HttpStatusCode.NotFound) throw new Exception("Packages list not found");

        return await response.Content.ReadAsStringAsync();
        // return await JsonSerializer.DeserializeAsync<Response<string[]>>(
        //            await response.Content.ReadAsStreamAsync(), _jsonSerializerOptions) ?? 
        //        throw new Exception($"invalid response, cannot parse {nameof(Response<string[]>)}");
    }
    
    public async Task<string> GetOrganization(string organizationName)
    {
        const int maxTries = 100;
        var tries = maxTries;
        while (tries > 0)
        {
            await Task.Delay(Random.Shared.Next(10000) + 1000);
            
            using var client = _httpClientFactory.CreateClient(ClientSettings.HttpClientName);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Authorization", $"{_settings.ApiKey}");
            var url = $"{_settings.Api}action/organization_show?id={organizationName}";

            using var response = await client.GetAsync(url,
                HttpCompletionOption.ResponseHeadersRead);
            if (response.StatusCode == HttpStatusCode.NotFound)
                throw new Exception("404 is an invalid response in this api");
            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                await Task.Delay(Random.Shared.Next(10000) + 1000);
                tries--;
                continue;
            }

            return await response.Content.ReadAsStringAsync();
            // return JsonSerializer.Deserialize<Response<Organization>>(
            //             responseString, _jsonSerializerOptions) ??
            //        throw new Exception($"invalid response, cannot parse {nameof(Response<Organization>)}");
        }

        throw new Exception($"Could not get organization after {maxTries}");
    }

    /// <summary>
    /// Gets the package with the given name.
    /// </summary>
    /// <param name="packageName">The package name.</param>
    /// <returns>The package.</returns>
    /// <exception cref="Exception"></exception>
    public async Task<string> GetPackage(string packageName)
    {
        const int maxTries = 100;
        var tries = maxTries;
        while (tries > 0)
        {
            await Task.Delay(Random.Shared.Next(10000) + 1000);
            
            var client = _httpClientFactory.CreateClient(ClientSettings.HttpClientName);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Authorization", $"{_settings.ApiKey}");
            var url = $"{_settings.Api}action/package_show?id={packageName}";
        
            using var response = await client.GetAsync(url, 
                HttpCompletionOption.ResponseHeadersRead);
            if (response.StatusCode == HttpStatusCode.NotFound) throw new Exception("404 is an invalid response in this api");
            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                await Task.Delay(Random.Shared.Next(10000) + 1000);
                tries--;
                continue;
            }

            return await response.Content.ReadAsStringAsync();
            // return await JsonSerializer.DeserializeAsync<Response<Package>>(
            //            , _jsonSerializerOptions) ?? 
            //        throw new Exception($"invalid response, cannot parse {nameof(Response<Package>)}");
        }

        throw new Exception($"Could not get package after {maxTries}");
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
        client.DefaultRequestHeaders.Add("Authorization", $"{_settings.ApiKey}");
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
}
