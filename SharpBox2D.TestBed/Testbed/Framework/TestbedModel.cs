/*******************************************************************************
 * Copyright (c) 2013, Daniel Murphy
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without modification,
 * are permitted provided that the following conditions are met:
 *  * Redistributions of source code must retain the above copyright notice,
 *    this list of conditions and the following disclaimer.
 *  * Redistributions in binary form must reproduce the above copyright notice,
 *    this list of conditions and the following disclaimer in the documentation
 *    and/or other materials provided with the distribution.
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

using System;
using System.Collections.Generic;
using SharpBox2D.Callbacks;
using SharpBox2D.Common;



namespace SharpBox2D.TestBed.Framework
{

    public class DefaultComboBoxModel : List<TestbedModel.ListItem>
    {
    }

/**
 * Model for the testbed
 * 
 * @author Daniel
 */

    public class TestbedModel
    {
        private DefaultComboBoxModel tests = new DefaultComboBoxModel();
        private TestbedSettings settings = new TestbedSettings();
        private DebugDraw draw;
        private TestbedTest test;
        private List<TestChangedListener> listeners = new List<TestChangedListener>();
        private bool[] keys = new bool[512];
        private bool[] codedKeys = new bool[512];
        private float calculatedFps;
        private int currTestIndex = -1;
        private TestbedTest runningTest;
        private List<string> implSpecificHelp;
        private TestbedPanel panel;
        private WorldCreator worldCreator = new DefaultWorldCreator();

        public WorldCreator getWorldCreator()
        {
            return worldCreator;
        }

        public void setWorldCreator(WorldCreator worldCreator)
        {
            this.worldCreator = worldCreator;
        }

        public void setPanel(TestbedPanel panel)
        {
            this.panel = panel;
        }

        public TestbedPanel getPanel()
        {
            return panel;
        }

        public void setImplSpecificHelp(List<string> implSpecificHelp)
        {
            this.implSpecificHelp = implSpecificHelp;
        }

        public List<string> getImplSpecificHelp()
        {
            return implSpecificHelp;
        }

        public void setCalculatedFps(float calculatedFps)
        {
            this.calculatedFps = calculatedFps;
        }

        public float getCalculatedFps()
        {
            return calculatedFps;
        }

        public void setViewportTransform(IViewportTransform transform)
        {
            draw.setViewportTransform(transform);
        }

        public void setDebugDraw(DebugDraw argDraw)
        {
            draw = argDraw;
        }

        public DebugDraw getDebugDraw()
        {
            return draw;
        }

        public TestbedTest getCurrTest()
        {
            return test;
        }

        /**
   * Gets the array of keys, index corresponding to the char value.
   * 
   * @return
   */

        public bool[] getKeys()
        {
            return keys;
        }

        /**
   * Gets the array of coded keys, index corresponding to the coded key value.
   * 
   * @return
   */

        public bool[] getCodedKeys()
        {
            return codedKeys;
        }

        public void setCurrTestIndex(int argCurrTestIndex)
        {
            if (argCurrTestIndex < 0 || argCurrTestIndex >= tests.Count)
            {
                throw new ArgumentException("Invalid test index");
            }
            if (currTestIndex == argCurrTestIndex)
            {
                return;
            }

            if (!isTestAt(argCurrTestIndex))
            {
                throw new ArgumentException("No test at " + argCurrTestIndex);
            }
            currTestIndex = argCurrTestIndex;
            ListItem item = (ListItem) tests[argCurrTestIndex];
            test = item.test;
            foreach (TestChangedListener listener in listeners)
            {
                listener.testChanged(test, currTestIndex);
            }
        }

        public int getCurrTestIndex()
        {
            return currTestIndex;
        }

        public void setRunningTest(TestbedTest runningTest)
        {
            this.runningTest = runningTest;
        }

        public TestbedTest getRunningTest()
        {
            return runningTest;
        }

        public void addTestChangeListener(TestChangedListener argListener)
        {
            listeners.Add(argListener);
        }

        public void removeTestChangeListener(TestChangedListener argListener)
        {
            listeners.Remove(argListener);
        }

        public void addTest(TestbedTest argTest)
        {
            tests.Add(new ListItem(argTest));
        }

        public void addCategory(string argName)
        {
            tests.Add(new ListItem(argName));
        }

        public TestbedTest getTestAt(int argIndex)
        {
            ListItem item = (ListItem) tests[argIndex];
            if (item.isCategory())
            {
                return null;
            }
            return item.test;
        }

        public bool isTestAt(int argIndex)
        {
            ListItem item = (ListItem) tests[argIndex];
            return !item.isCategory();
        }

        public void clearTestList()
        {
            tests.Clear();
        }

        public int getTestsSize()
        {
            return tests.Count;
        }

        public DefaultComboBoxModel getComboModel()
        {
            return tests;
        }

        public TestbedSettings getSettings()
        {
            return settings;
        }

        public class ListItem
        {
            public string category;
            public TestbedTest test;

            public ListItem(string argCategory)
            {
                category = argCategory;
            }

            public ListItem(TestbedTest argTest)
            {
                test = argTest;
            }

            public bool isCategory()
            {
                return category != null;
            }

            public override string ToString()
            {
                return isCategory() ? category : test.getTestName();
            }
        }

        public interface TestChangedListener
        {
            void testChanged(TestbedTest test, int index);
        }
    }
}