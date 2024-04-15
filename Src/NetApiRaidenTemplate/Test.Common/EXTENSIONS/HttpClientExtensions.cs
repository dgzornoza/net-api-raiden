using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using $safeprojectname$.Models;

namespace $safeprojectname$.Extensions;

public static class HttpClientExtensions
{
    public static readonly string HeaderBearerKey = "Bearer";

    public static async Task<TResult?> PostFileAsync<TResult>(this HttpClient client, HttpRequestDataModel<MultipartFormValuesModel> requestData)
        where TResult : class
    {
        return await client.HttpRequest<TResult>(requestData, HttpMethod.Post);
    }

    public static async Task<TResult?> PostAsync<TRequest, TResult>(this HttpClient client, HttpRequestDataModel<TRequest> requestData)
        where TRequest : class
        where TResult : class
    {
        return await client.HttpRequest<TResult>(requestData, HttpMethod.Post);
    }

    public static async Task<TResult?> PutAsync<TRequest, TResult>(this HttpClient client, HttpRequestDataModel<TRequest> requestData)
        where TRequest : class
        where TResult : class
    {
        return await client.HttpRequest<TResult>(requestData, HttpMethod.Put);
    }

    public static async Task<TResult?> GetAsync<TResult>(this HttpClient client, HttpRequestDataModel requestData)
        where TResult : class
    {
        return await client.HttpRequest<TResult>(requestData, HttpMethod.Get);
    }

    public static async Task DeleteAsync(this HttpClient client, HttpRequestDataModel requestData)
    {
        await client.HttpRequest<object>(requestData, HttpMethod.Get);
    }

    public static async Task<TResult?> GetResponse<TResult>(this HttpResponseMessage responseMessage) where TResult : class
    {
        if (!responseMessage.IsSuccessStatusCode)
        {
            var innerException = new InvalidOperationException(await responseMessage.Content.ReadAsStringAsync());
            throw new HttpRequestException(responseMessage.ReasonPhrase, innerException);
        }

        if (responseMessage.StatusCode == System.Net.HttpStatusCode.NoContent)
        {
            return default;
        }

        return responseMessage.Content.Headers?.ContentType?.MediaType switch
        {
            "application/json" => JsonSerializer.Deserialize<TResult?>(await responseMessage.Content.ReadAsStringAsync(), JsonSerializerOptions()),
            "text/html" => await responseMessage.Content.ReadAsStringAsync() as TResult,
            _ => await responseMessage.Content.ReadAsByteArrayAsync() as TResult
        };
    }

    private static async Task<TResult?> HttpRequest<TResult>(this HttpClient client, HttpRequestDataModel requestData, HttpMethod httpMethod)
        where TResult : class
    {
        client.ConfigureClientHeaders(requestData.Token);

        using var requestMessage = CreateRequestMessage(requestData, httpMethod);

        var response = await client.SendAsync(requestMessage);

        return await response.GetResponse<TResult>();
    }

    private static void ConfigureClientHeaders(this HttpClient client, string? token)
    {
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        if (!string.IsNullOrWhiteSpace(token))
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(HeaderBearerKey, token);
        }
    }

    private static HttpRequestMessage CreateRequestMessage(HttpRequestDataModel requestData, HttpMethod httpMethod)
    {
        var requestMessage = new HttpRequestMessage(httpMethod, requestData.RequestUri);

        requestData.RequestHeaders?.ToList()
            .ForEach(param => requestMessage.Headers.Add(param.Key, param.Value));

        if (requestData is IModelWrapper<object> requestDataWithModel)
        {
            requestMessage.Content = GetContent(requestDataWithModel.Model);
        }

        return requestMessage;
    }

    private static HttpContent GetContent<TRequest>(TRequest? model) where TRequest : class =>
        model switch
        {
            FormValuesModel form => new FormUrlEncodedContent(form.Values.ToList()),
            MultipartFormValuesModel multipart => GetMultipartFormDataContent(multipart),
            _ => new StringContent(JsonSerializer.Serialize(model, JsonSerializerOptions()), Encoding.UTF8, "application/json")
        };

    private static HttpContent GetMultipartFormDataContent(MultipartFormValuesModel multipart)
    {
        var multipartFormDataContent = new MultipartFormDataContent();

        if (multipart.Values != null)
        {
            foreach (var value in multipart.Values)
            {
                multipartFormDataContent.Add(new StringContent(value.Key), value.Value);
            }
        }

        multipartFormDataContent.Add(new ByteArrayContent(multipart.FileContent), multipart.FileFieldName, multipart.FileName);
        return multipartFormDataContent;
    }

    private static JsonSerializerOptions JsonSerializerOptions() => new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };
}

