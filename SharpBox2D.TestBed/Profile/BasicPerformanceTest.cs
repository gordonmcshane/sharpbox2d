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
 * Created at 8:12:11 AM Jan 18, 2011
 */


/**
 * @author Daniel Murphy
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using SharpBox2D.Common;

namespace SharpBox2D.TestBed.Profile
{
    public class ResultFormat
    {
        public static ResultFormat MILLISECONDS = new ResultFormat(1000000, "Milliseconds");
        public static ResultFormat MICROSECONDS = new ResultFormat(1000, "Microseconds");
        public static ResultFormat NANOSECONDS = new ResultFormat(1, "Nanoseconds");

        public  int divisor;
        public  string name;

        internal ResultFormat(int divisor, string name) {
            Debug.Assert (divisor != 0);
            this.divisor = divisor;
            this.name = name;
        }
    }

    public abstract class BasicPerformanceTest {

        static Random _random = new Random();
    
        public static void Shuffle<T>(List<T> array)
        {
            var random = _random;
            for (int i = array.Count; i > 1; i--)
            {
                // Pick random element to swap.
                int j = random.Next(i); // 0 <= j <= i-1
                // Swap.
                T tmp = array[j];
                array[j] = array[i - 1];
                array[i - 1] = tmp;
            }
        }

        private ResultFormat format = ResultFormat.MICROSECONDS;
        private  int numTests, iters, frames;
        internal  DescriptiveStatistics[] stats;
        private List<int> testOrder = new List<int>();

        public BasicPerformanceTest(int numTests, int iters, int frames) {
            this.numTests = numTests;
            this.iters = iters;
            this.frames = frames;
            stats = new DescriptiveStatistics[numTests];
            for (int i = 0; i < numTests; i++) {
                stats[i] = new DescriptiveStatistics(iters * frames + 1);
                testOrder.Add(i);
            }
        }
  
        public void setFormat(ResultFormat format) {
            this.format = format;
        }

        public void go() {
            long prev, after;
            // warmup
            println("Warmup");
            int warmupIters = iters / 10;
            for (int i = 0; i < warmupIters; i++) {
                println(i * 100.0 / warmupIters + "%");
                Shuffle(testOrder);
                for (int test = 0; test < numTests; test++) {
                    setupTest(test);
                }
                for (int j = 0; j < frames; j++) {
                    Shuffle(testOrder);
                    for (int test = 0; test < numTests; test++) {
                        int runningTest = testOrder[test];
                        preStep(runningTest);
                        step(runningTest);
                    }
                }
            }
            println("Testing");
            for (int i = 0; i < iters; i++) {
                println(i * 100.0 / iters + "%");
                for (int test = 0; test < numTests; test++) {
                    setupTest(test);
                }
                for (int j = 0; j < frames; j++) {
                    Shuffle(testOrder);
                    for (int test = 0; test < numTests; test++) {
                        int runningTest = testOrder[test];
                        preStep(runningTest);
                        prev = Stopwatch.GetTimestamp();
                        step(runningTest);
                        after = Stopwatch.GetTimestamp();
                        stats[runningTest].addValue((after - prev));
                    }
                }
            }
            printResults();
        }

        public void printResults() {
            printf("%-20s%20s%20s%20s\n", "Test Name", format.name + " Avg", "StdDev", "95% Interval");
            for (int i = 0; i < numTests; i++) {
                double mean = stats[i].getMean() / format.divisor;
                double stddev = stats[i].getStandardDeviation() / format.divisor;
                double diff = 1.96 * stddev / MathUtils.sqrt(stats[i].getN());
                printf("%-20s%20.3f%20.3f  (%7.3f,%7.3f)\n", getTestName(i), mean, stddev, mean - diff, mean
                                                                                                        + diff);
            }
        }

        public virtual void setupTest(int testNum) {}

        public virtual void preStep(int testNum) { }

        public abstract void step(int testNum);

        public abstract string getTestName(int testNum);

        public virtual int getFrames(int testNum)
        {
            return 0;
        }

        // override to change output
        public virtual void println(string s)
        {
            Console.WriteLine(s);
        }

        public virtual void printf(string s, params object[] args)
        {
            Console.WriteLine(string.Format(s, args));
        }
    }
}