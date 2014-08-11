/*******************************************************************************
 * Copyright (c) 2013, Daniel Murphy
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without modification,
 * are permitted provided that the following conditions are met:
 * 	* Redistributions of source code must retain the above copyright notice,
 * 	  this list of conditions and the following disclaimer.
 * 	* Redistributions in binary form must reproduce the above copyright notice,
 * 	  this list of conditions and the following disclaimer in the documentation
 * 	  and/or other materials provided with the distribution.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
 * ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
 * IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT,
 * INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
 * NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
 * PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
 * WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
 * POSSIBILITY OF SUCH DAMAGE.
 ******************************************************************************/
/**
 * Created at 1:58:18 PM Jul 17, 2010
 */

using System;
using System.Collections.Generic;
using SharpBox2D.Common;
using SharpBox2D.Dynamics;

namespace SharpBox2D.TestBed.Framework
{

    /**
     * Stores all the testbed settings.  Automatically populates default settings.
     * 
     * @author Daniel Murphy
     */
    public class TestbedSettings
    {
        public static readonly string Hz = "Hz";
        public static readonly string PositionIterations = "Pos Iters";
        public static readonly string VelocityIterations = "Vel Iters";
        public static readonly string AllowSleep = "Sleep";
        public static readonly string WarmStarting = "Warm Starting";
        public static readonly string SubStepping = "SubStepping";
        public static readonly string ContinuousCollision = "Continuous Collision";
        public static readonly string DrawShapes = "Shapes";
        public static readonly string DrawJoints = "Joints";
        public static readonly string DrawAABBs = "AABBs";
        public static readonly string DrawContactPoints = "Contact Points";
        public static readonly string DrawContactNormals = "Contact Normals";
        public static readonly string DrawContactImpulses = "Contact Impulses";
        public static readonly string DrawFrictionImpulses = "Friction Impulses";
        public static readonly string DrawCOMs = "Center of Mass";
        public static readonly string DrawStats = "Stats";
        public static readonly string DrawHelp = "Help";
        public static readonly string DrawTree = "Dynamic Tree";
        public static readonly string DrawWireframe = "Wireframe Mode";

        public bool pause = false;
        public bool singleStep = false;

        private List<TestbedSetting> settings;
        private IDictionary<string, TestbedSetting> settingsMap;

        public TestbedSettings()
        {
            settings = new List<TestbedSetting>();
            settingsMap = new Dictionary<string, TestbedSetting>();
            populateDefaultSettings();
        }

        private void populateDefaultSettings()
        {
            addSetting(new TestbedSetting(Hz, SettingType.ENGINE, 60, 1, 400));
            addSetting(new TestbedSetting(PositionIterations, SettingType.ENGINE, 3, 0, 100));
            addSetting(new TestbedSetting(VelocityIterations, SettingType.ENGINE, 8, 1, 100));
            addSetting(new TestbedSetting(AllowSleep, SettingType.ENGINE, true));
            addSetting(new TestbedSetting(WarmStarting, SettingType.ENGINE, true));
            addSetting(new TestbedSetting(ContinuousCollision, SettingType.ENGINE, true));
            addSetting(new TestbedSetting(SubStepping, SettingType.ENGINE, false));
            addSetting(new TestbedSetting(DrawShapes, SettingType.DRAWING, true));
            addSetting(new TestbedSetting(DrawJoints, SettingType.DRAWING, true));
            addSetting(new TestbedSetting(DrawAABBs, SettingType.DRAWING, false));
            addSetting(new TestbedSetting(DrawContactPoints, SettingType.DRAWING, false));
            addSetting(new TestbedSetting(DrawContactNormals, SettingType.DRAWING, false));
            addSetting(new TestbedSetting(DrawContactImpulses, SettingType.DRAWING, false));
            addSetting(new TestbedSetting(DrawFrictionImpulses, SettingType.DRAWING, false));
            addSetting(new TestbedSetting(DrawCOMs, SettingType.DRAWING, false));
            addSetting(new TestbedSetting(DrawStats, SettingType.DRAWING, true));
            addSetting(new TestbedSetting(DrawHelp, SettingType.DRAWING, false));
            addSetting(new TestbedSetting(DrawTree, SettingType.DRAWING, false));
            addSetting(new TestbedSetting(DrawWireframe, SettingType.DRAWING, true));
        }

        /**
         * Adds a settings to the settings list
         * @param argSetting
         */
        public void addSetting(TestbedSetting argSetting)
        {
            if (settingsMap.ContainsKey(argSetting.name))
            {
                throw new ArgumentException("Settings already contain a setting with name: "
                    + argSetting.name);
            }
            settings.Add(argSetting);
            settingsMap.Add(argSetting.name, argSetting);
        }

        /**
         * Returns an unmodifiable list of settings
         * @return
         */
        public IReadOnlyCollection<TestbedSetting> getSettings()
        {
            return settings;
        }

        /**
         * Gets a setting by name.
         * @param argName
         * @return
         */
        public TestbedSetting getSetting(string argName)
        {
            return settingsMap[argName];
        }
    }
}