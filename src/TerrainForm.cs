using System;
using System.Drawing;
using System.Windows.Forms;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectInput;

namespace TerrainGenerator
{
    // the setting window
    public class TerrainForm : System.Windows.Forms.Form
    {
        #region Private Members

        private TerrainSettings terrainSettings = null;

        // parameter: The size of the mountain
        private static float SIZE = 50;

        // screen size in pix
        private static int SCREEN_SIZE = 800;

        // the rendering device
        private Microsoft.DirectX.Direct3D.Device device;
        private System.ComponentModel.Container components = null;

        // settings
        private float[] DNA = null;
        private float rotateSpeed = 0f;
        private int levelOfDetail = 4;
        private int lightMode = 0;
        private int fillMode = 0;

        // scene data
        private HeightMap heightMap = null;
        private float angle = 0;
        private SettingsForm controlPanel = null;
        private float timeToDisplayStats = 0f;
        private Light directionalLight = null;
        private Light pointLight1 = null;
        private Light pointLight2 = null;
        private Texture[] textureArray = null;
        private Texture currentTexture = null;
        private Material textureMaterial;
        private Material colorMaterial;
        private Material wireframeMaterial;
        
        // vertex data
        private Mesh mesh;

        // camera
        private Camera camera;

        // input
        private Microsoft.DirectX.DirectInput.Device keyboard;
        private System.Drawing.Point lastMousePosition = new System.Drawing.Point();
        private bool mouseDown = false;

        #endregion

        #region ConstructDestruct: TerrainForm(), Dispose(bool disposing)

        // constructor
        public TerrainForm()
        {
            this.components = new System.ComponentModel.Container();
            this.Size = new System.Drawing.Size(SCREEN_SIZE, SCREEN_SIZE);
            this.Text = "Terrain Viewer";
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);
            this.Icon = TerrainGenerator.Properties.Resources.Ico;
            this.CenterToScreen();

            terrainSettings = new TerrainSettings();
            heightMap = new HeightMap();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #endregion

        #region Initialization

        public void Initialize()
        {
            Initialize(null);
        }

        public void Initialize(SettingsForm form)
        {
            // initialize devices
            InitDevice();
            InitKeyboard();

            // initialize graphics
            InitCamera();
            InitLights();            
            InitMaterial();

            // read settings
            controlPanel = form;
            form.Refresh();
            LoadSettings();

            // generate initial height map
            Generate();
        }

        public void ReInitialize()
        {            
            ReInitCamera();
            ReInitLights();
            ReInitMaterial();
            LoadMesh();
        }

        // set up device
        public void InitDevice()
        {
            // set the device parameters
            PresentParameters presentParams = new PresentParameters();
            presentParams.Windowed = true;
            presentParams.SwapEffect = SwapEffect.Discard;

            // Z-buffering
            presentParams.EnableAutoDepthStencil = true;
            presentParams.AutoDepthStencilFormat = DepthFormat.D16;

            // create DirectX device
            device = new Microsoft.DirectX.Direct3D.Device(
                0, 
                Microsoft.DirectX.Direct3D.DeviceType.Hardware, 
                this, 
                CreateFlags.SoftwareVertexProcessing, presentParams);

            // called every time we call device.Reset()
            device.DeviceReset += new EventHandler(this.OnDeviceReset);

            // DeviceLost gets called earlier for cleaning
            device.DeviceLost += new EventHandler(this.OnDeviceLost);
        }

        // set up keyboard
        public void InitKeyboard()
        {
            keyboard = new Microsoft.DirectX.DirectInput.Device(SystemGuid.Keyboard);
            keyboard.SetCooperativeLevel(
                this, 
                CooperativeLevelFlags.Background | CooperativeLevelFlags.NonExclusive);
            keyboard.Acquire();
        }

        // init camera
        private void InitCamera()
        {
            camera = new Camera(device);

            camera.Projection = 
                Matrix.PerspectiveFovLH(Toolbox.Math.PI / 4, this.Width / this.Height, 1f, 1000f);
            camera.SetLookAtLH(
                new Vector3(0f, 100f, 100f), 
                new Vector3(0f, -1f, -1), 
                new Vector3(0f, 1f, 0f));

            SetFillMode(fillMode);
            device.RenderState.CullMode = Cull.None;

            device.RenderState.Ambient = Color.FromArgb(0x40, 0x40, 0x40);
        }

