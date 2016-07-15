﻿// --------------------------------------------------------------
// ENCamera.cs is part of the VLAB project.
// Copyright (c) 2016 All Rights Reserved
// Li Alex Zhang fff008@gmail.com
// 5-21-2016
// --------------------------------------------------------------

using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

namespace VLab
{
    [NetworkSettings(channel = 0, sendInterval = 0)]
    public class ENCamera : NetworkBehaviour
    {
        [SyncVar(hook = "onbgcolor")]
        public Color BGColor = new Color();
        [SyncVar(hook = "onscreenhalfheight")]
        public float ScreenHalfHeight = 15;
        [SyncVar(hook = "onscreentoeye")]
        public float ScreenToEye = 57;

        public new Camera camera;
#if VLAB
        VLNetManager netmanager;
#endif

        void Awake()
        {
            OnAwake();
        }
        public virtual void OnAwake()
        {
            camera = gameObject.GetComponent<Camera>();
#if VLAB
            netmanager = FindObjectOfType<VLNetManager>();
#endif
        }

        void onbgcolor(Color c)
        {
            OnBGColor(c);
        }
        public virtual void OnBGColor(Color c)
        {
            if (camera != null)
            {
                camera.backgroundColor = c;
            }
            BGColor = c;
        }

        void onscreenhalfheight(float shh)
        {
            OnScreenHalfHeight(shh);
        }
        public virtual void OnScreenHalfHeight(float shh)
        {
            if (camera != null)
            {
                camera.orthographicSize = Mathf.Rad2Deg * Mathf.Atan2(shh, ScreenToEye);
            }
            ScreenHalfHeight = shh;
        }

        void onscreentoeye(float ste)
        {
            OnScreenToEye(ste);
        }
        public virtual void OnScreenToEye(float ste)
        {
            if (camera != null)
            {
                camera.orthographicSize = Mathf.Rad2Deg * Mathf.Atan2(ScreenHalfHeight, ste);
            }
            ScreenToEye = ste;
        }

#if VLAB
        public override bool OnCheckObserver(NetworkConnection conn)
        {
            return netmanager.IsConnectionPeerType(conn, VLPeerType.VLabEnvironment);
        }

        public override bool OnRebuildObservers(HashSet<NetworkConnection> observers, bool initialize)
        {
            var isrebuild = false;
            var cs = netmanager.GetPeerTypeConnection(VLPeerType.VLabEnvironment);
            if (cs.Count > 0)
            {
                foreach (var c in cs)
                {
                    observers.Add(c);
                }
                isrebuild = true;
            }
            return isrebuild;
        }
#endif

    }
}