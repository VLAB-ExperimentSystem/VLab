﻿/*
EnvNet.cs is part of the VLAB project.
Copyright (c) 2017 Li Alex Zhang and Contributors

Permission is hereby granted, free of charge, to any person obtaining a 
copy of this software and associated documentation files (the "Software"),
to deal in the Software without restriction, including without limitation
the rights to use, copy, modify, merge, publish, distribute, sublicense,
and/or sell copies of the Software, and to permit persons to whom the 
Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included 
in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF 
OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

namespace VLab
{
    public enum EnvironmentObject
    {
        None,
        Quad,
        GratingQuad,
        ImageQuad
    }

    [NetworkSettings(channel = 0, sendInterval = 0)]
    public class EnvNet : NetworkBehaviour
    {
        [SyncVar(hook = "onvisible")]
        public bool Visible = true;
        [SyncVar(hook = "onposition")]
        public Vector3 Position = new Vector3();
        [SyncVar(hook = "onpositionoffset")]
        public Vector3 PositionOffset = new Vector3();
        [SyncVar(hook = "onrotation")]
        public Vector3 Rotation = new Vector3();
        [SyncVar(hook = "onrotationoffset")]
        public Vector3 RotationOffset = new Vector3();

        public new Renderer renderer;
#if VLAB
        VLNetManager netmanager;
#endif

        void Awake()
        {
            OnAwake();
        }
        public virtual void OnAwake()
        {
            renderer = gameObject.GetComponent<Renderer>();
#if VLAB
            netmanager = FindObjectOfType<VLNetManager>();
#endif
        }

        void onvisible(bool v)
        {
            OnVisible(v);
        }
        public virtual void OnVisible(bool v)
        {
            if (renderer != null)
            {
                renderer.enabled = v;
            }
            Visible = v;
        }

        void onposition(Vector3 p)
        {
            OnPosition(p);
        }
        public virtual void OnPosition(Vector3 p)
        {
            transform.localPosition = p + PositionOffset;
            Position = p;
        }

        void onpositionoffset(Vector3 poffset)
        {
            OnPositionOffset(poffset);
        }
        public virtual void OnPositionOffset(Vector3 poffset)
        {
            transform.localPosition = Position + poffset;
            PositionOffset = poffset;
        }

        void onrotation(Vector3 r)
        {
            OnRotation(r);
        }
        public virtual void OnRotation(Vector3 r)
        {
            transform.localEulerAngles = r + RotationOffset;
            Rotation = r;
        }

        void onrotationoffset(Vector3 roffset)
        {
            OnRotationOffset(roffset);
        }
        public virtual void OnRotationOffset(Vector3 roffset)
        {
            transform.localEulerAngles = Rotation + roffset;
            RotationOffset = roffset;
        }

#if VLAB
        public override bool OnCheckObserver(NetworkConnection conn)
        {
            return netmanager.IsConnectionPeerType(conn, VLPeerType.VLabEnvironment);
        }

        public override bool OnRebuildObservers(HashSet<NetworkConnection> observers, bool initialize)
        {
            var vcs = netmanager.GetPeerTypeConnection(VLPeerType.VLabEnvironment);
            if (vcs.Count > 0)
            {
                foreach (var c in vcs)
                {
                    observers.Add(c);
                }
                return true;
            }
            return false;
        }
#endif

    }
}