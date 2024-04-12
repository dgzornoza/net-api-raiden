using System.Diagnostics;
using System.Reflection;
using System.Text.Json;

namespace NetApiRaiden1.Test.Common.Helpers;

public static class FilesHelpers
{
    /// <summary>
    /// Funcion para obtener un objeto desde un archivo json en la carpeta 'Resources' local al test
    /// </summary>
    /// <typeparam name="TObject">Tipo de objeto a obtener</typeparam>
    /// <param name="resourceName">Nombre el archivo json dentro de la carpeta 'Resoruces' en el directorio del test</param>
    /// <returns>Objeto deserializado desde el json</returns>
    /// <remarks>El archivo debe tener la propiedad "copiar siempre al directorio de salida"</remarks>
    public static async Task<TObject?> GetLocalResourceJsonObject<TObject>(string resourceName) =>
        JsonSerializer.Deserialize<TObject>(await GetEmbeddedResource<string>(resourceName));

    /// <summary>
    /// Funcion para obtener el contenido de texto desde un archivo de texto en la carpeta 'Resources' local al test
    /// </summary>
    /// <param name="resourceName">Nombre el archivo de recurso dentro de la carpeta 'Resoruces' en el directorio del test</param>
    /// <returns>Contenido del archivo de texto</returns>
    /// <remarks>El archivo debe tener la propiedad "copiar siempre al directorio de salida"</remarks>
    public static async Task<string> GetLocalResourceTextContent(string resourceName) => await GetEmbeddedResource<string>(resourceName);

    /// <summary>
    /// Funcion para obtener el contenido binario desde un archivo de texto en la carpeta 'Resources' local al test
    /// </summary>
    /// <param name="resourceName">Nombre el archivo de recurso dentro de la carpeta 'Resources' en el directorio del test</param>
    /// <returns>Contenido del archivo binaio</returns>
    /// <remarks>El archivo debe tener la propiedad "copiar siempre al directorio de salida"</remarks>
    public static async Task<byte[]> GetLocalResourceBytesContent(string resourceName) => await GetEmbeddedResource<byte[]>(resourceName);

    private static async Task<T> GetEmbeddedResource<T>(string resourceName) where T : class
    {
        var currentAssemblyName = Assembly.GetExecutingAssembly().GetName().Name;
        var frame = new StackTrace().GetFrames()
            .First(item =>
            {
                var assemblyName = item.GetMethod()?.DeclaringType?.Assembly?.GetName().Name;
                return assemblyName!.StartsWith("NetApiRaiden1.Test") && assemblyName != currentAssemblyName;
            });

        var type = frame.GetMethod()?.DeclaringType;
        var assembly = type?.Assembly ?? throw new KeyNotFoundException("Error en la pila de llamadas");
        var resourcePath = $"{type.Namespace}.Resources.{resourceName}";

        T? result = null!;
        using (var stream = assembly.GetManifestResourceStream(resourcePath)!)
        {
            var resultType = typeof(T);
#pragma warning disable S2589 // Boolean expressions should not be gratuitous
            result = resultType switch
            {
                Type _ when resultType == typeof(string) => await stream.GetText() as T,
                Type _ when resultType == typeof(byte[]) => await stream.GetBytes() as T,
                _ => throw new Exception("Invalid Generic Type")
            };
#pragma warning restore S2589 // Boolean expressions should not be gratuitous
        }

        return result!;
    }

    private static async Task<byte[]> GetBytes(this Stream stream)
    {
        using var reader = new BinaryReader(stream);
        var result = reader.ReadBytes((int)stream.Length);
        return await Task.FromResult(result);
    }

    private static async Task<string> GetText(this Stream stream)
    {
        using var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync();
    }
}
