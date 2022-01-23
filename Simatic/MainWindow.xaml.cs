//#define IRB6700

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HelixToolkit.Wpf;
using System.IO;


namespace Simatic
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private RobotArm robotArm = null;
        private Model3D sphere = null;

        bool switchingJoint = false;
        bool isAnimating = false;

        Color oldColor = Colors.White;
        GeometryModel3D oldSelectedModel = null;

        Vector3D reachingPoint;
        int movements = 10;
        System.Windows.Forms.Timer timer1;


        double DistanceThreshold = 20;
        double LearningRate = 0.01;
        double SamplingDistance = 0.15;


        public MainWindow()
        {
            InitializeComponent();

            viewPort3d.RotateGesture = new MouseGesture(MouseAction.RightClick);
            viewPort3d.PanGesture = new MouseGesture(MouseAction.LeftClick);
            DrawSphere();

            TbX.TextChanged += ReachingPoint_TextChanged;
            TbY.TextChanged += ReachingPoint_TextChanged;
            TbZ.TextChanged += ReachingPoint_TextChanged;

            DrawRoboticArm();
            viewPort3d.Camera.LookDirection = new Vector3D(2038, -5200, -2930);
            viewPort3d.Camera.UpDirection = new Vector3D(-0.145, 0.372, 0.917);
            viewPort3d.Camera.Position = new Point3D(-1571, 4801, 3774);

            SetSlidersPosition();
            //robotArm.UpdatePositionInfo += UpdatePositionInfo;
            double[] angles = { robotArm.Joints[0].angle, robotArm.Joints[1].angle, robotArm.Joints[2].angle, robotArm.Joints[3].angle, robotArm.Joints[4].angle, robotArm.Joints[5].angle };
            //robotArm.ForwardKinematics();
            ForwardKinematics(angles);
            ChangeSelectedJoint();


            timer1 = new System.Windows.Forms.Timer();
            timer1.Interval = 5;
            timer1.Tick += new System.EventHandler(timer1_Tick);

            InitEnvironment();

            viewPort3d.Camera.ChangeDirection(new Vector3D(-2825.791, 4629.890, -3218.286), new Vector3D(0.182, -0.300, 0.937), 5000);
            viewPort3d.ShowCameraTarget = true;
        }

        // ------------------------------------------------------------------------------------------------------------
        // ------------------------------------------------------------------------------------------------------------
        private void DrawSphere()
        {
            try
            {
                ModelVisual3D visual;
                var builder = new MeshBuilder(true, true);
                var position = new Point3D(0, 0, 0);
                builder.AddSphere(position, 50, 15, 15);
                sphere = new GeometryModel3D(builder.ToMesh(), Materials.Brown);
                visual = new ModelVisual3D();
                visual.Content = sphere;
                viewPort3d.Children.Add(visual);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception Error:" + ex.StackTrace + " " + ex.Message);
            }
        }
        private void DrawRoboticArm()
        {
            try
            {
                IRB4600 irb4600 = new IRB4600(); robotArm = new RobotArm(irb4600);
                //IRB6620 irb6620 = new IRB6620(); robotArm = new RobotArm(irb6620);
                //IRB6700 irb6700 = new IRB6700(); robotArm = new RobotArm(irb6700);

                viewPort3d.Children.Add(robotArm);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception Error:" + ex.StackTrace + " " + ex.Message);
            }
        }
        private void SetSlidersPosition()
        {
            try
            {
                sldJoint1.Minimum = robotArm.Joints[0].angleMin; sldJoint1.Maximum = robotArm.Joints[0].angleMax;
                sldJoint2.Minimum = robotArm.Joints[1].angleMin; sldJoint2.Maximum = robotArm.Joints[1].angleMax;
                sldJoint3.Minimum = robotArm.Joints[2].angleMin; sldJoint3.Maximum = robotArm.Joints[2].angleMax;
                sldJoint4.Minimum = robotArm.Joints[3].angleMin; sldJoint4.Maximum = robotArm.Joints[3].angleMax;
                sldJoint5.Minimum = robotArm.Joints[4].angleMin; sldJoint5.Maximum = robotArm.Joints[4].angleMax;
                sldJoint6.Minimum = robotArm.Joints[5].angleMin; sldJoint6.Maximum = robotArm.Joints[5].angleMax;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception Error:" + ex.StackTrace + " " + ex.Message);
            }
        }
        private void InitEnvironment()
        {
            try
            {
                for (int i = 0; i <= 6; i++)
                {
                    sldJointSelector.Value = i;
                }

                Binding TbXbinding = new Binding("Value");
                TbXbinding.Source = sldJointX;
                TbX.SetBinding(TextBox.TextProperty, TbXbinding);

                Binding TbYbinding = new Binding("Value");
                TbYbinding.Source = sldJointY;
                TbY.SetBinding(TextBox.TextProperty, TbYbinding);

                Binding TbZbinding = new Binding("Value");
                TbZbinding.Source = sldJointZ;
                TbZ.SetBinding(TextBox.TextProperty, TbZbinding);

                sldJointX.Value = 1526;
                sldJointY.Value = 0;
                sldJointZ.Value = 1765;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception Error:" + ex.StackTrace + " " + ex.Message);
            }
        }

        // ------------------------------------------------------------------------------------------------------------
        // ------------------------------------------------------------------------------------------------------------
        public static T Clamp<T>(T value, T min, T max)
            where T : System.IComparable<T>
        {
            T result = value;
            if (value.CompareTo(max) > 0)
                result = max;
            if (value.CompareTo(min) < 0)
                result = min;
            return result;
        }

        // ------------------------------------------------------------------------------------------------------------
        // ------------------------------------------------------------------------------------------------------------
        private void ViewPort3D_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point mousePos = e.GetPosition(viewPort3d);
            PointHitTestParameters hitParams = new PointHitTestParameters(mousePos);
            VisualTreeHelper.HitTest(viewPort3d, null, ResultCallback, hitParams);
        }
        private void ViewPort3D_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Perform the hit test on the mouse's position relative to the viewport.
            HitTestResult result = VisualTreeHelper.HitTest(viewPort3d, e.GetPosition(viewPort3d));
            RayMeshGeometry3DHitTestResult mesh_result = result as RayMeshGeometry3DHitTestResult;

            if (oldSelectedModel != null)
                UnselectModel();

            if (mesh_result != null)
            {
                SelectModel(mesh_result.ModelHit);
            }
        }

        private void ReachingPoint_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                reachingPoint = new Vector3D(Double.Parse(TbX.Text), Double.Parse(TbY.Text), Double.Parse(TbZ.Text));
                sphere.Transform = new TranslateTransform3D(reachingPoint);
            }
            catch
            {

            }
        }
        private void JointSelector_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                ChangeSelectedJoint();
            }
            catch
            {
            }

        }
        private void RotationPoint_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                if (switchingJoint)
                    return;

                int sel = ((int)sldJointSelector.Value) - 1;
                robotArm.Joints[sel].rotPointX = (int)sldJointX.Value;
                robotArm.Joints[sel].rotPointY = (int)sldJointY.Value;
                robotArm.Joints[sel].rotPointZ = (int)sldJointZ.Value;
                UpdateSpherePosition();
            }
            catch
            {
            }
        }
        private void Joint_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                if (isAnimating)
                    return;

                robotArm.Joints[0].angle = sldJoint1.Value;
                robotArm.Joints[1].angle = sldJoint2.Value;
                robotArm.Joints[2].angle = sldJoint3.Value;
                robotArm.Joints[3].angle = sldJoint4.Value;
                robotArm.Joints[4].angle = sldJoint5.Value;
                robotArm.Joints[5].angle = sldJoint6.Value;
                StartForwardKinematics();
            }
            catch
            {
            }
        }
        private void CheckBox_StateChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                if (switchingJoint)
                    return;

                int sel = ((int)sldJointSelector.Value) - 1;
                robotArm.Joints[sel].rotAxisX = jointXAxis.IsChecked.Value ? 1 : 0;
                robotArm.Joints[sel].rotAxisY = jointYAxis.IsChecked.Value ? 1 : 0;
                robotArm.Joints[sel].rotAxisZ = jointZAxis.IsChecked.Value ? 1 : 0;
            }
            catch
            {
            }
        }

        private void ChangeSelectedJoint()
        {
            try
            {
                if (robotArm.Joints == null)
                    return;

                int sel = ((int)sldJointSelector.Value) - 1;
                switchingJoint = true;
                UnselectModel();
                if (sel < 0)
                {
                    sldJointX.IsEnabled = false;
                    sldJointY.IsEnabled = false;
                    sldJointZ.IsEnabled = false;
                    jointXAxis.IsEnabled = false;
                    jointYAxis.IsEnabled = false;
                    jointZAxis.IsEnabled = false;
                }
                else
                {
                    if (!sldJointX.IsEnabled)
                    {
                        sldJointX.IsEnabled = true;
                        sldJointY.IsEnabled = true;
                        sldJointZ.IsEnabled = true;
                        jointXAxis.IsEnabled = true;
                        jointYAxis.IsEnabled = true;
                        jointZAxis.IsEnabled = true;
                    }
                    sldJointX.Value = robotArm.Joints[sel].rotPointX;
                    sldJointY.Value = robotArm.Joints[sel].rotPointY;
                    sldJointZ.Value = robotArm.Joints[sel].rotPointZ;
                    jointXAxis.IsChecked = robotArm.Joints[sel].rotAxisX == 1 ? true : false;
                    jointYAxis.IsChecked = robotArm.Joints[sel].rotAxisY == 1 ? true : false;
                    jointZAxis.IsChecked = robotArm.Joints[sel].rotAxisZ == 1 ? true : false;
                    SelectModel(robotArm.Joints[sel].model);
                    UpdateSpherePosition();
                }
                switchingJoint = false;

            }
            catch
            {
            }

        }
        private Color ChangeModelColor(RobotJoint pJoint, Color newColor)
        {
            Model3DGroup models = ((Model3DGroup)pJoint.model);
            return ChangeModelColor(models.Children[0] as GeometryModel3D, newColor);
        }
        private Color ChangeModelColor(GeometryModel3D pModel, Color newColor)
        {
            if (pModel == null)
                return oldColor;

            Color previousColor = Colors.Black;

            MaterialGroup mg = (MaterialGroup)pModel.Material;
            if (mg.Children.Count > 0)
            {
                try
                {
                    previousColor = ((EmissiveMaterial)mg.Children[0]).Color;
                    ((EmissiveMaterial)mg.Children[0]).Color = newColor;
                    ((DiffuseMaterial)mg.Children[1]).Color = newColor;
                }
                catch
                {
                    previousColor = oldColor;
                }
            }

            return previousColor;
        }
        private void SelectModel(Model3D pModel, Color? Color = null)
        {
            try
            {
                Model3DGroup models = ((Model3DGroup)pModel);
                oldSelectedModel = models.Children[0] as GeometryModel3D;
            }
            catch
            {
                oldSelectedModel = (GeometryModel3D)pModel;
            }
            oldColor = ChangeModelColor(oldSelectedModel, Color ?? ColorHelper.HexToColor("#ff3333"));
        }
        private void UnselectModel()
        {
            ChangeModelColor(oldSelectedModel, oldColor);
        }
        public HitTestResultBehavior ResultCallback(HitTestResult result)
        {
            // Did we hit 3D?
            RayHitTestResult rayResult = result as RayHitTestResult;
            if (rayResult != null)
            {
                // Did we hit a MeshGeometry3D?
                RayMeshGeometry3DHitTestResult rayMeshResult = rayResult as RayMeshGeometry3DHitTestResult;
                sphere.Transform = new TranslateTransform3D(new Vector3D(rayResult.PointHit.X, rayResult.PointHit.Y, rayResult.PointHit.Z));

                if (rayMeshResult != null)
                {
                    // Yes we did!
                }
            }

            return HitTestResultBehavior.Continue;
        }


        // ------------------------------------------------------------------------------------------------------------
        // ------------------------------------------------------------------------------------------------------------
        public void StartInverseKinematics(object sender, RoutedEventArgs e)
        {
            try
            {
                if (timer1.Enabled)
                {
                    button.Content = "Run";
                    isAnimating = false;
                    timer1.Stop();
                    movements = 0;
                }
                else
                {
                    sphere.Transform = new TranslateTransform3D(reachingPoint);
                    movements = 5000;
                    button.Content = "Stop";
                    isAnimating = true;
                    timer1.Start();
                }
            }
            catch
            {
            }
        }
        public void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                double[] angles = { robotArm.Joints[0].angle, robotArm.Joints[1].angle, robotArm.Joints[2].angle, robotArm.Joints[3].angle, robotArm.Joints[4].angle, robotArm.Joints[5].angle };
                angles = InverseKinematics(reachingPoint, angles);
                sldJoint1.Value = robotArm.Joints[0].angle = angles[0];
                sldJoint2.Value = robotArm.Joints[1].angle = angles[1];
                sldJoint3.Value = robotArm.Joints[2].angle = angles[2];
                sldJoint4.Value = robotArm.Joints[3].angle = angles[3];
                sldJoint5.Value = robotArm.Joints[4].angle = angles[4];
                sldJoint6.Value = robotArm.Joints[5].angle = angles[5];

                if ((--movements) <= 0)
                {
                    button.Content = "Run";
                    isAnimating = false;
                    timer1.Stop();
                }
            }
            catch
            {
            }
        }
        public double[] InverseKinematics(Vector3D target, double[] angles)
        {
            try
            {
                if (DistanceFromTarget(target, angles) < DistanceThreshold)
                {
                    movements = 0;
                    return angles;
                }

                double[] oldAngles = { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
                angles.CopyTo(oldAngles, 0);
                for (int i = 0; i <= 5; i++)
                {
                    // Gradient descent
                    // Update : Solution -= LearningRate * Gradient
                    double gradient = PartialGradient(target, angles, i);
                    angles[i] -= LearningRate * gradient;

                    // Clamp
                    angles[i] = Clamp(angles[i], robotArm.Joints[i].angleMin, robotArm.Joints[i].angleMax);

                    // Early termination
                    if (DistanceFromTarget(target, angles) < DistanceThreshold || CheckAngles(oldAngles, angles))
                    {
                        movements = 0;
                        return angles;
                    }
                }
            }
            catch
            {
            }

            return angles;
        }
        public bool CheckAngles(double[] oldAngles, double[] angles)
        {
            try
            {

                for (int i = 0; i <= 5; i++)
                {
                    if (oldAngles[i] != angles[i])
                        return false;
                }
            }
            catch
            {
            }
            return true;
        }
        public double PartialGradient(Vector3D target, double[] angles, int i)
        {
            // Saves the angle, it will be restored later
            double angle = angles[i];

            // Gradient : [F(x+SamplingDistance) - F(x)] / h
            double f_x = DistanceFromTarget(target, angles);

            angles[i] += SamplingDistance;
            double f_x_plus_d = DistanceFromTarget(target, angles);

            double gradient = (f_x_plus_d - f_x) / SamplingDistance;

            // Restores
            angles[i] = angle;

            return gradient;
        }
        public double DistanceFromTarget(Vector3D target, double[] angles)
        {
            Vector3D point = ForwardKinematics(angles);
            return Math.Sqrt(Math.Pow((point.X - target.X), 2.0) + Math.Pow((point.Y - target.Y), 2.0) + Math.Pow((point.Z - target.Z), 2.0));
        }

        // ------------------------------------------------------------------------------------------------------------
        // ------------------------------------------------------------------------------------------------------------
        private void StartForwardKinematics()
        {
            try
            {
                double[] angles = { robotArm.Joints[0].angle, robotArm.Joints[1].angle, robotArm.Joints[2].angle, robotArm.Joints[3].angle, robotArm.Joints[4].angle, robotArm.Joints[5].angle };
                ForwardKinematics(angles);
                UpdateSpherePosition();
            }
            catch
            {
            }
        }
        public Vector3D ForwardKinematics(double[] angles)
        {
            Transform3DGroup TransformJ1;
            Transform3DGroup TransformJ2;
            Transform3DGroup TransformJ3;
            Transform3DGroup TransformJ4;
            Transform3DGroup TransformJ5;
            Transform3DGroup TransformJ6;
            RotateTransform3D Rotate;
            TranslateTransform3D Translate;

            try
            {
                //The base only has rotation and is always at the origin, so the only transform in the transformGroup is the rotation R
                TransformJ1 = new Transform3DGroup();
                Rotate = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(robotArm.Joints[0].rotAxisX, robotArm.Joints[0].rotAxisY, robotArm.Joints[0].rotAxisZ), angles[0]), new Point3D(robotArm.Joints[0].rotPointX, robotArm.Joints[0].rotPointY, robotArm.Joints[0].rotPointZ));
                TransformJ1.Children.Add(Rotate);

                //This moves the first joint attached to the base, it may translate and rotate. Since the joint are already in the right position (the .stl model also store the joints position
                //in the virtual world when they were first created, so if you load all the .stl models of the joint they will be automatically positioned in the right locations)
                //so in all of these cases the first translation is always 0, I just left it for future purposes if something need to be moved
                //After that, the joint needs to rotate of a certain amount (given by the value in the slider), and the rotation must be executed on a specific point
                //After some testing it looks like the point 175, -200, 500 is the sweet spot to achieve the rotation intended for the joint
                //finally we also need to apply the transformation applied to the base 
                TransformJ2 = new Transform3DGroup();
                Translate = new TranslateTransform3D(0, 0, 0);
                Rotate = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(robotArm.Joints[1].rotAxisX, robotArm.Joints[1].rotAxisY, robotArm.Joints[1].rotAxisZ), angles[1]), new Point3D(robotArm.Joints[1].rotPointX, robotArm.Joints[1].rotPointY, robotArm.Joints[1].rotPointZ));
                TransformJ2.Children.Add(Translate);
                TransformJ2.Children.Add(Rotate);
                TransformJ2.Children.Add(TransformJ1);

                //The second joint is attached to the first one. As before I found the sweet spot after testing, and looks like is rotating just fine. No pre-translation as before
                //and again the previous transformation needs to be applied
                TransformJ3 = new Transform3DGroup();
                Translate = new TranslateTransform3D(0, 0, 0);
                Rotate = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(robotArm.Joints[2].rotAxisX, robotArm.Joints[2].rotAxisY, robotArm.Joints[2].rotAxisZ), angles[2]), new Point3D(robotArm.Joints[2].rotPointX, robotArm.Joints[2].rotPointY, robotArm.Joints[2].rotPointZ));
                TransformJ3.Children.Add(Translate);
                TransformJ3.Children.Add(Rotate);
                TransformJ3.Children.Add(TransformJ2);

                //as before
                TransformJ4 = new Transform3DGroup();
                Translate = new TranslateTransform3D(0, 0, 0); //1500, 650, 1650
                Rotate = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(robotArm.Joints[3].rotAxisX, robotArm.Joints[3].rotAxisY, robotArm.Joints[3].rotAxisZ), angles[3]), new Point3D(robotArm.Joints[3].rotPointX, robotArm.Joints[3].rotPointY, robotArm.Joints[3].rotPointZ));
                TransformJ4.Children.Add(Translate);
                TransformJ4.Children.Add(Rotate);
                TransformJ4.Children.Add(TransformJ3);

                //as before
                TransformJ5 = new Transform3DGroup();
                Translate = new TranslateTransform3D(0, 0, 0);
                Rotate = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(robotArm.Joints[4].rotAxisX, robotArm.Joints[4].rotAxisY, robotArm.Joints[4].rotAxisZ), angles[4]), new Point3D(robotArm.Joints[4].rotPointX, robotArm.Joints[4].rotPointY, robotArm.Joints[4].rotPointZ));
                TransformJ5.Children.Add(Translate);
                TransformJ5.Children.Add(Rotate);
                TransformJ5.Children.Add(TransformJ4);

                //NB: I was having a nightmare trying to understand why it was always rotating in a weird way... SO I realized that the order in which
                //you add the Children is actually VERY IMPORTANT in fact before I was applyting F and then T and R, but the previous transformation
                //Should always be applied as last (FORWARD Kinematics)
                TransformJ6 = new Transform3DGroup();
                Translate = new TranslateTransform3D(0, 0, 0);
                Rotate = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(robotArm.Joints[5].rotAxisX, robotArm.Joints[5].rotAxisY, robotArm.Joints[5].rotAxisZ), angles[5]), new Point3D(robotArm.Joints[5].rotPointX, robotArm.Joints[5].rotPointY, robotArm.Joints[5].rotPointZ));
                TransformJ6.Children.Add(Translate);
                TransformJ6.Children.Add(Rotate);
                TransformJ6.Children.Add(TransformJ5);


                robotArm.Joints[0].model.Transform = TransformJ1; //First joint
                robotArm.Joints[1].model.Transform = TransformJ2; //Second joint (the "biceps")
                robotArm.Joints[2].model.Transform = TransformJ3; //third joint (the "knee" or "elbow")
                robotArm.Joints[3].model.Transform = TransformJ4; //the "forearm"
                robotArm.Joints[4].model.Transform = TransformJ5; //the tool plate
                robotArm.Joints[5].model.Transform = TransformJ6; //the tool

                UpdatePositionInfo(this, EventArgs.Empty);

                switch (robotArm.GetName())
                {
                    case "IRB4600":
                        robotArm.Joints[7].model.Transform = TransformJ1; //Cables
                        robotArm.Joints[8].model.Transform = TransformJ2; //Cables
                        robotArm.Joints[6].model.Transform = TransformJ3; //The ABB writing
                        robotArm.Joints[9].model.Transform = TransformJ3; //Cables
                        break;
                    case "IRB6620": break;
                    case "IRB6700":
                        robotArm.Joints[6].model.Transform = TransformJ1;
                        robotArm.Joints[7].model.Transform = TransformJ1;
                        robotArm.Joints[19].model.Transform = TransformJ1;
                        robotArm.Joints[14].model.Transform = TransformJ1;
                        robotArm.Joints[8].model.Transform = TransformJ2;
                        robotArm.Joints[9].model.Transform = TransformJ2;
                        robotArm.Joints[10].model.Transform = TransformJ3;
                        robotArm.Joints[11].model.Transform = TransformJ3;
                        robotArm.Joints[12].model.Transform = TransformJ3;
                        robotArm.Joints[16].model.Transform = TransformJ3;
                        robotArm.Joints[13].model.Transform = TransformJ4;
                        robotArm.Joints[17].model.Transform = TransformJ4;
                        break;
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }

            return new Vector3D(robotArm.Joints[5].model.Bounds.Location.X, robotArm.Joints[5].model.Bounds.Location.Y, robotArm.Joints[5].model.Bounds.Location.Z);
        }
        private void UpdateSpherePosition()
        {
            int sel = ((int)sldJointSelector.Value) - 1;
            if (sel < 0)
                return;

            Transform3DGroup F = new Transform3DGroup();
            F.Children.Add(new TranslateTransform3D(robotArm.Joints[sel].rotPointX, robotArm.Joints[sel].rotPointY, robotArm.Joints[sel].rotPointZ));
            F.Children.Add(robotArm.Joints[sel].model.Transform);
            sphere.Transform = F;
        }

        // ------------------------------------------------------------------------------------------------------------
        // ------------------------------------------------------------------------------------------------------------
        public void UpdatePositionInfo(object sender, EventArgs e)
        {
            try
            {
                Tx.Content = robotArm.Joints[5].model.Bounds.Location.X;
                Ty.Content = robotArm.Joints[5].model.Bounds.Location.Y;
                Tz.Content = robotArm.Joints[5].model.Bounds.Location.Z;
                Tx_Copy.Content = sphere.Bounds.Location.X;
                Ty_Copy.Content = sphere.Bounds.Location.Y;
                Tz_Copy.Content = sphere.Bounds.Location.Z;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception Error:" + ex.StackTrace + " " + ex.Message);
            }
        }
        private void Joint_MouseEnter(object sender, MouseEventArgs e)
        {
            try
            {
                Slider sl = sender as Slider;
                int uid = -1;

                if (int.TryParse(sl.Uid, out uid))
                {
                    if (oldSelectedModel != null)
                        UnselectModel();
                    if (robotArm.Joints[uid - 1].model != null)
                    {
                        SelectModel(robotArm.Joints[uid - 1].model, Colors.Yellow);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception Error:" + ex.StackTrace + " " + ex.Message);
            }
        }
        private void Joint_MouseLeave(object sender, MouseEventArgs e)
        {
            try
            {
                if (oldSelectedModel != null)
                    UnselectModel();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception Error:" + ex.StackTrace + " " + ex.Message);
            }
        }

    }
}
