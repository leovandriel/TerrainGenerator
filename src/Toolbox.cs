//==================================================================================================
//
// TerrainGenerator v.1 - To generate (natural looking) height maps of terrain.
//
// Copyright (C) 2008  Leo Vandriel
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
//==================================================================================================

using System;

namespace TerrainGenerator.Toolbox
{

    // frame rate stats, for overall linear time movement
    public static class FrameTime
    {
        private static DateTime lastTime;

        private static float frameTime = -1;
        private static float frameRate = 0;
        private static float avgFrameTime = 0;
        private static float avgFrameTimeVar = 0;
        private static float avgFrameRate = 0;
        private static float avgFrameRateVar = 0;

        public static float Time
        {
            get { return frameTime; }
        }

        public static float AverageTime
        {
            get { return avgFrameTime; }
        }

        public static float AverageTimeVar
        {
            get { return Toolbox.Math.Sqrt(avgFrameTimeVar); }
        }

        public static float Rate
        {
            get { return frameRate; }
        }

        public static float AverageRate
        {
            get { return avgFrameRate; }
        }

        public static float AverageRateVar
        {
            get { return Toolbox.Math.Sqrt(avgFrameRateVar); }
        }

        public static void NextFrame()
        {
            if (frameTime == -1)
            {
                frameTime = 0;
            }
            else
            {
                TimeSpan span = DateTime.Now.Subtract(lastTime);
                frameTime = .001f * span.Milliseconds;

                if(frameTime != 0)
                    frameRate = 1 / frameTime;

                avgFrameTime = avgFrameTime * .9f + frameTime * .1f;
                avgFrameTimeVar = avgFrameTimeVar * .9f + (frameTime - avgFrameTime) * (frameTime - avgFrameTime) * .1f;


                avgFrameRate = avgFrameRate * .9f + frameRate * .1f;
                avgFrameRateVar = avgFrameRateVar * .9f + (frameRate - avgFrameRate) * (frameRate - avgFrameRate) * .1f;

            }

            lastTime = DateTime.Now;
        }
    }

    // easy random number generator (wrapper)
    public static class Random
    {
        private static System.Random random = null;

        private static void CheckInit()
        {
            if (random == null)
                random = new System.Random();
        }

        public static void SetSeed(int seed)
        {
            random = new System.Random(seed);
        }

        public static double Double()
        {
            CheckInit();
            return random.NextDouble();
        }

        public static float Float()
        {
            CheckInit();
            return (float)random.NextDouble();
        }

        public static int Int(int upper)
        {
            CheckInit();
            return random.Next(upper);
        }

        public static int Int(int lower, int upper)
        {
            CheckInit();
            return random.Next(lower, upper);
        }

        public static double Double(double lower, double upper)
        {
            CheckInit();
            return lower + (upper - lower) * random.NextDouble();
        }

        public static float Float(float lower, float upper)
        {
            CheckInit();
            return lower + (upper - lower) * (float)random.NextDouble();
        }
    }

    // common maths in float (wrapper)
    public static class Math
    {
        public static float PI
        {
            get { return (float)System.Math.PI; }
        }

        public static float Sqrt(float f)
        {
            return (float)System.Math.Sqrt(f);
        }

        public static float Tan(float f)
        {
            return (float)System.Math.Tan(f);
        }

        public static float Atan(float f)
        {
            return (float)System.Math.Atan(f);
        }

        public static float Log(float f)
        {
            return (float)System.Math.Log(f);
        }
    }

}
