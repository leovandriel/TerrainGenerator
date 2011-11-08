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

namespace TerrainGenerator
{

    // Contains the settings of the terrain viewer. This class is used to communicate in between
    // threads of settings window and rendering. Settings are only applied in between frames.
    public class TerrainSettings
    {
        private int SETTING_COUNT = 20;
        private int ACTION_COUNT = 10;

        public enum Setting
        {
            SampleDepth,
            RotateSpeed,
            LightMode,
            FillMode,
            DNA0,
            DNA1,
            DNA2,
            DNA3,
            DNA4,
            DNA5,
            DNA6,
            DNA7,
            DNA8,
            DNA9
        };

        public enum Action
        {
            UpSample,
            DownSample,
            Generate,
            RandomizeLight,
            RandomizeFill
        }

        private bool[] actions;
        private bool[] bools;
        private int[] ints;
        private float[] floats;

        public TerrainSettings()
        {
            actions = new bool[ACTION_COUNT];
            bools = new bool[SETTING_COUNT];
            ints = new int[SETTING_COUNT];
            floats = new float[SETTING_COUNT];

            // initial settings
            Set(Setting.DNA0, .6f);
            Set(Setting.DNA1, .6f);
            Set(Setting.DNA2, .6f);
            Set(Setting.DNA3, .6f);
            Set(Setting.DNA4, .6f);
            Set(Setting.DNA5, .6f);
            Set(Setting.DNA6, .6f);
            Set(Setting.DNA7, .6f);
            Set(Setting.DNA8, .6f);
            Set(Setting.DNA9, .6f);
            Set(Setting.FillMode, 0);
            Set(Setting.LightMode, 0);
            Set(Setting.RotateSpeed, 0f);
            Set(Setting.SampleDepth, 2);
        }

        public string TypeOf(Setting s)
        {
            switch (s)
            {
                case Setting.DNA0: return "float";
                case Setting.DNA1: return "float";
                case Setting.DNA2: return "float";
                case Setting.DNA3: return "float";
                case Setting.DNA4: return "float";
                case Setting.DNA5: return "float";
                case Setting.DNA6: return "float";
                case Setting.DNA7: return "float";
                case Setting.DNA8: return "float";
                case Setting.DNA9: return "float";
                case Setting.SampleDepth: return "int";
                case Setting.RotateSpeed: return "float";
                case Setting.LightMode: return "int";
                case Setting.FillMode: return "int";
            }
            return null;
        }

        public void DoAction(Action a)
        {
            actions[a.GetHashCode()] = true;
        }

        public void Set(Setting s, bool value)
        {
            if (!TypeOf(s).Equals("bool"))
                throw new ArgumentException("Setting "+s+" requires type "+TypeOf(s));

            bools[s.GetHashCode()] = value;
        }

        public void Set(Setting s, int value)
        {
            if (!TypeOf(s).Equals("int"))
                throw new ArgumentException("Setting " + s + " requires type " + TypeOf(s));

            ints[s.GetHashCode()] = value;
        }

        public void Set(Setting s, float value)
        {
            if (!TypeOf(s).Equals("float"))
                throw new ArgumentException("Setting " + s + " requires type " + TypeOf(s));

            floats[s.GetHashCode()] = value;
        }

        public void CopyFrom(TerrainSettings setting)
        {
            for (int i = 0; i < actions.Length; i++)
            {
                actions[i] = setting.actions[i];
                setting.actions[i] = false;
            }

            for (int i = 0; i < bools.Length; i++)
                bools[i] = setting.bools[i];

            for (int i = 0; i < ints.Length; i++)
                ints[i] = setting.ints[i];

            for (int i = 0; i < floats.Length; i++)
                floats[i] = setting.floats[i];
        }

        public bool GetAction(Action a)
        {
            if (actions[a.GetHashCode()])
            {
                actions[a.GetHashCode()] = false;
                return true;
            }
            return false;
        }

        public bool GetBool(Setting s)
        {
            if (!TypeOf(s).Equals("bool"))
                throw new ArgumentException("Setting " + s + " requires type " + TypeOf(s));

            return bools[s.GetHashCode()];
        }

        public int GetInt(Setting s)
        {
            if (!TypeOf(s).Equals("int"))
                throw new ArgumentException("Setting " + s + " requires type " + TypeOf(s));

            return ints[s.GetHashCode()];
        }

        public float GetFloat(Setting s)
        {
            if (!TypeOf(s).Equals("float"))
                throw new ArgumentException("Setting " + s + " requires type " + TypeOf(s));

            return floats[s.GetHashCode()];
        }
    }
}
