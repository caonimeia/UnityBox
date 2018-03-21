using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using System;
using System.IO;

public class BuildProtobufFile {
    private static readonly string _fileName = "protoc.exe";
    private static readonly string _curPath = Environment.CurrentDirectory + "/Assets/Editor/Protobuf/";
    private static readonly string _protoFilePath = _curPath + "ProtoFile/";
    private static readonly string _csharpOutPath = Environment.CurrentDirectory + "/Assets/Scripts/Protobuf/";
    private static Process _process;

    [MenuItem("XoYo/Goole Protobuf/ReBuildAll")]
    static void ReBuild() {
        _process = new Process();
        _process.StartInfo.FileName = _curPath + _fileName;
        _process.StartInfo.UseShellExecute = false;
        _process.StartInfo.RedirectStandardOutput = true;
        _process.StartInfo.RedirectStandardError = true;
        _process.StartInfo.CreateNoWindow = true;

        DirectoryInfo info = new DirectoryInfo(_protoFilePath);
        WalkDirectoryTree(info, "");
        SDebug.Log("Protobuf ReBuild All Completed");
    }

    private static void SetupArgs(string protoFileName) {
        string args = " --csharp_opt=base_namespace=Protocol";
        args = args + string.Format(" --proto_path={0} --csharp_out={1} {2}",
            _protoFilePath, _csharpOutPath, protoFileName);
        _process.StartInfo.Arguments = args;
    }

    private static void WalkDirectoryTree(DirectoryInfo root, string rootDicName) {
        FileInfo[] files = null;
        DirectoryInfo[] subDirs = null;
        subDirs = root.GetDirectories();
        foreach(DirectoryInfo dirInfo in subDirs) {
            WalkDirectoryTree(dirInfo, rootDicName + dirInfo.Name + "/");
        }

        files = root.GetFiles("*.proto");
        if(files != null) {
            foreach(FileInfo fi in files) {
                string protoFileName = rootDicName + fi.Name;
                SetupArgs(protoFileName);
                _process.Start();

                while(!_process.StandardError.EndOfStream) {
                    SDebug.LogError(_process.StandardError.ReadLine());
                }
            }
        }
    }
}
