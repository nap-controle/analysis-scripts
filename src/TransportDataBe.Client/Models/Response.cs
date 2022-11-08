namespace TransportDataBe.Client.Models;

public class Response<T>
{
    public Response(bool success, T result)
    {
        Success = success;
        Result = result;
    }

    public bool Success { get; }
    
    public T Result { get; }
}