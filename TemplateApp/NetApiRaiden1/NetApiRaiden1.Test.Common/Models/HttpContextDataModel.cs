namespace NetApiRaiden1.Test.Common.Models;

public interface IModelWrapper<out T> where T : class
{
    public T? Model { get; }
}


public class HttpRequestDataModel<TRequest> : HttpRequestDataModel, IModelWrapper<TRequest> where TRequest : class
{
    public HttpRequestDataModel(string requestUri, TRequest model) : base(requestUri)
    {
        Model = model;
    }

    public TRequest? Model { get; private set; }
}

public class HttpRequestDataModel
{
    public string RequestUri { get; }
    public string? Token { get; set; }
    public IDictionary<string, string>? RequestHeaders { get; set; } = default!;

    public HttpRequestDataModel(string requestUri)
    {
        RequestUri = requestUri;
    }
}

public class FormValuesModel
{
    public IDictionary<string, string> Values { get; set; } = default!;
}

public class MultipartFormValuesModel
{
    public IDictionary<string, string> Values { get; set; } = default!;

    public string FileFieldName { get; set; } = default!;
    public string FileName { get; set; } = default!;
    public byte[] FileContent { get; set; } = default!;
}