        // init camera
        private void ReInitCamera()
        {
            camera.UpdateProjectionMatrix();
            camera.UpdateViewMatrix();

            SetFillMode(fillMode);
            device.RenderState.CullMode = Cull.None;

            device.RenderState.Ambient = Color.FromArgb(0x40, 0x40, 0x40);
        }


        // load the three available lights
        private void InitLights()
        {
            directionalLight = new Light();
            pointLight1 = new Light();
            pointLight2 = new Light();

            RandomizeLights();
            SetLightMode(-1);
        }

        private void ReInitLights()
        {
            SetLightMode(-1);
        }

        private void InitMaterial()
        {
            // load textures
            System.Collections.Generic.List<Texture> list = 
                new System.Collections.Generic.List<Texture>();

            // max 16 jpg textures
            for (int i = 0; i < 16; i++ )
            {
                try
                {
                    Texture t = TextureLoader.FromFile(device, "texture" + i + ".jpg");
                    list.Add(t);
                }
                catch { }
            }

            if (list.Count == 0)
                textureArray = new Texture[] { null };
            else
                textureArray = list.ToArray();

            device.RenderState.SpecularEnable = true;
            device.RenderState.ShadeMode = ShadeMode.Phong;

            // generate fills
            RandomizeFill();
            SetFillMode(-1);
        }

        private void ReInitMaterial()
        {
            SetFillMode(-1);
        }

        #endregion

        #region Settings: LoadSettings(), SetFillMode(int mode), SetLightMode(int mode)

        private void LoadSettings()
        {
            if (controlPanel != null)
            {
                terrainSettings.CopyFrom(controlPanel.GetTerrain());
            }
            else
            {
                if (terrainSettings == null)
                    terrainSettings = new TerrainSettings();
            }


            DNA = new float[7];
            DNA[0] = terrainSettings.GetFloat(TerrainSettings.Setting.DNA0);
            DNA[1] = terrainSettings.GetFloat(TerrainSettings.Setting.DNA1);
            DNA[2] = terrainSettings.GetFloat(TerrainSettings.Setting.DNA2);
            DNA[3] = terrainSettings.GetFloat(TerrainSettings.Setting.DNA3);
            DNA[4] = terrainSettings.GetFloat(TerrainSettings.Setting.DNA4);
            DNA[5] = terrainSettings.GetFloat(TerrainSettings.Setting.DNA5);
            DNA[6] = terrainSettings.GetFloat(TerrainSettings.Setting.DNA6);

            int tempFillMode = terrainSettings.GetInt(TerrainSettings.Setting.FillMode);
            if (fillMode != tempFillMode)
                SetFillMode(tempFillMode);

            int tempLightMode = terrainSettings.GetInt(TerrainSettings.Setting.LightMode);
            if (lightMode != tempLightMode)
                SetLightMode(tempLightMode);

            rotateSpeed = terrainSettings.GetFloat(TerrainSettings.Setting.RotateSpeed);

            levelOfDetail = terrainSettings.GetInt(TerrainSettings.Setting.SampleDepth);


            if (terrainSettings.GetAction(TerrainSettings.Action.Generate))
                Generate();

            if (terrainSettings.GetAction(TerrainSettings.Action.UpSample))
            {
                if (CanUpsample())
                {
                    heightMap.UpSample(DNA);
                    heightMap.CalcVerticesIndices(SIZE);
                    LoadMesh();
                }
            }

            if (terrainSettings.GetAction(TerrainSettings.Action.DownSample))
            {
                heightMap.DownSample();
                heightMap.CalcVerticesIndices(SIZE);
                LoadMesh();
            }

            if (terrainSettings.GetAction(TerrainSettings.Action.RandomizeLight))
            {
                RandomizeLights();
                SetLightMode(-1);
            }

            if (terrainSettings.GetAction(TerrainSettings.Action.RandomizeFill))
            {
                RandomizeFill();
                SetFillMode(-1);
            }
        }

        private void SetFillMode(int mode)
        {
            if(mode >= 0)
                fillMode = mode % 3;
            
            switch (fillMode)
            {
                case 0:
                    {
                        device.Material = colorMaterial;
                        device.SetTexture(0, null);
                        device.RenderState.FillMode = FillMode.Solid; 
                    } break;
                case 1:
                    {
                        device.Material = textureMaterial;
                        device.SetTexture(0, currentTexture);
                        device.RenderState.FillMode = FillMode.Solid;
                    } break;
                case 2:
                    {
                        device.Material = wireframeMaterial;
                        device.SetTexture(0, null);
                        device.RenderState.FillMode = FillMode.WireFrame;
                    } break;
            }            
        }

