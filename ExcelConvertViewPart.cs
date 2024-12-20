﻿using System;
using ExcelConvert.Controller;
using UnityEditor;
using UnityEngine;

namespace ExcelConvert.View
{
    public partial class ExcelConvertView 
    {
        private bool isRunning = false;
        private void OnEnable()
        {
           AssemblyReloadEvents.afterAssemblyReload += CheckCompilationStatus;
           AssemblyReloadEvents.afterAssemblyReload += ExcelConvertController.Instance.LoadSelectionData;
        }

        private void OnDisable()
        {
            AssemblyReloadEvents.afterAssemblyReload -= CheckCompilationStatus;
            AssemblyReloadEvents.afterAssemblyReload -= ExcelConvertController.Instance.LoadSelectionData;
        }
        
        private void CheckCompilationStatus()
        {
            if (EditorApplication.isCompiling)
            {
                Debug.Log("update");
            }
            else
            {
                EditorApplication.update -= CheckCompilationStatus;
                ExecuteAfterCompilation();
            }
        }
        
        private void ExecuteAfterCompilation()
        {
            if (isRunning)
            {
                OnCreateClassAfter();
            }
            isRunning = false;
        }

        private void OnCreateClassAfter()
        {
            Debug.Log("Compilation finished, executing code...");
            ExcelConvertController.Instance.ConvertData();
            
        }
        
        
        
        
    }
}