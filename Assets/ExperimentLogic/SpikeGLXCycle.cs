﻿/*
SpikeGLXCycle.cs is part of the Experica.
Copyright (c) 2016 Li Alex Zhang and Contributors

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
using Experica;
using System.Collections.Generic;
using ColorSpace = Experica.ColorSpace;

/// <summary>
/// Periodic modulation of parameters with SpikeGLX Data Acquisition System, and Predefined Colors
/// </summary>
public class SpikeGLXCycle : SpikeGLXCTLogic
{
    protected override void GenerateFinalCondition()
    {
        var colorspace = GetExParam<ColorSpace>("ColorSpace");
        var colorvar = GetExParam<string>("Color");
        var colorname = colorspace + "_" + colorvar;

        // get color
        List<Color> color = null;
        var data = ex.Display_ID.GetColorData();
        if (data != null)
        {
            if (data.ContainsKey(colorname))
            {
                color = data[colorname].Convert<List<Color>>();
            }
            else
            {
                Debug.Log(colorname + " is not found in colordata of " + ex.Display_ID);
            }
        }

        if (color != null)
        {
            SetEnvActiveParam("MinColor", color[0]);
            SetEnvActiveParam("MaxColor", color[1]);
        }
    }

    protected override void Logic()
    {
        switch (CondState)
        {
            case CONDSTATE.NONE:
                EnterCondState(CONDSTATE.PREICI);
                SyncFrame();
                break;
            case CONDSTATE.PREICI:
                if (PreICIHold >= ex.PreICI)
                {
                    EnterCondState(CONDSTATE.COND);
                    SyncEvent(CONDSTATE.COND.ToString());
                    SetEnvActiveParam("Visible", true);
                    SyncFrame();
                }
                break;
            case CONDSTATE.COND:
                var param = GetExParam<string>("ModulateParam");
                var freq = GetEnvActiveParam<float>("ModulateTemporalFreq");
                var cycledir = GetExParam<float>("CycleDirection");
                var div = (float)CondHold / 1000f * freq;
                var nc = Mathf.Floor(div);
                var phase = div - nc;
                if (nc >= ex.CondRepeat)
                {
                    StartStopExperiment(false);
                    return;
                }
                else
                {
                    switch (param)
                    {
                        case "ModulateTime":
                            SetEnvActiveParam("ModulateTimeSecond", phase / freq * cycledir);
                            break;
                        case "Ori":
                            SetEnvActiveParam("Ori", phase * cycledir * 360f);
                            break;
                        case "DKLIsoLum":
                            SetEnvActiveParam("MinColor", (phase * cycledir * 360f).DKLIsoLum(GetExParam<float>("Intercept"), ex.Display_ID));
                            break;
                        case "DKLIsoSLM":
                            SetEnvActiveParam("MinColor", (phase * cycledir * 360f).DKLIsoSLM(GetExParam<float>("Intercept"), ex.Display_ID));
                            break;
                        case "DKLIsoLM":
                            SetEnvActiveParam("MinColor", (phase * cycledir * 360f).DKLIsoLM(GetExParam<float>("Intercept"), ex.Display_ID));
                            break;
                    }
                }
                SyncFrame();
                break;
        }
    }
}
