using System;
using System.Collections;
using SharpGL;
using SharpGL.SceneGraph.Primitives;
using SharpGL.SceneGraph.Quadrics;
using SharpGL.Enumerations;
using SharpGL.SceneGraph.Core;
using System.Diagnostics;
using AssimpSample;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Threading;
using System.Windows.Input;
using System.Windows;

namespace Transformations
{
    ///<summary> Klasa koja enkapsulira OpenGL programski kod </summary>
    ///
    class World
    {
        #region Atributi

        /// <summary>
        ///	 Ugao rotacije sveta oko X ose
        /// </summary>
        private float m_xRotation = 0.0f;

        /// <summary>
        ///	 Ugao rotacije sveta oko Y ose
        /// </summary>
        private float m_yRotation = 0.0f;
        private float m_zRotation = 0.0f;

        private float m_eyeX = 0.0f;
        private float m_eyeY = 0.0f;
        private float m_eyeZ = 0.0f;

        private float m_centerX = 0.0f;
        private float m_centerY = 2.5f;
        private float m_centerZ = -1.0f;

        private float m_upX = 0.0f;
        private float m_upY = 1.0f;
        private float m_upZ = 0.0f;

        private AssimpScene m_scene1;
        private AssimpScene m_scene2;
        private float m_sceneDistance = 7000.0f;

        /// <summary>
        ///	 Sirina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_width = 0;

        /// <summary>
        ///	 Visina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_height = 0;

        /// <summary>
        ///	 Mreza za iscrtavanje
        /// </summary>
        Grid grid;

        /// <summary>
        ///	 Mreza za iscrtavanje
        /// </summary>
        Cube cube;
        Cylinder cylinderLamp;
        Sphere sphereLamp;

        /// <summary>
        ///   Odabrani tip projekcije.
        /// </summary>
        //private ProjectionType m_currentProjectionType = ProjectionType.Perspective;

        /// <summary>
        ///   Trenutna udaljenost objekta po z-osi;
        /// </summary>
        private float m_zDistance;

        private enum TextureObjects { Grass = 0, Road, Metal };
        private readonly int m_textureCount = Enum.GetNames(typeof(TextureObjects)).Length;
        private uint[] m_textures = null;
        private string[] m_textureFiles = { "..//..//images//grass.jpg", "..//..//images//road.jpg", "..//..//images//metal.jpg" };
        private int rotateRight = 0;
        private int translateLeft = 0;
        private double scaleCar = 1;

        DispatcherTimer timer;
        int count = 0;

        private float rightAnimation = 0;
        private float leftAnimation = 0;
        private float mx_temp;


        #endregion

        #region Properties

        public float TempMX {
            get { return mx_temp; }
            set { mx_temp = value; }
        }
        /// <summary>
        ///	 Sirina OpenGL kontrole u pikselima.
        /// </summary>
        public int Width {
            get { return m_width; }
            set { m_width = value; }
        }

        /// <summary>
        ///	 Visina OpenGL kontrole u pikselima.
        /// </summary>
        public int Height {
            get { return m_height; }
            set { m_height = value; }
        }

        /// <summary>
        ///	 Ugao rotacije sveta oko Y ose.
        /// </summary>
        public float RotationY {
            get { return m_yRotation; }
            set { m_yRotation = value; }
        }

        /// <summary>
        ///	 Ugao rotacije sveta oko X ose.
        /// </summary>
        public float RotationX {
            get { return m_xRotation; }
            set { m_xRotation = value; }
        }

        public float RotationZ {
            get { return m_zRotation; }
            set { m_zRotation = value; }
        }

        /// <summary>
        ///	 X pozicija kamere.
        /// </summary>
        public float EyeX {
            get { return m_eyeX; }
            set { m_eyeX = value; }
        }

        /// <summary>
        ///	 Y pozicija kamere.
        /// </summary>
        public float EyeY {
            get { return m_eyeY; }
            set { m_eyeY = value; }
        }

        /// <summary>
        ///	 Z pozicija kamere.
        /// </summary>
        public float EyeZ {
            get { return m_eyeZ; }
            set { m_eyeZ = value; }
        }

        /// <summary>
        ///   Odabrani tip projekcije.
        /// </summary>
        /*public ProjectionType CurrentProjectionType
        {
            get { return m_currentProjectionType; }
            set { m_currentProjectionType = value; }
        }*/

