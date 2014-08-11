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
using System.Diagnostics;
using System.IO;
using System.Threading;
using SharpBox2D.Common;
using SharpBox2D.Dynamics;

namespace SharpBox2D.TestBed.Framework
{

    public enum UpdateBehavior
    {
        UPDATE_CALLED,
        UPDATE_IGNORED
    }

    public enum MouseBehavior
    {
        NORMAL,
        FORCE_Y_FLIP
    }

/**
 * This class contains most control logic for the testbed and the update loop. It also watches the
 * model to switch tests and populates the model with some loop statistics.
 * 
 * @author Daniel Murphy
 */

    public class TestbedController : TestbedModel.TestChangedListener
    {


        public static readonly int DEFAULT_FPS = 60;

        private TestbedTest _currTest = null;
        private TestbedTest _nextTest = null;

        private long startTime;
        private long frameCount;
        private int targetFrameRate;
        private float frameRate = 0;
        private bool animating = false;
        //private Thread animator;

        private TestbedModel model;

        private bool savePending, loadPending, resetPending = false;

        private UpdateBehavior updateBehavior;
        private MouseBehavior mouseBehavior;

        private LinkedList<QueueItem> inputQueue;
        private TestbedErrorHandler errorHandler;

        private float viewportHalfHeight;
        private float viewportHalfWidth;
        private object _lock = new object();

        public TestbedController(TestbedModel argModel, UpdateBehavior behavior,
            MouseBehavior mouseBehavior, TestbedErrorHandler errorHandler)
        {
            model = argModel;
            inputQueue = new LinkedList<QueueItem>();
            setFrameRate(DEFAULT_FPS);
            //animator = new Thread(s => run());
            updateBehavior = behavior;
            this.errorHandler = errorHandler;
            this.mouseBehavior = mouseBehavior;
            addListeners();
        }

        public void testChanged(TestbedTest test, int index)
        {
            model.getPanel().grabFocus();
            _nextTest = test;
        }

        private void addListeners()
        {
            // time for our controlling
            model.addTestChangeListener(this);

        }

        public void load()
        {
            loadPending = true;
        }

        public void save()
        {
            savePending = true;
        }

        public void reset()
        {
            resetPending = true;
        }

        public void queueLaunchBomb()
        {
            lock (inputQueue)
            {
                inputQueue.AddLast(new QueueItem());
            }
        }

        public void queuePause()
        {
            lock (inputQueue)
            {
                inputQueue.AddLast(new QueueItem(QueueItemType.Pause));
            }
        }

        public void queueMouseUp(Vec2 screenPos, int button)
        {
            lock (inputQueue)
            {
                inputQueue.AddLast(new QueueItem(QueueItemType.MouseUp, screenPos, button));
            }
        }

        public void queueMouseDown(Vec2 screenPos, int button)
        {
            lock (inputQueue)
            {
                inputQueue.AddLast(new QueueItem(QueueItemType.MouseDown, screenPos, button));
            }
        }

        public void queueMouseMove(Vec2 screenPos)
        {
            lock (inputQueue)
            {
                inputQueue.AddLast(new QueueItem(QueueItemType.MouseMove, screenPos, 0));
            }
        }

        public void queueMouseDrag(Vec2 screenPos, int button)
        {
            lock (inputQueue)
            {
                inputQueue.AddLast(new QueueItem(QueueItemType.MouseDrag, screenPos, button));
            }
        }

        public void queueKeyPressed(char c, int code)
        {
            lock (inputQueue)
            {
                inputQueue.AddLast(new QueueItem(QueueItemType.KeyPressed, c, code));
            }
        }

        public void queueKeyReleased(char c, int code)
        {
            lock (inputQueue)
            {
                inputQueue.AddLast(new QueueItem(QueueItemType.KeyReleased, c, code));
            }
        }

