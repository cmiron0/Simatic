using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Simatic
{
    public class RobotJoint
    {
        private Color mainColor = Colors.White;

        public Model3D model { get; set; }
        public double angle { get; set; }
        public double angleMin { get; set; }
        public double angleMax { get; set; }
        public int rotAxisX { get; set; }
        public int rotAxisY { get; set; }
        public int rotAxisZ { get; set; }
        public int rotPointX { get; set; }
        public int rotPointY { get; set; }
        public int rotPointZ { get; set; }


        public RobotJoint(Model3D pModel)
        {
            model = pModel;
            angle = 0;
            angleMin = -180;
            angleMax = 180;
            rotAxisX = 0;
            rotAxisY = 0;
            rotAxisZ = 0;
            rotPointX = 0;
            rotPointY = 0;
            rotPointZ = 0;
        }

        public void ChangeValues(double angleMin, double angleMax, int rotAxisX, int rotAxisY, int rotAxisZ, int rotPointX, int rotPointY, int rotPointZ)
        {
            this.angleMin = angleMin;
            this.angleMax = angleMax;
            this.rotAxisX = rotAxisX;
            this.rotAxisY = rotAxisY;
            this.rotAxisZ = rotAxisZ;
            this.rotPointX = rotPointX;
            this.rotPointY = rotPointY;
            this.rotPointZ = rotPointZ;
        }
        public void ChangeValues(coords coord)
        {
            this.angleMin = coord.angleMin;
            this.angleMax = coord.angleMax;
            this.rotAxisX = coord.rotAxisX;
            this.rotAxisY = coord.rotAxisY;
            this.rotAxisZ = coord.rotAxisZ;
            this.rotPointX = coord.rotPointX;
            this.rotPointY = coord.rotPointY;
            this.rotPointZ = coord.rotPointZ;
        }

        public Color ChangeModelColor(Color? Color)
        {
            Model3DGroup model = ((Model3DGroup)this.model);
            return ChangeModelColor(model.Children[0] as GeometryModel3D, Color);
        }
        private Color ChangeModelColor(GeometryModel3D Model, Color? Color)
        {
            if (Model == null)
                return mainColor;

            Color previousColor = Colors.Black;

            MaterialGroup mg = (MaterialGroup)Model.Material;
            if (mg.Children.Count > 0)
            {
                try
                {
                    previousColor = ((EmissiveMaterial)mg.Children[0]).Color;
                    ((EmissiveMaterial)mg.Children[0]).Color = Color ?? previousColor;
                    ((DiffuseMaterial)mg.Children[1]).Color = Color ?? previousColor;
                }
                catch
                {
                    previousColor = mainColor;
                }
            }

            return previousColor;
        }
    }
}


