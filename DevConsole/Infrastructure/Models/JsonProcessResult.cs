using System;

namespace DevConsole.Infrastructure.Models;

public class JsonProcessResult<T> where T : class
{
    public JsonProcessResult(T? output, string error)
    {
        Output = output;
        Error = error ?? throw new ArgumentNullException(nameof(error));
    }

    public T? Output { get; set; }

    public string Error { get; set; }
}