        private void SetLightMode(int mode)
        {
            if (mode >= 0)
                lightMode = mode % 3;

            switch (lightMode)
            {
                case 0:
                    {
                        device.RenderState.Lighting = true;
                        device.Lights[0].FromLight(directionalLight);
                        device.Lights[0].Enabled = true;
                        device.Lights[1].Enabled = false;
                    } break;
                case 1:
                    {
                        device.RenderState.Lighting = true;
                        device.Lights[0].FromLight(pointLight1);
                        device.Lights[0].Enabled = true;
                        device.Lights[1].Enabled = false;
                    } break;
                case 2:
                    {
                        device.RenderState.Lighting = true;
                        device.Lights[0].FromLight(pointLight1);
                        device.Lights[1].FromLight(pointLight2);
                        device.Lights[0].Enabled = true;
                        device.Lights[1].Enabled = true;
                    } break;
            }
        }

        #endregion

        #region TerrainGeneration: Generate(), CanUpsample(), LoadMesh(), RandomizeLights(), RandomizeFill()

        // generate a new terrain
        private void Generate()
        {
            // generate terrain by up-sampling
            heightMap.SetMap(new float[,] { { 0f, 0f }, { 0f, 0f } });
            for (int i = 0; i < levelOfDetail; i++)
            {
                if (CanUpsample())
                {
                    heightMap.UpSample(DNA);
                }
            }
            heightMap.CalcVerticesIndices(SIZE);
            LoadMesh();
        }

        // try new mesh size (this way is rather ugly)
        private bool CanUpsample()
        {
            int newWidth = heightMap.Width*2-1;
            int newHeight = heightMap.Height*2-1;

            int newVerticesLength = newWidth * newHeight;
            int newIndicesLength = (newWidth-1) * (newHeight-1) * 6;

            Mesh newMesh = null;

            try
            {
                newMesh = new Mesh(newIndicesLength / 3, newVerticesLength, 
                    0, CustomVertex.PositionNormalTextured.Format, device);
            }
            catch
            {
                return false;
            }

            newMesh.Dispose();
            return true;
        }

        // loads the heightMap into the vertex and index buffer
        private void LoadMesh()
        {
            if (mesh != null)
                mesh.Dispose();

            mesh = new Mesh(heightMap.IndicesCount / 3, heightMap.VerticesCount, 
                0, CustomVertex.PositionNormalTextured.Format, device);

            mesh.VertexBuffer.SetData(heightMap.Vertices, 0, LockFlags.None);
            mesh.IndexBuffer.SetData(heightMap.Indices, 0, LockFlags.None);
            int[] adjacency = new int[mesh.NumberFaces * 3];

            mesh.GenerateAdjacency(0.01F, adjacency);
            mesh.OptimizeInPlace(MeshFlags.OptimizeVertexCache, adjacency);
        }

        private void RandomizeLights()
        {
            float x, z;

            x = Toolbox.Random.Float(-1, 1);
            z = Toolbox.Random.Float(-1, 1);
            device.Lights[0].Type = LightType.Directional;
            directionalLight.Direction = new Vector3(x, -Toolbox.Math.Sqrt(2 - x * x - z * z), z);
            directionalLight.Diffuse = Color.White;

            x = Toolbox.Random.Float(-40, 40);
            z = Toolbox.Random.Float(-40, 40);
            pointLight1.Type = LightType.Point;
            pointLight1.Range = 1000f;
            pointLight1.Attenuation1 = Toolbox.Random.Float(.05f, .009f);
            pointLight1.Position = new Vector3(x, Toolbox.Math.Sqrt(3200 - x * x - z * z), z);
            pointLight1.Diffuse = Color.White;

            x = Toolbox.Random.Float(-70, 70);
            z = Toolbox.Random.Float(-70, 70);
            pointLight2.Type = LightType.Point;
            pointLight2.Range = 1000f;
            pointLight2.Attenuation1 = Toolbox.Random.Float(.05f, .009f);
            pointLight2.Position = new Vector3(x, Toolbox.Math.Sqrt(9800 - x * x - z * z), z);
            pointLight2.Diffuse = Color.White;
        }

