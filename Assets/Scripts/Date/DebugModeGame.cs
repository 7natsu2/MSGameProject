﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugModeGame : MonoBehaviour {
    [System.Serializable]
    public struct DebugMode
    {
        public bool m_debugMode;　        // デバッグモード機能
        public bool m_debugPlayerEnable;  // デバッグ機能：プレイヤー操作可能状態
        public int m_debugPlayerNum;      // デバッグ機能：プレイヤー人数（遊べる）
        public bool m_controllerEnable;   // デバッグ機能：コントローラー接続有無
    }

    // デバッグモード機能
    [SerializeField]
    public DebugMode m_debugMode;

    private static DebugMode g_debugMode;

	// Use this for initialization
	void Start () {
        SetProperty();
    }
	
	// Update is called once per frame
	void Update () {
        SetProperty();
    }

    void SetProperty()
    {
        g_debugMode.m_debugMode = m_debugMode.m_debugMode;
        g_debugMode.m_debugPlayerEnable = m_debugMode.m_debugPlayerEnable;
        g_debugMode.m_debugPlayerNum = m_debugMode.m_debugPlayerNum;
        g_debugMode.m_controllerEnable = m_debugMode.m_controllerEnable;
    }

    public static DebugMode GetProperty()
    {
        return g_debugMode;
    }
}
