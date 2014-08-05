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

using System;
using System.Diagnostics;
using SharpBox2D.Common;

namespace SharpBox2D.TestBed.Tests.Math
{
    public class SinCosTest
    {
        // formating stuff
        public static int COLUMN_PADDING = 3;
        public static int NUM_DECIMALS = 8;

        public static int numTables = 50;
        // accuracy
        public static float mostPreciseTable = .00001f;
        public static float leastPreciseTable = .01f;
        public static int accuracyIterations = 100000;

        // speed
        public static int speedTrials = 20;
        public static int speedIterations = 50000;

        private static SinCosTable[] tables;

        private static Random _random = new Random();

        /**
   * @param args
   */

        public static void main(string[] args)
        {
            int overall = 1;
            try
            {
                numTables = int.Parse(args[0]);
                mostPreciseTable = float.Parse(args[1]);
                leastPreciseTable = float.Parse(args[2]);
                accuracyIterations = int.Parse(args[3]);
                speedTrials = int.Parse(args[4]);
                speedIterations = int.Parse(args[5]);
                overall = int.Parse(args[6]);
            }
            catch (Exception)
            {
                Console.WriteLine("Parameters: <number of tables to use> <most precise table value (smallest)> "
                                  +
                                  "<least precise table value> <number of accuracy test iterations> <number of speed test trials>"
                                  + "<number of speed test iterations> <number of overall speed test sets>");
                Console.WriteLine("Sample parameters: 200 .00001 .01 100000 20 5000 2");
                // return;
            }
            Console.WriteLine("Tables: " + numTables);
            Console.WriteLine("Most Precise Table: " + mostPreciseTable);
            Console.WriteLine("Least Precise Table: " + leastPreciseTable);
            Console.WriteLine("Accuracy Iterations: " + accuracyIterations);
            Console.WriteLine("Speed Trials: " + speedTrials);
            Console.WriteLine("Speed Iterations: " + speedIterations);

            constructTables();
            doAccuracyTest(true);
            for (int i = 0; i < overall; i++)
            {
                doSpeedTest(true);
                System.Threading.Thread.Sleep(1000);

            }
        }

        /**
   * constructs the tables from the static parameters
   */

        public static void constructTables()
        {
            tables = new SinCosTable[numTables];

            Console.WriteLine("constructing tables");
            for (int i = 0; i < numTables; i++)
            {
                // well... basic lerp
                float precision = i*1f/numTables*(leastPreciseTable - mostPreciseTable)
                                  + mostPreciseTable;
                tables[i] = new SinCosTable(precision);
            }
        }

        /**
   * accuracy test from the static parameters, the tables array needs to be constructed as well,
   * returns double[tables][0-3 (no lerp, lerp, then the difference)]
   * 
   * @return
   */

        public static double[][] doAccuracyTest(bool print)
        {

            Console.WriteLine("doing accuracy tests");

            double[][] accuracyResults = new double[numTables][];

            for (int i = 0; i < accuracyResults.Length; i++)
            {
                accuracyResults[i] = new double[3];
            }

            SinCosTable.LERP_LOOKUP = false;
            // without lerp
            for (int i = 0; i < numTables; i++)
            {
                accuracyResults[i][0] = accuracyTest(tables[i], accuracyIterations);
            }

            SinCosTable.LERP_LOOKUP = true;
            // with lerp
            for (int i = 0; i < numTables; i++)
            {
                accuracyResults[i][1] = accuracyTest(tables[i], accuracyIterations);
            }

            for (int i = 0; i < numTables; i++)
            {
                accuracyResults[i][2] = accuracyResults[i][0] - accuracyResults[i][1];
            }

            if (print)
            {
                Console.WriteLine("Accuracy results, average displacement");
                string[] header = {"Not lerped", "Lerped", "Difference"};
                string[] side = new string[numTables + 1];
                side[0] = "Table precision";
                for (int i = 0; i < tables.Length; i++)
                {
                    side[i + 1] = formatDecimal(tables[i].precision, NUM_DECIMALS);
                }
                printTable(header, side, accuracyResults);
            }
            return accuracyResults;
        }

        /**
   * speed test from the static parameters the tables array needs to be constructed as well, returns
   * double[tables][0-3 (no lerp, lerp, then the difference)]
   * 
   * @return
   */

        public static double[][] doSpeedTest(bool print)
        {
            Console.WriteLine("\nDoing speed tests");
            double[][] speedResults = new double[numTables][];

            for (int i = 0; i < speedResults.Length; i++)
            {
                speedResults[i] = new double[4];
            }

            SinCosTable.LERP_LOOKUP = false;
            // without lerp
            for (int i = 0; i < numTables; i++)
            {
                speedResults[i][0] = speedTest(tables[i], speedIterations, speedTrials);
            }

            SinCosTable.LERP_LOOKUP = true;
            // with lerp
            for (int i = 0; i < numTables; i++)
            {
                speedResults[i][1] = speedTest(tables[i], speedIterations, speedTrials);
            }

            // with the Math calls
            for (int i = 0; i < numTables; i++)
            {
                speedResults[i][3] = speedTestMath(speedIterations, speedTrials);
            }

            for (int i = 0; i < numTables; i++)
            {
                speedResults[i][2] = speedResults[i][0] - speedResults[i][1];
            }

            if (print)
            {
                Console.WriteLine("Speed results, in iterations per second (higher number means faster)");
                string[] header = {"Not lerped", "Lerped", "Difference", "Java Math"};
                string[] side = new string[numTables + 1];
                side[0] = "Table precision";
                for (int i = 0; i < tables.Length; i++)
                {
                    side[i + 1] = formatDecimal(tables[i].precision, NUM_DECIMALS);
                }
                printTable(header, side, speedResults);
            }
            return speedResults;
        }