        public AssimpScene Scene1 {
            get { return m_scene1; }
            set { m_scene1 = value; }
        }

        public AssimpScene Scene2 {
            get { return m_scene2; }
            set { m_scene2 = value; }
        }

        public DispatcherTimer Timer {
            get { return timer; }
            set { timer = value; }
        }

        /// <summary>
        ///   Trenutna udaljenost objekta po z-osi;
        /// </summary>
        public float ZDistance {
            get { return m_zDistance; }
            set { m_zDistance = value; }
        }

        public float SceneDistance {
            get { return m_sceneDistance; }
            set { m_sceneDistance = value; }
        }

        public int TranslateLeft {
            get { return translateLeft; }
            set { translateLeft = value; }
        }

        public int RotateRight {
            get { return rotateRight; }
            set { rotateRight = value; }
        }

        public double ScaleCar {
            get { return scaleCar; }
            set { scaleCar = value; }
        }

        #endregion

        #region Konstruktori

        /// <summary>
        ///		Konstruktor opengl sveta.
        /// </summary>
        public World(String scenePath1, String sceneFileName1, String scenePath2, String sceneFileName2, int width, int height, OpenGL gl)
        {
            this.m_scene1 = new AssimpScene(scenePath1, sceneFileName1, gl);
            this.m_scene2 = new AssimpScene(scenePath2, sceneFileName2, gl);
            this.m_width = width;
            this.m_height = height;
            m_textures = new uint[m_textureCount];
        }

        #endregion

        #region Metode

        /// <summary>
        /// Korisnicka inicijalizacija i podesavanje OpenGL parametara
        /// </summary>
        public void Initialize(OpenGL gl)
        {
            gl.LookAt(m_eyeX, m_eyeY, m_eyeZ, m_centerX, m_centerY, m_centerZ, m_upX, m_upY, m_upZ);
            m_zDistance = -200;
            gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f); // podesi boju za brisanje ekrana na crnu
            grid = new Grid( );
            cube = new Cube( );
            //gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            //gl.Color(1f, 0f, 0f);

            gl.ShadeModel(OpenGL.GL_FLAT);
            gl.Enable(OpenGL.GL_DEPTH_TEST);
            gl.Enable(OpenGL.GL_CULL_FACE);
            gl.CullFace(OpenGL.GL_FRONT);
            gl.FrontFace(OpenGL.GL_CW);


            m_scene1.LoadScene( );
            m_scene1.Initialize( );
            m_scene2.LoadScene( );
            m_scene2.Initialize( );

            //osvetljenje
            SetupLighting(gl);

            //teksture
            setupTextures(gl);

            cylinderLamp = new Cylinder( );
            cylinderLamp.CreateInContext(gl);
            cylinderLamp.Material = new SharpGL.SceneGraph.Assets.Material( );
            cylinderLamp.Material.Ambient = Color.Gray;

            sphereLamp = new Sphere( );
            sphereLamp.CreateInContext(gl);
            sphereLamp.Radius = 3f;
            sphereLamp.Material = new SharpGL.SceneGraph.Assets.Material( );
            sphereLamp.Material.Emission = Color.Yellow;

            timer = new DispatcherTimer( );
            timer.Interval = TimeSpan.FromMilliseconds(20);
            timer.Tick += new EventHandler(UpdateAnimation);
        }

        private void UpdateAnimation(object sender, EventArgs e)
        {
            count += 1;
            if (count < 75) {
                if(count < 40) {
                    leftAnimation += 1.5f;
                    rightAnimation += 1f;
                } else {
                    leftAnimation += 1.5f;
                    rightAnimation += 3f;
                }
                if (count < 30) {
                    ZDistance += 3f;
                } else {
                    if(count < 60) {
                        ZDistance -= 3f;
                    }
                    if (m_xRotation < 68) {
                        m_xRotation += 2f;
                    }
                }
            } else {
                timer.Stop( );
                count = 0;
                rightAnimation = 0;
                leftAnimation = 0;
                m_xRotation = mx_temp;
                MainWindow mainWindow = Application.Current.Windows[0] as MainWindow;
                mainWindow.translateLeftTextBox.IsEnabled = true;
                mainWindow.rotateRightTextBox.IsEnabled = true;
                mainWindow.scaleCarTextBox.IsEnabled = true;
                Keyboard.Focus(mainWindow.translateLeftTextBox);
            }

        }

