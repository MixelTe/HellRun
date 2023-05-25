using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;

public class BuildPostprocessor
{
    [PostProcessBuild(1)]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
        var indexFilePath = Path.Join(pathToBuiltProject, "index.html");
        var success = ReadFile(indexFilePath, out var html);
        if (!success) return;

        if (html.Contains("https://yandex.ru/games/sdk/v2"))
        {
            Debug.Log($"Postprocess canceled: index.html already postprocessed");
            return;
        }

        var InsertHtmlHeadPath = Path.Join(Application.dataPath, "Yandex", "InsertHtmlHead.html");
        success = ReadFile(InsertHtmlHeadPath, out var insertHtmlHead);
        if (!success) return;

        var InsertHtmlBodyPath = Path.Join(Application.dataPath, "Yandex", "InsertHtmlBody.html");
        success = ReadFile(InsertHtmlBodyPath, out var insertHtmlBody);
        if (!success) return;

        var metrikaIdPath = Path.Join(Directory.GetParent(Application.dataPath).FullName, "metrika_id.txt");
		success = ReadFile(metrikaIdPath, out var metrikaId, "Failed to set Metrica Id.");
        if (success)
		{
            insertHtmlHead = insertHtmlHead.Replace("'%MID%'", metrikaId);
            insertHtmlBody = insertHtmlBody.Replace("%MID%", metrikaId);
		}

        html = html.Replace("width: 960px; height: 600px;", "width: 100%; height: 100%;");
        html = html.Replace("margin: 0;", "margin: 0; position: absolute; top: 0; left: 0; width: 100%; height: 100%; display: flex;");
        html = html.Replace(";\n    </script>", ".then((unityInstance) => {myGameInstance = unityInstance;});" + ";\n    </script>");
        html = html.Replace("</head>", insertHtmlHead + "</head>");
        html = html.Replace("</body>", insertHtmlBody + "</body>");
        using var fileWrite = new StreamWriter(indexFilePath);
        fileWrite.Write(html);
        Debug.Log($"Build was successfully postprocessed");
    }

    private static bool ReadFile(string path, out string content, string errorText = "Failed to postprocess.")
	{
        content = "";
        if (!File.Exists(path))
        {
            Debug.LogError($"{errorText} File dont exist: {path}");
            return false;
        }
        using var fileRead = File.OpenText(path);
        content = fileRead.ReadToEnd();
        return true;
    }
}