        private static double accuracyTest(SinCosTable table, int iterations)
        {
            double totalDiff = 0f, diff = 0f;

            for (int i = 0; i < iterations; i++)
            {
                float querry = (float) _random.NextDouble()*MathUtils.TWOPI;
                diff = MathUtils.abs((float) System.Math.Sin(querry) - table.sin(querry));
                totalDiff += diff;
            }
            totalDiff /= iterations;
            return totalDiff;
        }

        private static void printTable(string[] header, string[] side, double[][] results)
        {

            // first determine the amount of space we need for each column
            int[] colLengths = new int[results[0].Length + 1];
            for (int i = 0; i < colLengths.Length; i++)
            {
                colLengths[i] = 0;
            }
            for (int j = -1; j < results[0].Length; j++)
            {
                if (j == -1)
                {
                    int colLength = side[j + 1].Length + COLUMN_PADDING;
                    if (colLength > colLengths[j + 1])
                    {
                        colLengths[j + 1] = colLength;
                    }
                }
                else
                {
                    int colLength = header[j].Length + COLUMN_PADDING;
                    if (colLength > colLengths[j + 1])
                    {
                        colLengths[j + 1] = colLength;
                    }
                    for (int i = 0; i < results.Length; i++)
                    {
                        colLength = (formatDecimal(results[i][j], NUM_DECIMALS)).Length + COLUMN_PADDING;
                        if (colLength > colLengths[j + 1])
                        {
                            colLengths[j + 1] = colLength;
                        }
                    }
                }

            }

            // header

            Console.WriteLine(spacestring(side[0], colLengths[0]));
            for (int i = 1; i < colLengths.Length; i++)
            {
                Console.Write(spacestring(header[i - 1], colLengths[i]));
            }
            Console.WriteLine();

            for (int i = 0; i < results.Length; i++)
            {

                for (int j = -1; j < results[i].Length; j++)
                {
                    if (j == -1)
                    {
                        Console.Write(spacestring(side[i + 1], colLengths[j + 1]));
                    }
                    else
                    {
                        string toPrint = formatDecimal(results[i][j], NUM_DECIMALS);
                        Console.Write(spacestring(toPrint, colLengths[j + 1]));
                    }
                }
                Console.WriteLine();
            }
        }

        private static long speedTest(SinCosTable table, int numIterations, int numTrials)
        {
            long startTime, endTime;
            long totalTime = 0;
            float i, j;
            float k = 0;

            float jstep = MathUtils.TWOPI/numIterations;

            for (i = 0; i < numTrials; i++)
            {

                startTime = Stopwatch.GetTimestamp();
                for (j = 0; j < MathUtils.TWOPI; j += jstep)
                {
                    k += table.sin(j);
                }
                endTime = Stopwatch.GetTimestamp();
                totalTime += endTime - startTime;
            }
            i += k;

            return numIterations*numTrials*1000000000L/(totalTime);
        }

        private static long speedTestMath(int numIterations, int numTrials)
        {
            long startTime, endTime;
            long totalTime = 0;
            float i, j;
            float k = 0;

            float jstep = MathUtils.TWOPI/numIterations;

            for (i = 0; i < numTrials; i++)
            {

                startTime = Stopwatch.GetTimestamp();
                for (j = 0; j < MathUtils.TWOPI; j += jstep)
                {
                    k += (float) System.Math.Sin(j);
                }
                endTime = Stopwatch.GetTimestamp();
                totalTime += endTime - startTime;
            }
            i += k;

            return numIterations*numTrials*1000000000L/(totalTime);
        }

        private static string spacestring(string str, int space)
        {
            // if the string is more than the space
            if (str.Length == space)
            {
                return str;
            }
            else if (str.Length >= space)
            {
                return str.Substring(0, space);
            }
            string s = str;

            for (int i = s.Length; i < space; i++)
            {
                s = " " + s;
            }
            return s;
        }

        private static string formatDecimal(double n, int decimals)
        {
            string num = n + "";
            // no decimal
            if (num.IndexOf(".") == -1)
            {
                return num;
            }

            bool ePresent = false;
            string e = null;

            if (num.IndexOf("E") != -1)
            {
                e = num.Substring(num.IndexOf("E"));
                decimals -= e.Length;
                num = num.Substring(0, num.IndexOf("E"));
                ePresent = true;
            }

            int decLen = num.Substring(num.IndexOf(".") + 1).Length;
            int numLen = num.Substring(0, num.IndexOf(".")).Length;

            // if not enough decimals
            if (decLen < decimals)
            {
                for (int i = 0; i < (decimals - decLen); i++)
                {
                    num = num + " ";
                }
            }
            else if (decLen > decimals)
            {
                // more decimals than needed
                num = num.Substring(0, numLen + decimals + 1);
            }
            if (ePresent)
            {
                num += e;
            }
            return num;
        }
    }
}