        public void updateExtents(float halfWidth, float halfHeight)
        {
            viewportHalfHeight = halfHeight;
            viewportHalfWidth = halfWidth;

            if (_currTest != null)
            {
                _currTest.getCamera().getTransform().setExtents(halfWidth, halfHeight);
            }
        }

        protected void loopInit()
        {
            model.getPanel().grabFocus();

            if (_currTest != null)
            {
                _currTest.init(model);
            }
        }

        private void initTest(TestbedTest test)
        {
            test.init(model);
            test.getCamera().getTransform().setExtents(viewportHalfWidth, viewportHalfHeight);
            model.getPanel().grabFocus();
        }

        /**
   * Called by the main run loop. If the update behavior is set to
   * {@link UpdateBehavior#UPDATE_IGNORED}, then this needs to be called manually to update the input
   * and test.
   */

        public void updateTest()
        {
            if (resetPending)
            {
                if (_currTest != null)
                {
                    _currTest.init(model);
                }
                resetPending = false;
                model.getPanel().grabFocus();
            }
            if (savePending)
            {
                if (_currTest != null)
                {
                   // _save();
                }
                savePending = false;
                model.getPanel().grabFocus();
            }
            if (loadPending)
            {
                if (_currTest != null)
                {
                    //_load();
                }
                loadPending = false;
                model.getPanel().grabFocus();
            }

            if (_currTest == null)
            {
                lock (inputQueue)
                {
                    inputQueue.Clear();
                    return;
                }
            }
            IViewportTransform transform = _currTest.getCamera().getTransform();
            // process our input
            while (inputQueue.Count != 0)
            {
                QueueItem i = null;
                lock (inputQueue)
                {
                    if (inputQueue.Count != 0)
                    {
                        i = inputQueue.First.Value;
                        inputQueue.RemoveFirst();
                    }
                }
                if (i == null)
                {
                    continue;
                }
                bool oldFlip = transform.isYFlip();
                if (mouseBehavior == MouseBehavior.FORCE_Y_FLIP)
                {
                    transform.setYFlip(true);
                }
                _currTest.getCamera().getTransform().getScreenToWorld(i.p, i.p);
                if (mouseBehavior == MouseBehavior.FORCE_Y_FLIP)
                {
                    transform.setYFlip(oldFlip);
                }
                switch (i.type)
                {
                    case QueueItemType.KeyPressed:

                        model.getKeys()[i.c] = true;

                        model.getCodedKeys()[i.code] = true;
                        _currTest.keyPressed(i.c, i.code);
                        break;
                    case QueueItemType.KeyReleased:

                        model.getKeys()[i.c] = false;

                        model.getCodedKeys()[i.code] = false;
                        _currTest.keyReleased(i.c, i.code);
                        break;
                    case QueueItemType.MouseDown:
                        _currTest.mouseDown(i.p, i.button);
                        break;
                    case QueueItemType.MouseMove:
                        _currTest.mouseMove(i.p);
                        break;
                    case QueueItemType.MouseUp:
                        _currTest.mouseUp(i.p, i.button);
                        break;
                    case QueueItemType.MouseDrag:
                        _currTest.mouseDrag(i.p, i.button);
                        break;
                    case QueueItemType.LaunchBomb:
                        _currTest.lanchBomb();
                        break;
                    case QueueItemType.Pause:
                        model.getSettings().pause = !model.getSettings().pause;
                        break;
                }
            }

            if (_currTest != null)
            {
                _currTest.step(model.getSettings());
            }
        }

        public void nextTest()
        {
            int index = model.getCurrTestIndex() + 1;
            index %= model.getTestsSize();

            while (!model.isTestAt(index) && index < model.getTestsSize() - 1)
            {
                index++;
            }
            if (model.isTestAt(index))
            {
                model.setCurrTestIndex(index);
            }
        }

