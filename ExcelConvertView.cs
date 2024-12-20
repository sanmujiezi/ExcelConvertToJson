using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ExcelConvert.Controller;
using LitJson;
using OfficeOpenXml;
using UnityEditor;
using UnityEngine;


namespace ExcelConvert
{
   
    namespace View
    {
        public partial class ExcelConvertView : EditorWindow
        {
            private static float windowsWidth = 300f;
            private static float windowsHeight = 200f;
            private int _convertType = 0;

            [MenuItem("Tools/Excel Converter")]
            public static void ShowWindow()
            {
                var windows = GetWindow<ExcelConvertView>();
                CenterWindow(windows);
                windows.minSize = new Vector2(windowsWidth + 30f, windowsHeight);
                
                //读取数据
                ExcelConvertController.Instance.LoadSelectionData();
                
            }

            private static void CenterWindow(ExcelConvertView window)
            {
                Rect mainWindowRect = GetMainEditorWindowPosition();
                if (mainWindowRect == Rect.zero)
                {
                    Debug.LogWarning("Failed to get the main editor window position. Default positioning applied.");
                    return;
                }

                float centeredX = mainWindowRect.x + (mainWindowRect.width - windowsWidth) / 2f;
                float centeredY = mainWindowRect.y + (mainWindowRect.height - windowsHeight) / 2f;

                window.position = new Rect(centeredX, centeredY, windowsWidth, windowsHeight);
            }

            private static Rect GetMainEditorWindowPosition()
            {
                Type containerWindowType = typeof(Editor).Assembly.GetType("UnityEditor.ContainerWindow");
                if (containerWindowType == null)
                {
                    Debug.LogError("Unable to find UnityEditor.ContainerWindow type.");
                    return Rect.zero;
                }

                FieldInfo showModeField =
                    containerWindowType.GetField("m_ShowMode", BindingFlags.NonPublic | BindingFlags.Instance);
                PropertyInfo positionProperty =
                    containerWindowType.GetProperty("position", BindingFlags.Public | BindingFlags.Instance);

                if (showModeField == null || positionProperty == null)
                {
                    Debug.LogError("Unable to retrieve required fields or properties.");
                    return Rect.zero;
                }

                foreach (UnityEngine.Object window in Resources.FindObjectsOfTypeAll(containerWindowType))
                {
                    int showMode = (int)showModeField.GetValue(window);
                    // ShowMode 4 is typically the main Unity editor window
                    if (showMode == 4)
                    {
                        return (Rect)positionProperty.GetValue(window, null);
                    }
                }

                Debug.LogError("Unable to find main editor window.");
                return Rect.zero;
            }

            public void OnGUI()
            {
                GUILayout.BeginArea(new Rect(0, 0, position.width, position.height));
                GUILayout.BeginVertical();
                GUILayout.Label("ExcelConvertToJson", EditorStyles.boldLabel);
                GUILayout.Label("");

                GUILayout.BeginHorizontal();
                _convertType = (int)ExcelConvertController.Instance.DataFileType;
                if (GUILayout.Toggle(_convertType == 1,"Json",  GUILayout.Width(100)))
                {
                    _convertType = 1;
                    ExcelConvertController.Instance.SetConvertType((DataFileType)_convertType);
                    ExcelConvertController.Instance.SaveSeletionData();
                }
                if (GUILayout.Toggle(_convertType == 2,"Binary",  GUILayout.Width(100)))
                {
                    _convertType = 2;
                    ExcelConvertController.Instance.SetConvertType((DataFileType)_convertType);
                    ExcelConvertController.Instance.SaveSeletionData();
                }
                if (GUILayout.Toggle(_convertType == 3,"Xml",  GUILayout.Width(100)))
                {
                    _convertType = 3;
                    ExcelConvertController.Instance.SetConvertType((DataFileType)_convertType);
                    ExcelConvertController.Instance.SaveSeletionData();
                }
                
                GUILayout.EndHorizontal();
                
                GUILayout.BeginHorizontal();
                GUILayout.Label("InputPath: ", GUILayout.Width(80));
                GUILayout.TextField(ExcelConvertController.Instance.excelPath, GUILayout.Width(200));
                if (GUILayout.Button("...", GUILayout.Width(20)))
                {
                    InputPath();
                }

                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("CreateModel",GUILayout.Width(100)))
                {
                    CreateModel();
                }
                if (GUILayout.Button("Convert", GUILayout.Width(100)))
                {
                    ConvertButton();
                    //Debug.Log("Convert");
                }

                


                GUILayout.EndHorizontal();

                GUILayout.EndVertical();
                GUILayout.EndArea();
            }

            private void CreateModel()
            {
                ExcelConvertController.Instance.CreateModel();
                
            }

            private void ConvertButton()
            {
                ExcelConvertController.Instance.CreateModel();
                EditorApplication.update += CheckCompilationStatus;
                isRunning = true;
            }


            private void InputPath()
            {
                string temp_excelPath = EditorUtility.OpenFolderPanel("Select a Folder", Application.dataPath,"");
                if (!string.IsNullOrEmpty(temp_excelPath))
                {
                    Debug.Log($"<color=yellow>输入PATH：</color>{temp_excelPath}");
                    ExcelConvertController.Instance.SetExcelPath(temp_excelPath);
                    ExcelConvertController.Instance.SaveSeletionData();
                }
                else
                {
                    Debug.Log("File not found");
                }
            }
          
        }
    }
}