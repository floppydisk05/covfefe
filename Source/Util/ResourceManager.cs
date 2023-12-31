using System.IO;

namespace WinBot.Util; 

public static class ResourceManager {
    public static string GetResourcePath(string name, ResourceType type = ResourceType.MiscData) {
        // Get a file name
        var fileName = name;
        if (type == ResourceType.Config || type == ResourceType.JsonData)
            fileName += ".json";
        // MiscData and Resource are treated as is

        // Get the full path
        var path = fileName;
        switch (type) {
            case ResourceType.JsonData:
            case ResourceType.MiscData:
                path = "Data/" + path;
                break;
            case ResourceType.Resource:
                path = "Resources/" + path;
                break;
        }

        return path;
    }

    public static bool ResourceExists(string name, ResourceType type = ResourceType.MiscData) {
        return File.Exists(GetResourcePath(name, type));
    }
}

public enum ResourceType {
    JsonData,
    MiscData,
    Config,
    Resource
}
