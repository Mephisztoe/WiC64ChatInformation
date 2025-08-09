using System.Media;
using System.Reflection;

namespace WiC64ChatInformation;
public static class ResourceHelper
{
    public static Stream GetEmbeddedResourceStream(string resourceName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourcePath = $"{assembly.GetName().Name!.Replace(" ", "_")}.Assets.{resourceName}";
        return assembly.GetManifestResourceStream(resourcePath)
               ?? throw new FileNotFoundException($"Resource '{resourcePath}' not found.");
    }

    public static Icon GetEmbeddedIcon(string iconName)
    {
        using var stream = GetEmbeddedResourceStream(iconName);
        return new Icon(stream);
    }

    public static Image GetEmbeddedImage(string resourceName)
    {
        using var icon = GetEmbeddedIcon(resourceName);
        return icon.ToBitmap();
    }

    public static string ExtractResourceToTempFile(string resourceName, string fileName)
    {
        using var stream = GetEmbeddedResourceStream(resourceName);

        var tempPath = Path.Combine(Path.GetTempPath(), fileName);

        using var fileStream = new FileStream(tempPath, FileMode.Create, FileAccess.Write);
        stream.CopyTo(fileStream);

        return tempPath;
    }

    public static SoundPlayer GetEmbeddedSoundPlayer(string resourceName)
    {       
        using var resourceStream = GetEmbeddedResourceStream(resourceName);
                
        var memoryStream = new MemoryStream();
        resourceStream.CopyTo(memoryStream);
               
        memoryStream.Position = 0;
              
        return new SoundPlayer(memoryStream);
    }
}