        public void lastTest()
        {
            int index = model.getCurrTestIndex() - 1;

            while (index >= 0 && !model.isTestAt(index))
            {
                if (index == 0)
                {
                    index = model.getTestsSize() - 1;
                }
                else
                {
                    index--;
                }
            }

            if (model.isTestAt(index))
            {
                model.setCurrTestIndex(index);
            }
        }

        public void playTest(int argIndex)
        {
            if (argIndex == -1)
            {
                return;
            }
            while (!model.isTestAt(argIndex))
            {
                if (argIndex + 1 < model.getTestsSize())
                {
                    argIndex++;
                }
                else
                {
                    return;
                }
            }
            model.setCurrTestIndex(argIndex);
        }

        public void setFrameRate(int fps)
        {
            if (fps <= 0)
            {
                throw new ArgumentException("Fps cannot be less than or equal to zero");
            }
            targetFrameRate = fps;
            frameRate = fps;
        }

        public int getFrameRate()
        {
            return targetFrameRate;
        }

        public float getCalculatedFrameRate()
        {
            return frameRate;
        }

        public long getStartTime()
        {
            return startTime;
        }

        public long getFrameCount()
        {
            return frameCount;
        }

        public bool isAnimating()
        {
            return animating;
        }

        public void start()
        {
            lock (_lock)
            {
                if (animating != true)
                {
                    beforeTime = startTime = updateTime = Stopwatch.GetTimestamp();

                    loopInit();
                    animating = true;
                    frameCount = 0;
                }
                else
                {
                    Debug.WriteLine("Animation is already animating.");
                }
            }
        }

        private void stop()
        {
            lock (_lock)
            {
                animating = false;
            }
        }


        long beforeTime, afterTime, updateTime, timeDiff, sleepTime, timeSpent;
            float timeInSecs;
            

        public void Update()
        {
            if (animating)
            {
                if (_nextTest != null)
                {
                    initTest(_nextTest);
                    model.setRunningTest(_nextTest);
                    if (_currTest != null)
                    {
                        _currTest.exit();
                    }
                    _currTest = _nextTest;
                    _nextTest = null;
                }

                timeSpent = beforeTime - updateTime;
                if (timeSpent > 0)
                {
                    timeInSecs = timeSpent * 1.0f / 1000000000.0f;
                    updateTime = Stopwatch.GetTimestamp();
                    frameRate = (frameRate * 0.9f) + (1.0f / timeInSecs) * 0.1f;
                    model.setCalculatedFps(frameRate);
                }
                else
                {
                    updateTime = Stopwatch.GetTimestamp();
                }

                if (_currTest != null && updateBehavior == UpdateBehavior.UPDATE_CALLED)
                {
                    updateTest();
                }


                frameCount++;

                afterTime = Stopwatch.GetTimestamp();

                timeDiff = afterTime - beforeTime;
                sleepTime = (1000000000 / targetFrameRate - timeDiff) / 1000000;
                //if (sleepTime > 0)
                //{
                //    Thread.Sleep(TimeSpan.FromMilliseconds(sleepTime));
                //}

                beforeTime = Stopwatch.GetTimestamp();
            }
        }

        public void Draw()
        {
            if (animating)
            {
                
            }
        }

        //public void run()
        //{
            
        //    sleepTime = 0;

        //    animating = true;
        //    loopInit();
        //    while (animating)
        //    {

        //        if (_nextTest != null)
        //        {
        //            initTest(_nextTest);
        //            model.setRunningTest(_nextTest);
        //            if (_currTest != null)
        //            {
        //                _currTest.exit();
        //            }
        //            _currTest = _nextTest;
        //            _nextTest = null;
        //        }

        //        timeSpent = beforeTime - updateTime;
        //        if (timeSpent > 0)
        //        {
        //            timeInSecs = timeSpent*1.0f/1000000000.0f;
        //            updateTime = Stopwatch.GetTimestamp();
        //            frameRate = (frameRate*0.9f) + (1.0f/timeInSecs)*0.1f;
        //            model.setCalculatedFps(frameRate);
        //        }
        //        else
        //        {
        //            updateTime = Stopwatch.GetTimestamp();
        //        }
        //        TestbedPanel panel = model.getPanel();