        private void RandomizeFill()
        {
            int ambient = Toolbox.Random.Int(0x40);
            int diffuse = Toolbox.Random.Int(0x88, 0xFF);

            colorMaterial.Ambient = Color.FromArgb(ambient, ambient, ambient);
            colorMaterial.Diffuse = Color.FromArgb(Toolbox.Random.Int(0x88, 0xFF), 
                Toolbox.Random.Int(0x88, 0xFF), Toolbox.Random.Int(0x88, 0xFF));
            colorMaterial.SpecularSharpness = Toolbox.Random.Float(1f, 20f);

            textureMaterial.Ambient = Color.FromArgb(ambient, ambient, ambient);
            textureMaterial.Diffuse = Color.FromArgb(diffuse, diffuse, diffuse);
            textureMaterial.SpecularSharpness = Toolbox.Random.Float(20f, 50f);

            wireframeMaterial.Ambient = Color.FromArgb(0xFF, 0xFF, 0xFF);
            wireframeMaterial.Diffuse = Color.FromArgb(0, 0, 0);

            currentTexture = textureArray[Toolbox.Random.Int(textureArray.Length)];
        }

        #endregion

        #region Rendering: Render(), PrintStats()

        private void Render()
        {
            // check new settings
            LoadSettings();

            // print stats
            PrintStats();

            // check for camera movement
            HandleInput();

            // keep up the frame time
            Toolbox.FrameTime.NextFrame();

            // rotate the landscape
            angle += rotateSpeed * Toolbox.FrameTime.Time;

            // clear device
            device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);

            // render scene
            device.BeginScene();
             
            device.Transform.World = 
                Matrix.Translation(-SIZE / 2, 0, -SIZE / 2) * Matrix.RotationY(angle);

            int numSubSets = mesh.GetAttributeTable().Length;
            for (int i = 0; i < numSubSets; ++i)
                mesh.DrawSubset(0);

            device.EndScene();

            // show scene
            device.Present();

            this.Invalidate();
        }

        // display stats about the scene
        private void PrintStats()
        {
            if (controlPanel == null)
                return;

            timeToDisplayStats -= Toolbox.FrameTime.Time;

            if (timeToDisplayStats < 0)
            {
                controlPanel.SetFramerate(""+(int)Toolbox.FrameTime.AverageRate);
                controlPanel.SetTriangles(heightMap.IndicesCount / 3);
                controlPanel.SetVariance((int)(heightMap.Variance *SIZE*SIZE* 100) / 100f);

                int width = heightMap.Width * 2 - 1;
                int height = heightMap.Height * 2 - 1;

                int level = -2;
                for (int size = width > height ? height : width; size > 0; size = size >> 1)
                    level++;

                controlPanel.SetDetailLevel(level);
                controlPanel.SetDimension(width, height);

                timeToDisplayStats = .5f;
            }
        }

        #endregion

        #region MouseKeyInput: HandleInput()

        private void HandleInput()
        {
            // Handle mouse
            if (mouseDown)
            {
                int x = Cursor.Position.X;
                int y = Cursor.Position.Y;

                x -= lastMousePosition.X;
                y -= lastMousePosition.Y;

                lastMousePosition = Cursor.Position;

                camera.Rotate(x, y);
            }

            // Handle keys

            KeyboardState keys = keyboard.GetCurrentKeyboardState();

            Vector3 move = new Vector3();
            if (keys[Key.UpArrow])
                move += new Vector3(0f, 0f, 1f);
            if (keys[Key.DownArrow])
                move += new Vector3(0f, 0f, -1f);
            if (keys[Key.LeftArrow])
                move += new Vector3(1f, 0f, 0f);
            if (keys[Key.RightArrow])
                move += new Vector3(-1f, 0f, 0f);
            if (keys[Key.PageUp])
                move += new Vector3(0f, 1f, 0f);
            if (keys[Key.PageDown])
                move += new Vector3(0f, -1f, 0f);
          
            if(this.Focused)
                camera.Move(move);
        }

        #endregion

        #region EventListeners: OnPaint, OnMouseDown, OnMouseUp, OnDeviceLost, OnDeviceReset

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            Render();
        }

        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            mouseDown = true;
            lastMousePosition = Cursor.Position;
        }

        protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            mouseDown = false;
        }

        protected void OnDeviceReset(object sender, EventArgs e)
        {
            ReInitialize();
        }

        protected void OnDeviceLost(object sender, EventArgs e)
        {
            
            mesh.Dispose();
        }

        #endregion

        static void Main()
        {
            // show control panel
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            SettingsForm controlPanel = new SettingsForm();
            controlPanel.Show();

            // start terrain viewer
            TerrainForm form = new TerrainForm();
            form.Initialize(controlPanel);
            form.Show();
            try
            {
                Application.Run(form);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Application exception. \n\n" + ex);
                return;
            }
        }
    }
}