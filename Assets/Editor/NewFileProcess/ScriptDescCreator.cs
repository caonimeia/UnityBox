/*	File Name:		ScriptDescCreator.cs
 * 	Author:			huijie wu
 * 	Create Time: 	2018/3/9
 *	Description:	新建cs文件时在文件开始处添加相关的描述信息
 *	                占位符(例如#AUTHORNAME#)在Unity\Editor\Data\Resources\ScriptTemplates\81-C# Script-NewBehaviourScript.cs.txt中定义
 *	                修改文件编码为UTF-8，换行符为CRLF(VS2017下)
 */


public class ScriptDescCreator : UnityEditor.AssetModificationProcessor {
    private static void OnWillCreateAsset(string path) {
        path = path.Replace(".meta", "");
        if(path.EndsWith(".cs")) {
            string[] lines = System.IO.File.ReadAllLines(path);
            for(int i = 0; i < lines.Length; i++) {
                lines[i] = lines[i].Replace("#AUTHORNAME#", "huijie wu");
                lines[i] = lines[i].Replace("#CREATETIME#", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            }

            System.IO.File.WriteAllText(path, string.Join(System.Environment.NewLine, lines), System.Text.Encoding.UTF8);
            UnityEditor.AssetDatabase.Refresh();
        }
    }
}
