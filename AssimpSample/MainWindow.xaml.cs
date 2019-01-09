using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
//using System.Windows.Shapes;
using SharpGL.SceneGraph;
using SharpGL;
using System.Reflection;
using System.IO;
using Microsoft.Win32;
using SharpGL.WPF;
using System.Text.RegularExpressions;
using System.Globalization;

namespace Transformations
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Atributi

        /// <summary>
        ///  Instanca OpenGL "sveta" - klase koja je zaduzena za iscrtavanje koriscenjem OpenGL-a.
        /// </summary>
        World m_world = null;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent( );
            // Kreiranje OpenGL sveta
            try {
                m_world = new World(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly( ).Location), "3D Models\\Formula"), "ChevroletCamaro.dae", Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly( ).Location), "3D Models\\Formula"), "ChevroletCamaro.dae", (int)openGLControl.Width, (int)openGLControl.Height, openGLControl.OpenGL);
            } catch (Exception) {
                System.Windows.MessageBox.Show("Neuspesno kreirana instanca OpenGL sveta", "GRESKA", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close( );
            }
        }

        private void StartAnimation()
        {
            Keyboard.ClearFocus( );
            translateLeftTextBox.IsEnabled = false;
            rotateRightTextBox.IsEnabled = false;
            scaleCarTextBox.IsEnabled = false;
            m_world.TempMX = m_world.RotationX;
            m_world.Timer.Start( );
        }

        private void translateLeftCar(object sender, TextChangedEventArgs e)
        {
            try {
                string str = translateLeftTextBox.Text;
                if (!str.Equals("") && !str.Equals("-")) {
                    int num = Int32.Parse(str);
                    if (num > 20) { num = 20; } else if (num < -15) { num = -15; }
                    m_world.TranslateLeft = num;
                } else if (str.Equals("")) {
                    m_world.TranslateLeft = 0;
                }
            } catch {
                openGLControl.Focus( );
            }
        }

        private void rotateRightCar(object sender, TextChangedEventArgs e)
        {
            string str = rotateRightTextBox.Text;
            if (!str.Equals("") && !str.Equals("-")) {
                int num = Int32.Parse(str);
                if (num > 180) { num = 180; } else if (num < -180) { num = -180; }
                m_world.RotateRight = num;
            } else if (str.Equals("")) {
                m_world.RotateRight = 0;
            }
        }

        private void scaleCar(object sender, TextChangedEventArgs e)
        {
            string str = scaleCarTextBox.Text;
            if (!str.Equals("") && !str.Equals("-")) {
                double num = double.Parse(str, CultureInfo.InvariantCulture);
                if (num > 2) { num = 2; } else if (num < 0.7) { num = 0.7; }
                m_world.ScaleCar = num;
            } else if (str.Equals("")) {
                m_world.ScaleCar = 1;
            }
        }


        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9-]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void DoubleNumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //projectionComboBox.ItemsSource = Enum.GetValues(typeof(ProjectionType));
            //projectionComboBox.SelectedIndex = 0;
        }

        /// <summary>
        /// Handles the OpenGLDraw event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLDraw(object sender, OpenGLEventArgs args)
        {
            //Iscrtaj svet
            m_world.Draw(args.OpenGL);
        }

        /// <summary>
        /// Handles the OpenGLInitialized event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLInitialized(object sender, OpenGLEventArgs args)
        {
            m_world.Initialize(args.OpenGL);
        }

        /// <summary>
        /// Handles the Resized event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_Resized(object sender, OpenGLEventArgs args)
        {
            m_world.Resize(args.OpenGL, (int)openGLControl.ActualWidth, (int)openGLControl.ActualHeight);
        }

        private void projectionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            m_world.Resize(openGLControl.OpenGL, (int)openGLControl.ActualWidth, (int)openGLControl.ActualHeight);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key) {
                case Key.W:
                    if(m_world.RotationX > -20)
                        m_world.RotationX -= 2.0f;
                    break;
                case Key.S:
                    if (m_world.RotationX < 68)
                        m_world.RotationX += 2.0f;
                    break;

                case Key.A:
                    if (m_world.RotationY > -88)
                        m_world.RotationY -= 2.0f;
                    break;
                case Key.D:
                    if (m_world.RotationY < 88)
                        m_world.RotationY += 2.0f;
                    break;

                case Key.K:
                    if (m_world.ZDistance < -5)
                        m_world.ZDistance += 5.0f;
                    break;
                case Key.I: m_world.ZDistance -= 5.0f; break;

                case Key.J: m_world.RotationZ += 2.0f; break;
                case Key.L: m_world.RotationZ -= 2.0f; break;
                case Key.F4: Close( ); break;
                case Key.V: StartAnimation( ); break;
            }
        }
    }
}