        private void setupTextures(OpenGL gl)
        {
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_ADD);

            gl.GenTextures(m_textureCount, m_textures);
            for (int i = 0; i < m_textureCount; ++i) {
                // Pridruzi teksturu odgovarajucem identifikatoru
                gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[i]);

                // Ucitaj sliku i podesi parametre teksture
                Bitmap image = new Bitmap(m_textureFiles[i]);
                // rotiramo sliku zbog koordinantog sistema opengl-a
                image.RotateFlip(RotateFlipType.RotateNoneFlipY);
                Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
                // RGBA format (dozvoljena providnost slike tj. alfa kanal)
                BitmapData imageData = image.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly,
                                                      System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                gl.Build2DMipmaps(OpenGL.GL_TEXTURE_2D, (int)OpenGL.GL_RGBA8, image.Width, image.Height, OpenGL.GL_BGRA, OpenGL.GL_UNSIGNED_BYTE, imageData.Scan0);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_LINEAR);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_S, OpenGL.GL_REPEAT);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_T, OpenGL.GL_REPEAT);
                gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_MODULATE);

                image.UnlockBits(imageData);
                image.Dispose( );
            }
        }

        private void SetupLighting(OpenGL gl)
        {
            gl.Enable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_COLOR_MATERIAL);
            gl.Enable(OpenGL.GL_NORMALIZE);
            gl.ColorMaterial(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT_AND_DIFFUSE);

            //light 0
            float[] light0pos = new float[] { 0.0f, 0.0f, 200.0f, 1.0f };
            float[] light0ambient = new float[] { 0.0f, 0.0f, 0.0f, 1.0f };
            float[] light0diffuse = new float[] { 0.6f, 0.6f, 0.6f, 1.0f };
            float[] light0specular = new float[] { 0.8f, 0.8f, 0.8f, 0.8f };

            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, light0pos);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT, light0ambient);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_DIFFUSE, light0diffuse);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPECULAR, light0specular);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPOT_CUTOFF, 90.0f);

            gl.Enable(OpenGL.GL_LIGHT0);

            //light 1
            float[] light1pos = new float[] { 0.0f,0.0f, 0.0f, 1.0f };
            float[] light1ambient = new float[] { 0.4f, 0.4f, 0.4f, 1.0f };
            float[] light1diffuse = new float[] { 0.8f, 0.8f, 0.3f, 1.0f };
            float[] light1specular = new float[] { 1.0f, 1.0f, 0.0f, 0.0f };

            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_POSITION, light1pos);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_AMBIENT, light1ambient);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_DIFFUSE, light1diffuse);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPECULAR, light1specular);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPOT_CUTOFF, 45.0f);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPOT_EXPONENT, 5.0f);
            float[] direction = { 0.0f, 0.0f, -1.0f };
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPOT_DIRECTION, direction);

            gl.Enable(OpenGL.GL_LIGHT1);

            gl.ClearColor(0f, 0f, 0f, 1.0f);
        }

        /// <summary>
        /// Podesava viewport i projekciju za OpenGL kontrolu.
        /// </summary>
        public void Resize(OpenGL gl, int width, int height)
        {
            m_width = width;
            m_height = height;
            gl.Viewport(0, 0, m_width, m_height); // kreiraj viewport po celom prozoru
            gl.MatrixMode(OpenGL.GL_PROJECTION);      // selektuj Projection Matrix
            gl.LoadIdentity( );                        // resetuj Projection Matrix


            gl.Perspective(50, (float)m_width / m_height, 1, 20000);


            gl.MatrixMode(OpenGL.GL_MODELVIEW);   // selektuj ModelView Matrix
            gl.LoadIdentity( );                // resetuj ModelView Matrix

        }

        /// <summary>
        ///  Iscrtavanje OpenGL kontrole.
        /// </summary>
        public void Draw(OpenGL gl)
        {
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.Color(0.4f, 0.4f, 0.4f);
            gl.LoadIdentity( );

            gl.PushMatrix( );
            float[] light0pos = new float[] { 0.0f, 0.0f, 0.0f, 1.0f };
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, light0pos);
            gl.Translate(0.0f, 0.0f, m_zDistance);
            gl.LookAt(m_eyeX, m_eyeY, m_eyeZ, m_centerX, m_centerY, m_centerZ, m_upX, m_upY, m_upZ);
            gl.Viewport(0, 0, m_width, m_height);

            gl.PointSize(10.0f);
            gl.Rotate(m_xRotation, 1.0f, 0.0f, 0.0f);
            gl.Rotate(m_yRotation, 0.0f, 1.0f, 0.0f);
            gl.Rotate(m_zRotation, 0.0f, 0.0f, 1.0f);

            gl.Normal(0.0f, -1.0f, 0.0f);
            gl.PushMatrix( );
            //trava
            gl.Color(0.2f, 0.6f, 0.3f);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Grass]);
            gl.MatrixMode(OpenGL.GL_TEXTURE);
            gl.LoadIdentity( );
            gl.Scale(200f, 200f, 200f);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.Begin(OpenGL.GL_QUADS);
            gl.TexCoord(0.0f, 0.0f);
            gl.Vertex(-100, -100);
            gl.TexCoord(0.0f, 2.0f);
            gl.Vertex(-64, -100);
            gl.TexCoord(8.0f, 2.0f);
            gl.Vertex(-64, 100);
            gl.TexCoord(8.0f, 0.0f);
            gl.Vertex(-100, 100);
            gl.End( );
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, 0);
            //bela ivica
            gl.Color(1.0f, 1.0f, 1.0f);
            gl.Begin(OpenGL.GL_QUADS);
            gl.Vertex(-64, -100);
            gl.Vertex(-56, -100);
            gl.Vertex(-56, 100);
            gl.Vertex(-64, 100);
            gl.End( );
            //put
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Road]);
            gl.Color(0.5f, 0.5f, 0.5f);
            gl.Begin(OpenGL.GL_QUADS);
            gl.TexCoord(0.0f, 0.0f);
            gl.Vertex(-56, -100);
            gl.TexCoord(0.0f, 4.0f);
            gl.Vertex(-1, -100);
            gl.TexCoord(4.0f, 1.0f);
            gl.Vertex(-1, 100);
            gl.TexCoord(1.0f, 0.0f);
            gl.Vertex(-56, 100);
            gl.End( );
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, 0);
            //bela sredina
            gl.Color(1.0f, 1.0f, 1.0f);
            gl.Begin(OpenGL.GL_QUADS);
            gl.Vertex(-1, -100);
            gl.Vertex(1, -100);
            gl.Vertex(1, 100);
            gl.Vertex(-1, 100);
            gl.End( );
            //put
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Road]);
            gl.Color(0.5f, 0.5f, 0.5f);
            gl.Begin(OpenGL.GL_QUADS);
            gl.TexCoord(0.0f, 0.0f);
            gl.Vertex(1, -100);
            gl.TexCoord(0.0f, 1.0f);
            gl.Vertex(56, -100);
            gl.TexCoord(1.0f, 4.0f);
            gl.Vertex(56, 100);
            gl.TexCoord(4.0f, 0.0f);
            gl.Vertex(1, 100);
            gl.End( );
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, 0);
            //bela ivica
            gl.Color(1.0f, 1.0f, 1.0f);
            gl.Begin(OpenGL.GL_QUADS);
            gl.Vertex(56, -100);
            gl.Vertex(64, -100);
            gl.Vertex(64, 100);
            gl.Vertex(56, 100);
            gl.End( );
            //trava
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Grass]);
            gl.MatrixMode(OpenGL.GL_TEXTURE);
            gl.LoadIdentity( );
            gl.Scale(200f, 200f, 200f);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.Color(0.2f, 0.6f, 0.3f);
            gl.Begin(OpenGL.GL_QUADS);
            gl.TexCoord(0.0f, 0.0f);
            gl.Vertex(64, -100);
            gl.TexCoord(0.0f, 8.0f);
            gl.Vertex(100, -100);
            gl.TexCoord(8.0f, 2.0f);
            gl.Vertex(100, 100);
            gl.TexCoord(2.0f, 0.0f);
            gl.Vertex(64, 100);
            gl.End( );

            //leva ograda
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Metal]);
            gl.PushMatrix( );
            gl.Scale(3, 3, 3);
            gl.Translate(-20.0f, -35.0f, 1.5f);
            gl.Color(0.4f, 0.4f, 0.4f);

            for (int i = 0; i <= 15; i++) {
                gl.Translate(0f, 4, 0f);
                cube.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Design);
            }
            gl.PopMatrix( );

            //leva ograda
            gl.PushMatrix( );
            gl.Scale(3, 3, 3);
            gl.Translate(20.0f, -35.0f, 1.5f);
            gl.Color(0.4f, 0.4f, 0.4f);

            for (int i = 0; i <= 15; i++) {
                gl.Translate(0f, 4, 0f);
                cube.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Design);
            }
            gl.PopMatrix( );
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, 0);
            gl.PopMatrix( );

            //levi auto
            gl.PushMatrix( );

            gl.Rotate(90, 1.0f, 0.0f, 0.0f);
            gl.Translate(-30.0f + translateLeft, 4.0f, -70.0f + leftAnimation);
            gl.Scale(6 * scaleCar, 6 * scaleCar, 6 * scaleCar);
            gl.Translate(0.0f, (scaleCar-1) * 0.2 , 0.0f);
            float[] light2pos = new float[] { 0f, 10f, 0f, 1.0f };
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_POSITION, light2pos);
            m_scene1.Draw( );
            gl.PopMatrix( );


            //desni auto
            gl.PushMatrix( );
            gl.Rotate(90, 1.0f, 0.0f, 0.0f);
            gl.Translate(30.0f, 4.0f, -70.0f + rightAnimation);
            gl.Rotate(0, rotateRight, 0);
            gl.Scale(6 * scaleCar, 6 * scaleCar, 6 * scaleCar);
            gl.Translate(0.0f, (scaleCar - 1) * 0.2, 0.0f);
            light2pos = new float[] { 0f, 10f, 0f, 1.0f };
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_POSITION, light2pos);
            m_scene2.Draw( );
            gl.PopMatrix( );
            gl.PopMatrix( );

            //text
            gl.PushMatrix( );
            gl.Color(1.0f, 0.0f, 0.0f);
            gl.Viewport(m_width * 2 / 3, 0, m_width, m_height / 3);
            gl.Ortho2D(0, 1, 0, 1);
            gl.PushMatrix( );
            gl.Translate(-24, 0, 80);
            gl.Scale(1, 3, 1);
            gl.DrawText3D("Helvetica", 14f, 1f, 0.1f, "Predmet: Racunarska grafika");
            gl.Translate(-11.6, -0.1, 0);
            gl.DrawText3D("Helvetica", 14f, 1f, 0.1f, "_______________________");
            gl.Translate(-11.6, -1, 0);
            gl.DrawText3D("Helvetica", 14f, 1f, 0.1f, "Sk.god: 2018/19.");
            gl.Translate(-6.75, -0.1, 0);
            gl.DrawText3D("Helvetica", 14f, 1f, 0.1f, "______________");
            gl.Translate(-6.75, -1, 0);
            gl.DrawText3D("Helvetica", 14f, 1f, 0.1f, "Ime: Jelena");
            gl.Translate(-4.6, -0.1, 0);
            gl.DrawText3D("Helvetica", 14f, 1f, 0.1f, "_________");
            gl.Translate(-4.6, -1, 0);
            gl.DrawText3D("Helvetica", 14f, 1f, 0.1f, "Prezime: Dokic");
            gl.Translate(-6, -0.2, 0);
            gl.DrawText3D("Helvetica", 14f, 1f, 0.1f, "____________");
            gl.Translate(-6, -1, 0);
            gl.DrawText3D("Helvetica", 14f, 1f, 0.1f, "Sifra zad: 2.2");
            gl.Translate(-5.4, -0.1, 0);
            gl.DrawText3D("Helvetica", 14f, 1f, 0.1f, "____________");
            gl.PopMatrix( );
            gl.PopMatrix( );

            gl.Flush( );
        }

        /// <summary>
        ///  Dispose metoda.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///  Destruktor.
        /// </summary>
        ~World()
        {
            this.Dispose(false);
        }

        #endregion

        #region IDisposable Metode

        /// <summary>
        ///  Implementacija IDisposable interfejsa.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            //if (disposing)
            //{
            //    //Oslobodi managed resurse
            //}
            if (disposing) {
                m_scene1.Dispose( );
                m_scene2.Dispose( );
            }
        }

        #endregion
    }
}