        //        if (panel.render())
        //        {
        //            if (_currTest != null && updateBehavior == UpdateBehavior.UPDATE_CALLED)
        //            {
        //                updateTest();
        //            }
        //            panel.paintScreen();
        //        }
        //        frameCount++;

        //        afterTime = Stopwatch.GetTimestamp();

        //        timeDiff = afterTime - beforeTime;
        //        sleepTime = (1000000000/targetFrameRate - timeDiff)/1000000;
        //        if (sleepTime > 0)
        //        {
        //            Thread.Sleep(TimeSpan.FromMilliseconds(sleepTime));
        //        }

        //        beforeTime = Stopwatch.GetTimestamp();
        //    } // end of run loop
        //}


        //private void _save() {
        //  SerializationResult result;
        //  try {
        //    result = _currTest.getSerializer().serialize(_currTest.getWorld());
        //  } catch (UnsupportedObjectException e1) {
        //    log.error("Error serializing world", e1);
        //    if (errorHandler != null)
        //      errorHandler.serializationError(e1, "Error serializing the object: " + e1.ToString());
        //    return;
        //  }

        //  try {
        //    FileOutputStream fos = new FileOutputStream(_currTest.getFilename());
        //    result.writeTo(fos);
        //    fos.flush();
        //    fos.close();
        //  } catch (FileNotFoundException e) {
        //    log.error("File not found exception while saving", e);
        //    if (errorHandler != null)
        //      errorHandler.serializationError(e,
        //          "File not found exception while saving: " + _currTest.getFilename());
        //  } catch (IOException e) {
        //    log.error("Exception while writing world", e);
        //    if (errorHandler != null)
        //      errorHandler.serializationError(e, "Error while writing world: " + e.ToString());
        //  }
        //  log.debug("Serialed world to " + _currTest.getFilename());
        //}

        //private void _load() {
        //  World w;
        //  try {
        //    FileInputStream fis = new FileInputStream(_currTest.getFilename());
        //    w = _currTest.getDeserializer().deserializeWorld(fis);
        //    fis.close();
        //  } catch (FileNotFoundException e) {
        //    log.error("File not found error while loading", e);
        //    if (errorHandler != null)
        //      errorHandler.serializationError(e,
        //          "File not found exception while loading: " + _currTest.getFilename());
        //    return;
        //  } catch (UnsupportedObjectException e) {
        //    log.error("Error deserializing object", e);
        //    if (errorHandler != null)
        //      errorHandler.serializationError(e, "Error deserializing the object: " + e.ToString());
        //    return;
        //  } catch (IOException e) {
        //    log.error("Exception while reading world", e);
        //    if (errorHandler != null)
        //      errorHandler.serializationError(e, "Error while reading world: " + e.ToString());
        //    return;
        //  }
        //  log.debug("Deserialized world from " + _currTest.getFilename());

        //  _currTest.init(w, true);
        //}
    }


    internal enum QueueItemType
    {
        MouseDown,
        MouseMove,
        MouseUp,
        MouseDrag,
        KeyPressed,
        KeyReleased,
        LaunchBomb,
        Pause
    }


    internal class QueueItem
    {
        public QueueItemType type;
        public Vec2 p = new Vec2();
        public char c;
        public int button;
        public int code;

        public QueueItem()
        {
            type = QueueItemType.LaunchBomb;
        }

        public QueueItem(QueueItemType t)
        {
            type = t;
        }

        public QueueItem(QueueItemType t, Vec2 pt, int button)
        {
            type = t;
            p.set(pt);
            this.button = button;
        }

        public QueueItem(QueueItemType t, char cr, int cd)
        {
            type = t;
            c = cr;
            code = cd;
        }
    }
}
