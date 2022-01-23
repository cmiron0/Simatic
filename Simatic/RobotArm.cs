using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using System.IO;

namespace Simatic
{
    public class RobotArm : ModelVisual3D
    {
        public event EventHandler UpdatePositionInfo;

        public Model3D model = null;
        private Model3DGroup RoboticArmGroup = new Model3DGroup(); 
        public List<RobotJoint> Joints = new List<RobotJoint>();
        private Color mainColor = Colors.White;

        public RobotArm(RobotConfig robotConfig)
        {
            try
            {
                this.SetName(robotConfig.Name);
                Assemble(robotConfig);
                this.Content = RoboticArmGroup;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void Assemble(RobotConfig robotConfig)
        {
            try
            {
                string basePath = robotConfig.Path;

                foreach (string file in robotConfig.Files)
                {
                    var materialGroup = new MaterialGroup();
                    EmissiveMaterial emissMat = new EmissiveMaterial(new SolidColorBrush(mainColor));
                    DiffuseMaterial diffMat = new DiffuseMaterial(new SolidColorBrush(mainColor));
                    SpecularMaterial specMat = new SpecularMaterial(new SolidColorBrush(mainColor), 200);
                    materialGroup.Children.Add(emissMat);
                    materialGroup.Children.Add(diffMat);
                    materialGroup.Children.Add(specMat);

                    ModelImporter import = new ModelImporter();
                    Model3DGroup link = import.Load(basePath + "\\" + file);
                    GeometryModel3D model = link.Children[0] as GeometryModel3D;
                    model.Material = materialGroup;
                    model.BackMaterial = materialGroup;
                    Joints.Add(new RobotJoint(link));
                }

                ConfigureJoints(Joints, robotConfig.JoinCfg);

                for (int i = 0; i < Joints.Count; i++)
                {
                    RoboticArmGroup.Children.Add(Joints[i].model);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void ConfigureJoints(List<RobotJoint> joints, Dictionary<int, JoinConfig> joinCfg)
        {
            try 
            {
                for (int i = 0; i < Joints.Count; i++)
                {
                    JoinConfig cfg = new JoinConfig();
                    if (joinCfg.TryGetValue(i, out cfg))
                    {
                        if (cfg.Coord != null)
                            joints[i].ChangeValues(cfg.Coord);
                        if (cfg.Color != null)
                            Joints[i].ChangeModelColor(cfg.Color);
                    }
                    //RoboticArmGroup.Children.Add(Joints[i].model);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private Color ChangeModelColor(RobotJoint Joint, Color Color)
        {
            try
            {
                Model3DGroup models = (Model3DGroup)Joint.model;
                return ChangeModelColor(models.Children[0] as GeometryModel3D, Color);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        private Color ChangeModelColor(GeometryModel3D Model, Color Color)
        {
            Color previousColor = Colors.Black;

            try
            {
                if (Model == null)
                    return mainColor;

                MaterialGroup mg = (MaterialGroup)Model.Material;
                if (mg.Children.Count > 0)
                {
                    try
                    {
                        previousColor = ((EmissiveMaterial)mg.Children[0]).Color;
                        ((EmissiveMaterial)mg.Children[0]).Color = Color;
                        ((DiffuseMaterial)mg.Children[1]).Color = Color;
                    }
                    catch
                    {
                        previousColor = mainColor;
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                //throw new Exception(ex.Message);
            }

            return previousColor;
        }

        protected virtual void OnUpdatePositionInfo(EventArgs e)
        {
            UpdatePositionInfo?.Invoke(this, e);
        }

        public Vector3D ForwardKinematics(double[] angles = null)
        {
            if (angles == null)
            {
                angles = new double[] { this.Joints[0].angle, this.Joints[1].angle, this.Joints[2].angle, this.Joints[3].angle, this.Joints[4].angle, this.Joints[5].angle };
            }

            RobotJoint[] joints = { this.Joints[0], Joints[1], this.Joints[2], this.Joints[3], this.Joints[4], this.Joints[5] };

            try
            {
                Transform3DGroup TransformJ1;
                Transform3DGroup TransformJ2;
                Transform3DGroup TransformJ3;
                Transform3DGroup TransformJ4;
                Transform3DGroup TransformJ5;
                Transform3DGroup TransformJ6;
                RotateTransform3D Rotate;
                TranslateTransform3D Translate;


                //The base only has rotation and is always at the origin, so the only transform in the transformGroup is the rotation R
                TransformJ1 = new Transform3DGroup();
                Rotate = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(joints[0].rotAxisX, joints[0].rotAxisY, joints[0].rotAxisZ), angles[0]), new Point3D(joints[0].rotPointX, joints[0].rotPointY, joints[0].rotPointZ));
                TransformJ1.Children.Add(Rotate);

                //This moves the first joint attached to the base, it may translate and rotate. Since the joint are already in the right position (the .stl model also store the joints position
                //in the virtual world when they were first created, so if you load all the .stl models of the joint they will be automatically positioned in the right locations)
                //so in all of these cases the first translation is always 0, I just left it for future purposes if something need to be moved
                //After that, the joint needs to rotate of a certain amount (given by the value in the slider), and the rotation must be executed on a specific point
                //After some testing it looks like the point 175, -200, 500 is the sweet spot to achieve the rotation intended for the joint
                //finally we also need to apply the transformation applied to the base 
                TransformJ2 = new Transform3DGroup();
                Translate = new TranslateTransform3D(0, 0, 0);
                Rotate = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(joints[1].rotAxisX, joints[1].rotAxisY, joints[1].rotAxisZ), angles[1]), new Point3D(joints[1].rotPointX, joints[1].rotPointY, joints[1].rotPointZ));
                TransformJ2.Children.Add(Translate);
                TransformJ2.Children.Add(Rotate);
                TransformJ2.Children.Add(TransformJ1);

                //The second joint is attached to the first one. As before I found the sweet spot after testing, and looks like is rotating just fine. No pre-translation as before
                //and again the previous transformation needs to be applied
                TransformJ3 = new Transform3DGroup();
                Translate = new TranslateTransform3D(0, 0, 0);
                Rotate = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(joints[2].rotAxisX, joints[2].rotAxisY, joints[2].rotAxisZ), angles[2]), new Point3D(joints[2].rotPointX, joints[2].rotPointY, joints[2].rotPointZ));
                TransformJ3.Children.Add(Translate);
                TransformJ3.Children.Add(Rotate);
                TransformJ3.Children.Add(TransformJ2);

                //as before
                TransformJ4 = new Transform3DGroup();
                Translate = new TranslateTransform3D(0, 0, 0); //1500, 650, 1650
                Rotate = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(joints[3].rotAxisX, joints[3].rotAxisY, joints[3].rotAxisZ), angles[3]), new Point3D(joints[3].rotPointX, joints[3].rotPointY, joints[3].rotPointZ));
                TransformJ4.Children.Add(Translate);
                TransformJ4.Children.Add(Rotate);
                TransformJ4.Children.Add(TransformJ3);

                //as before
                TransformJ5 = new Transform3DGroup();
                Translate = new TranslateTransform3D(0, 0, 0);
                Rotate = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(joints[4].rotAxisX, joints[4].rotAxisY, joints[4].rotAxisZ), angles[4]), new Point3D(joints[4].rotPointX, joints[4].rotPointY, joints[4].rotPointZ));
                TransformJ5.Children.Add(Translate);
                TransformJ5.Children.Add(Rotate);
                TransformJ5.Children.Add(TransformJ4);

                //NB: I was having a nightmare trying to understand why it was always rotating in a weird way... SO I realized that the order in which
                //you add the Children is actually VERY IMPORTANT in fact before I was applyting F and then T and R, but the previous transformation
                //Should always be applied as last (FORWARD Kinematics)
                TransformJ6 = new Transform3DGroup();
                Translate = new TranslateTransform3D(0, 0, 0);
                Rotate = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(joints[5].rotAxisX, joints[5].rotAxisY, joints[5].rotAxisZ), angles[5]), new Point3D(joints[5].rotPointX, joints[5].rotPointY, joints[5].rotPointZ));
                TransformJ6.Children.Add(Translate);
                TransformJ6.Children.Add(Rotate);
                TransformJ6.Children.Add(TransformJ5);


                joints[0].model.Transform = TransformJ1; //First joint
                joints[1].model.Transform = TransformJ2; //Second joint (the "biceps")
                joints[2].model.Transform = TransformJ3; //third joint (the "knee" or "elbow")
                joints[3].model.Transform = TransformJ4; //the "forearm"
                joints[4].model.Transform = TransformJ5; //the tool plate
                joints[5].model.Transform = TransformJ6; //the tool

                OnUpdatePositionInfo(EventArgs.Empty);

                string baseName = this.GetName();
                if (baseName == "IRB4600")
                {
                    this.Joints[7].model.Transform = TransformJ1; //Cables
                    this.Joints[8].model.Transform = TransformJ2; //Cables
                    this.Joints[6].model.Transform = TransformJ3; //The ABB writing
                    this.Joints[9].model.Transform = TransformJ3; //Cables
                }
                if (baseName == "IRB6620")
                {

                }
                if (baseName == "IRB6700")
                {
                    this.Joints[6].model.Transform = TransformJ1;
                    this.Joints[7].model.Transform = TransformJ1;
                    this.Joints[19].model.Transform = TransformJ1;
                    this.Joints[14].model.Transform = TransformJ1;

                    this.Joints[8].model.Transform = TransformJ2;
                    this.Joints[9].model.Transform = TransformJ2;

                    this.Joints[10].model.Transform = TransformJ3;
                    this.Joints[11].model.Transform = TransformJ3;
                    this.Joints[12].model.Transform = TransformJ3;
                    this.Joints[16].model.Transform = TransformJ3;

                    this.Joints[13].model.Transform = TransformJ4;
                    this.Joints[17].model.Transform = TransformJ4;
                }

            }
            catch (Exception ex)
            {
                //MessageBox.Show("Exception Error:" + ex.StackTrace + " " + ex.Message);
                throw new Exception(ex.Message);
            }

            return new Vector3D(joints[5].model.Bounds.Location.X, joints[5].model.Bounds.Location.Y, joints[5].model.Bounds.Location.Z);
        }


    }
}
