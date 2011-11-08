using System;
using System.Collections.Generic;
using System.ComponentModel;
//using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TerrainGenerator
{
    public partial class SettingsForm : Form
    {
       private TerrainSettings terrain;
        
        public SettingsForm()
        {
            InitializeComponent();
            terrain = new TerrainSettings();
            trackBar1.Value = terrain.GetInt(TerrainSettings.Setting.SampleDepth);
            trackBar1_Scroll(null, null);
            vScrollBar1.Value = (int)((1 - terrain.GetFloat(TerrainSettings.Setting.DNA0)) * 100);
            vScrollBar2.Value = (int)((1 - terrain.GetFloat(TerrainSettings.Setting.DNA1)) * 100);
            vScrollBar3.Value = (int)((1 - terrain.GetFloat(TerrainSettings.Setting.DNA2)) * 100);
            vScrollBar4.Value = (int)((1 - terrain.GetFloat(TerrainSettings.Setting.DNA3)) * 100);
            vScrollBar5.Value = (int)((1 - terrain.GetFloat(TerrainSettings.Setting.DNA4)) * 100);
            vScrollBar6.Value = (int)((1 - terrain.GetFloat(TerrainSettings.Setting.DNA5)) * 100);
            vScrollBar7.Value = (int)((1 - terrain.GetFloat(TerrainSettings.Setting.DNA6)) * 100);
            label2.Text = terrain.GetInt(TerrainSettings.Setting.SampleDepth).ToString();
            trackBar2.Value = (int) (terrain.GetFloat(TerrainSettings.Setting.RotateSpeed) * 2);
            comboBox1.SelectedIndex = terrain.GetInt(TerrainSettings.Setting.LightMode);
            comboBox2.SelectedIndex = terrain.GetInt(TerrainSettings.Setting.FillMode);
        }

        public TerrainSettings GetTerrain()
        {
            return terrain;
        }

        public void SetFramerate(string framerate)
        {
            label3.Text = framerate;
        }

        public void SetTriangles(int triangles)
        {
            label5.Text = triangles.ToString();
        }

        public void SetVariance(float variance)
        {
            label7.Text = variance.ToString();
        }

        public void SetDetailLevel(int level)
        {
            label9.Text = level.ToString();
        }

        public void SetDimension(int width, int height)
        {
            label11.Text = width +"x"+height;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            label2.Text = trackBar1.Value.ToString();

            switch(trackBar1.Value)
            {

                case 0:
                    vScrollBar1.Visible = false;
                    vScrollBar2.Visible = false;
                    vScrollBar3.Visible = false;
                    vScrollBar4.Visible = false;
                    vScrollBar5.Visible = false;
                    vScrollBar6.Visible = false;
                    vScrollBar7.Visible = false;
                    button1.Visible = false;
                    break;
                
                case 1:
                    vScrollBar1.Visible = true;
                    vScrollBar2.Visible = false;
                    vScrollBar3.Visible = false;
                    vScrollBar4.Visible = false;
                    vScrollBar5.Visible = false;
                    vScrollBar6.Visible = false;
                    vScrollBar7.Visible = false;
                    button1.Visible = true;
                    break; 
                 
                case 2:
                    vScrollBar1.Visible = true;
                    vScrollBar2.Visible = true;
                    vScrollBar3.Visible = false;
                    vScrollBar4.Visible = false;
                    vScrollBar5.Visible = false;
                    vScrollBar6.Visible = false;
                    vScrollBar7.Visible = false;
                    button1.Visible = true;
                    break;  
                  
                case 3:
                    vScrollBar1.Visible = true;
                    vScrollBar2.Visible = true;
                    vScrollBar3.Visible = true;
                    vScrollBar4.Visible = false;
                    vScrollBar5.Visible = false;
                    vScrollBar6.Visible = false;
                    vScrollBar7.Visible = false;
                    button1.Visible = true;
                    break;

                case 4:
                    vScrollBar1.Visible = true;
                    vScrollBar2.Visible = true;
                    vScrollBar3.Visible = true;
                    vScrollBar4.Visible = true;
                    vScrollBar5.Visible = false;
                    vScrollBar6.Visible = false;
                    vScrollBar7.Visible = false;
                    button1.Visible = true;
                    break;

                case 5:
                    vScrollBar1.Visible = true;
                    vScrollBar2.Visible = true;
                    vScrollBar3.Visible = true;
                    vScrollBar4.Visible = true;
                    vScrollBar5.Visible = true;
                    vScrollBar6.Visible = false;
                    vScrollBar7.Visible = false;
                    button1.Visible = true;
                    break;
                   
                case 6:
                    vScrollBar1.Visible = true;
                    vScrollBar2.Visible = true;
                    vScrollBar3.Visible = true;
                    vScrollBar4.Visible = true;
                    vScrollBar5.Visible = true;
                    vScrollBar6.Visible = true;
                    vScrollBar7.Visible = false;
                    button1.Visible = true;
                    break;
                
                case 7:
                    vScrollBar1.Visible = true;
                    vScrollBar2.Visible = true;
                    vScrollBar3.Visible = true;
                    vScrollBar4.Visible = true;
                    vScrollBar5.Visible = true;
                    vScrollBar6.Visible = true;
                    vScrollBar7.Visible = true;
                    button1.Visible = true;
                    break;
                
                default:
                    break;
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Random RandomClass = new Random();
            vScrollBar1.Value = RandomClass.Next(100);
            vScrollBar2.Value = RandomClass.Next(100);
            vScrollBar3.Value = RandomClass.Next(100);
            vScrollBar4.Value = RandomClass.Next(100);
            vScrollBar5.Value = RandomClass.Next(100);
            vScrollBar6.Value = RandomClass.Next(100);
            vScrollBar7.Value = RandomClass.Next(100);
            
        }

        private void button2_Click(object sender, EventArgs e)
        {

            terrain.Set(TerrainSettings.Setting.SampleDepth, trackBar1.Value);
            terrain.Set(TerrainSettings.Setting.DNA0, (100 - vScrollBar1.Value) / (float)100);
            terrain.Set(TerrainSettings.Setting.DNA1, (100 - vScrollBar2.Value) / (float)100);
            terrain.Set(TerrainSettings.Setting.DNA2, (100 - vScrollBar3.Value) / (float)100);
            terrain.Set(TerrainSettings.Setting.DNA3, (100 - vScrollBar4.Value) / (float)100);
            terrain.Set(TerrainSettings.Setting.DNA4, (100 - vScrollBar5.Value) / (float)100);
            terrain.Set(TerrainSettings.Setting.DNA5, (100 - vScrollBar6.Value) / (float)100);
            terrain.Set(TerrainSettings.Setting.DNA6, (100 - vScrollBar7.Value) / (float)100);

            terrain.DoAction(TerrainSettings.Action.Generate);

        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            terrain.Set(TerrainSettings.Setting.RotateSpeed, (float)trackBar2.Value / 2);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    terrain.Set(TerrainSettings.Setting.LightMode, 0);
                    break;

                case 1:
                    terrain.Set(TerrainSettings.Setting.LightMode, 1);
                    break;

                case 2:
                    terrain.Set(TerrainSettings.Setting.LightMode, 2);
                    break;

                default:
                    terrain.Set(TerrainSettings.Setting.LightMode, -1);
                    break;
            }  
        }

        private void button7_Click(object sender, EventArgs e)
        {
            terrain.DoAction(TerrainSettings.Action.DownSample);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            terrain.DoAction(TerrainSettings.Action.UpSample);
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox2.SelectedIndex)
            {
                case 0:
                    terrain.Set(TerrainSettings.Setting.FillMode, 0);
                    break;

                case 1:
                    terrain.Set(TerrainSettings.Setting.FillMode, 1);
                    break;

                case 2:
                    terrain.Set(TerrainSettings.Setting.FillMode, 2);
                    break;

                default:
                    terrain.Set(TerrainSettings.Setting.FillMode, -1);
                    break;

            }  
        }

        private void button3_Click(object sender, EventArgs e)
        {
            terrain.DoAction(TerrainSettings.Action.RandomizeLight);
        }

        private void groupBox7_Enter(object sender, EventArgs e)
        {

        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void splitContainer2_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }

        private void groupBox6_Enter(object sender, EventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            terrain.DoAction(TerrainSettings.Action.RandomizeFill);
        }

    }
}