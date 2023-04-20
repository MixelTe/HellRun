using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;

public class BuildPostprocessor
{
    [PostProcessBuild(1)]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
        var indexFile = Path.Join(pathToBuiltProject, "index.html");
        if (!File.Exists(indexFile))
		{
            Debug.LogError($"Failed to postprocess. File dont exist: {indexFile}");
            return;
		}

        var insertHtmlPath = Path.Join(Application.dataPath, "Yandex", "InsertHtml.html");
        if (!File.Exists(insertHtmlPath))
        {
            Debug.LogError($"Failed to postprocess. File dont exist: {indexFile}");
            return;
        }

        using var fileRead = File.OpenText(indexFile);
        var html = fileRead.ReadToEnd();

        if (html.Contains("https://yandex.ru/games/sdk/v2"))
        {
            Debug.Log($"Postprocess canceled: index.html already postprocessed");
            return;
        }

        using var insertHtmlFile = File.OpenText(insertHtmlPath);
        var insertHtml = insertHtmlFile.ReadToEnd();

        html = html.Replace("width: 960px; height: 600px;", "width: 100%; height: 100%;");
        html = html.Replace("margin: 0;", "margin: 0; position: absolute; top: 0; left: 0; width: 100%; height: 100%; display: flex;");
        html = html.Replace("</head>", insertHtml + "</head>");
        html = html.Replace(";\n    </script>", ".then((unityInstance) => {myGameInstance = unityInstance;});" + ";\n    </script>");
        fileRead.Close();
        using var fileWrite = new StreamWriter(indexFile);
        fileWrite.Write(html);
        Debug.Log($"Build was successfully postprocessed");
    }
}
