/*	File Name:		UnlitShaderDescCreator.cs
 * 	Author:			huijie wu
 * 	Create Time: 	2018-03-21 11:42:39
 *	Description:	
 */


using System.Collections.Generic;

namespace MFLib {
    
    public class FileHeaderDesc {
        public static string[] InsertHeaderDesc(string fileName, string[] lines) {
            int index = 0;
            List<string> lineList = new List<string>(lines);

            lineList.Insert(index++, string.Format("{0, -30}{1}", "// File Name:", fileName));
            lineList.Insert(index++, string.Format("{0, -30}{1}", "// Author:", "huijie wu"));
            lineList.Insert(index++, string.Format("{0, -30}{1}", "// Create Time:", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
            lineList.Insert(index++, string.Format("{0, -30}{1}", "// Description:", ""));
            lineList.Insert(index++, System.Environment.NewLine);

            return lineList.ToArray();
        }
    }

    public class UnlitShaderDescCreator : UnityEditor.AssetModificationProcessor {
        private static void OnWillCreateAsset(string path) {
            System.Text.Encoding utf8WithoutBom = new System.Text.UTF8Encoding(false);
            path = path.Replace(".meta", "");
            if(path.EndsWith(".shader")) {
                string[] lines = System.IO.File.ReadAllLines(path);
                string fileName = path.Substring(path.LastIndexOf("/") + 1);
                lines = FileHeaderDesc.InsertHeaderDesc(fileName, lines);
                System.IO.File.WriteAllText(path, string.Join(System.Environment.NewLine, lines), utf8WithoutBom);
            }
        }
    }
}