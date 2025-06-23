using UnityEngine;
using UnityEditor;
using System.IO;

namespace HuayaWASM
{
    public class HYEditor
    {
         // 配置参数（按需修改）
        private const string PLUGIN_NAME = "com.huya.minigame"; // 插件名称（插件主文件夹名）
        private const string SOURCE_FOLDER = "WebGLTemplates"; // 插件内需要复制的子目录
        private const string TARGET_FOLDER = "Assets/WebGLTemplates"; // Assets内的目标位置

        [MenuItem("虎牙小游戏 / 添加构建模版", false, 1)]
        public static void AddBuildTemplate()
        {
            if (!Directory.Exists(TARGET_FOLDER))
            {
                Directory.CreateDirectory(TARGET_FOLDER);
            }
            string templateName = "";
#if TUANJIE_2022_3_OR_NEWER
    templateName = "HYTemplate2022TJ";
#else
#if UNITY_2022_3_OR_NEWER
    templateName = "HYTemplate2022";
#elif UNITY_2020_1_OR_NEWER
    templateName = "HYTemplate2020";
#else
    templateName = "HYTemplate";
#endif
#endif
            string targetDirectory = Path.Combine(TARGET_FOLDER, templateName);
            string sourceDirectory = Path.Combine("Packages", "com.huya.minigame", SOURCE_FOLDER, templateName);
            if (Directory.Exists(sourceDirectory))
            {
                FileUtil.CopyFileOrDirectory(sourceDirectory, targetDirectory);
                AssetDatabase.Refresh();
                Debug.Log("插件资源已复制到 Assets 目录。");
            }
        }
    }
}