using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.IO;

namespace Simatic
{
    public class RobotConfig
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public List<string> Files { get; set; }
        public Dictionary<int, JoinConfig> JoinCfg { get; set; }
        

        public RobotConfig()
        {
            Name = "";
            Path = "";
            Files = new List<string>();
            JoinCfg = new Dictionary<int, JoinConfig>();
        }

        public RobotConfig(string name, string path, List<string> files, Dictionary<int,JoinConfig> joinCfg)
        {
            Name = name;
            Path = path;
            Files = files;
            JoinCfg = joinCfg;
        }
    }

    public class IRB4600 : RobotConfig
    {
        private static readonly string path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "\\ArmModels\\IRB4600\\";
        private static readonly List<string> files = new List<string>
        {
            "IRB4600_20kg-250_LINK1_CAD_rev04.stl",
            "IRB4600_20kg-250_LINK2_CAD_rev04.stl",
            "IRB4600_20kg-250_LINK3_CAD_rev005.stl",
            "IRB4600_20kg-250_LINK4_CAD_rev04.stl",
            "IRB4600_20kg-250_LINK5_CAD_rev04.stl",
            "IRB4600_20kg-250_LINK6_CAD_rev04.stl",
            "IRB4600_20kg-250_LINK3_CAD_rev04.stl",
            "IRB4600_20kg-250_CABLES_LINK1_rev03.stl",
            "IRB4600_20kg-250_CABLES_LINK2_rev03.stl",
            "IRB4600_20kg-250_CABLES_LINK3_rev03.stl",
            "IRB4600_20kg-250_BASE_CAD_rev04.stl"
        };
        private static readonly Dictionary<int, JoinConfig> joincfg = new Dictionary<int, JoinConfig> ()
        {
            //{00,new JoinConfig { Id=00, Coord=new coords() { angleMin=-180, angleMax=180, rotAxisX=0, rotAxisY=0, rotAxisZ=1, rotPointX=0000, rotPointY=0000, rotPointZ=0000 }, Color=null } },
            //{01,new JoinConfig { Id=01, Coord=new coords() { angleMin=-100, angleMax=060, rotAxisX=0, rotAxisY=1, rotAxisZ=0, rotPointX=0175, rotPointY=-200, rotPointZ=0500 }, Color=null } },
            //{02,new JoinConfig { Id=02, Coord=new coords() { angleMin=-090, angleMax=090, rotAxisX=0, rotAxisY=1, rotAxisZ=0, rotPointX=0190, rotPointY=-700, rotPointZ=1595 }, Color=null } },
            //{03,new JoinConfig { Id=03, Coord=new coords() { angleMin=-180, angleMax=180, rotAxisX=1, rotAxisY=0, rotAxisZ=0, rotPointX=0400, rotPointY=0000, rotPointZ=1765 }, Color=null } },
            //{04,new JoinConfig { Id=04, Coord=new coords() { angleMin=-115, angleMax=115, rotAxisX=0, rotAxisY=1, rotAxisZ=0, rotPointX=1405, rotPointY=0050, rotPointZ=1765 }, Color=null } },
            //{05,new JoinConfig { Id=05, Coord=new coords() { angleMin=-180, angleMax=180, rotAxisX=1, rotAxisY=0, rotAxisZ=0, rotPointX=1405, rotPointY=0000, rotPointZ=1765 }, Color=null } },

            {00,new JoinConfig { Id=00, Coord=new coords() { angleMin=-180, angleMax=180, rotAxisX=0, rotAxisY=0, rotAxisZ=1, rotPointX=0000, rotPointY=0000, rotPointZ=0000 }, Color=null } },
            {01,new JoinConfig { Id=01, Coord=new coords() { angleMin=-090, angleMax=090, rotAxisX=0, rotAxisY=1, rotAxisZ=0, rotPointX=0175, rotPointY=-200, rotPointZ=0500 }, Color=null } },
            {02,new JoinConfig { Id=02, Coord=new coords() { angleMin=-180, angleMax=080, rotAxisX=0, rotAxisY=1, rotAxisZ=0, rotPointX=0190, rotPointY=-700, rotPointZ=1595 }, Color=null } },
            {03,new JoinConfig { Id=03, Coord=new coords() { angleMin=-180, angleMax=180, rotAxisX=1, rotAxisY=0, rotAxisZ=0, rotPointX=0400, rotPointY=0000, rotPointZ=1765 }, Color=null } },
            {04,new JoinConfig { Id=04, Coord=new coords() { angleMin=-115, angleMax=115, rotAxisX=0, rotAxisY=1, rotAxisZ=0, rotPointX=1405, rotPointY=0050, rotPointZ=1765 }, Color=null } },
            {05,new JoinConfig { Id=05, Coord=new coords() { angleMin=-180, angleMax=180, rotAxisX=1, rotAxisY=0, rotAxisZ=0, rotPointX=1405, rotPointY=0000, rotPointZ=1765 }, Color=null } },

            {06,new JoinConfig { Id=06, Coord=null, Color=Colors.Red   } },
            {07,new JoinConfig { Id=07, Coord=null, Color=Colors.Black } },
            {08,new JoinConfig { Id=08, Coord=null, Color=Colors.Black } },
            {09,new JoinConfig { Id=09, Coord=null, Color=Colors.Black } },
            {10,new JoinConfig { Id=10, Coord=null, Color=Colors.Gray  } },
        };
        public IRB4600() : base("IRB4600", path, files, joincfg)
        {
        }
    }

    public class IRB6620 : RobotConfig
    {
        private static readonly string path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "\\ArmModels\\IRB6620\\";
        private static readonly List<string> files = new List<string>
        {
            "IRB6620_220-150_m2004_rev0_01-1.STL",
            "IRB6620_220-150_m2004_rev0_01-2.STL",
            "IRB6620_220-150_m2004_rev0_01-3.STL",
            "IRB6620_220-150_m2004_rev0_01-4.STL",
            "IRB6620_220-150_m2004_rev0_01-5.STL",
            "IRB6620_220-150_m2004_rev0_01-6.STL",
            "IRB6620_220-150_m2004_rev0_01-7.STL"
        };
        private static readonly Dictionary<int, JoinConfig> joincfg = new Dictionary<int, JoinConfig>()
        {
            {00,new JoinConfig { Id=00, Coord=new coords() { angleMin=-180, angleMax=180, rotAxisX=0, rotAxisY=0, rotAxisZ=1, rotPointX=0000, rotPointY=0000, rotPointZ=0000 }, Color=null } },
            {01,new JoinConfig { Id=01, Coord=new coords() { angleMin=-100, angleMax=060, rotAxisX=0, rotAxisY=1, rotAxisZ=0, rotPointX=0175, rotPointY=-200, rotPointZ=0500 }, Color=null } },
            {02,new JoinConfig { Id=02, Coord=new coords() { angleMin=-090, angleMax=090, rotAxisX=0, rotAxisY=1, rotAxisZ=0, rotPointX=0190, rotPointY=-700, rotPointZ=1595 }, Color=null } },
            {03,new JoinConfig { Id=03, Coord=new coords() { angleMin=-180, angleMax=180, rotAxisX=1, rotAxisY=0, rotAxisZ=0, rotPointX=0400, rotPointY=0000, rotPointZ=1765 }, Color=null } },
            {04,new JoinConfig { Id=04, Coord=new coords() { angleMin=-115, angleMax=115, rotAxisX=0, rotAxisY=1, rotAxisZ=0, rotPointX=1405, rotPointY=0050, rotPointZ=1765 }, Color=null } },
            {05,new JoinConfig { Id=05, Coord=new coords() { angleMin=-180, angleMax=180, rotAxisX=1, rotAxisY=0, rotAxisZ=0, rotPointX=1405, rotPointY=0000, rotPointZ=1765 }, Color=null } },

            {06,new JoinConfig { Id=06, Coord=null, Color=Colors.Red   } },
            {07,new JoinConfig { Id=07, Coord=null, Color=Colors.Black } },
            {08,new JoinConfig { Id=08, Coord=null, Color=Colors.Black } },
            {09,new JoinConfig { Id=09, Coord=null, Color=Colors.Black } },
            {10,new JoinConfig { Id=10, Coord=null, Color=Colors.Gray  } },
        };
        public IRB6620() : base("IRB6620",path, files, joincfg)
        {
        }
    }

    class IRB6700 : RobotConfig
    {
        private static string path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "\\ArmModels\\IRB6700\\";
        private static readonly List<string> files = new List<string>
        {
            "IRB6700-MH3_245-300_IRC5_rev02_LINK01_CAD.stl",
            "IRB6700-MH3_245-300_IRC5_rev00_LINK02_CAD.stl",
            "IRB6700-MH3_245-300_IRC5_rev02_LINK03_CAD.stl",
            "IRB6700-MH3_245-300_IRC5_rev01_LINK04_CAD.stl",
            "IRB6700-MH3_245-300_IRC5_rev01_LINK05_CAD.stl",
            "IRB6700-MH3_245-300_IRC5_rev01_LINK06_CAD.stl",
            "IRB6700-MH3_245-300_IRC5_rev02_LINK01_CABLE.stl",
            "IRB6700-MH3_245-300_IRC5_rev02_LINK01m_CABLE.stl",
            "IRB6700-MH3_245-300_IRC5_rev00_LINK02_CABLE.stl",
            "IRB6700-MH3_245-300_IRC5_rev00_LINK02m_CABLE.stl",
            "IRB6700-MH3_245-300_IRC5_rev00_LINK03a_CABLE.stl",
            "IRB6700-MH3_245-300_IRC5_rev00_LINK03b_CABLE.stl",
            "IRB6700-MH3_245-300_IRC5_rev02_LINK03m_CABLE.stl",
            "IRB6700-MH3_245-300_IRC5_rev01_LINK04_CABLE.stl",
            "IRB6700-MH3_245-300_IRC5_rev00_ROD_CAD.stl",
            "IRB6700-MH3_245-300_IRC5_rev00_LOGO1_CAD.stl",
            "IRB6700-MH3_245-300_IRC5_rev00_LOGO2_CAD.stl",
            "IRB6700-MH3_245-300_IRC5_rev00_LOGO3_CAD.stl",
            "IRB6700-MH3_245-300_IRC5_rev01_BASE_CAD.stl",
            "IRB6700-MH3_245-300_IRC5_rev00_CYLINDER_CAD.stl"
        };
        private static readonly Dictionary<int, JoinConfig> joincfg = new Dictionary<int, JoinConfig>()
        {
            {00,new JoinConfig { Id=00, Coord=new coords() { angleMin=-180, angleMax=180, rotAxisX=0, rotAxisY=0, rotAxisZ=1, rotPointX=0000, rotPointY=0000, rotPointZ=0000 }, Color=null } },
            {01,new JoinConfig { Id=01, Coord=new coords() { angleMin=-100, angleMax=060, rotAxisX=0, rotAxisY=1, rotAxisZ=0, rotPointX=0348, rotPointY=-243, rotPointZ=0775 }, Color=null } },
            {02,new JoinConfig { Id=02, Coord=new coords() { angleMin=-090, angleMax=090, rotAxisX=0, rotAxisY=1, rotAxisZ=0, rotPointX=0347, rotPointY=-376, rotPointZ=1923 }, Color=null } },
            {03,new JoinConfig { Id=03, Coord=new coords() { angleMin=-180, angleMax=180, rotAxisX=1, rotAxisY=0, rotAxisZ=0, rotPointX=0060, rotPointY=0000, rotPointZ=2125 }, Color=null } },
            {04,new JoinConfig { Id=04, Coord=new coords() { angleMin=-115, angleMax=115, rotAxisX=0, rotAxisY=1, rotAxisZ=0, rotPointX=1815, rotPointY=0000, rotPointZ=2125 }, Color=null } },
            {05,new JoinConfig { Id=05, Coord=new coords() { angleMin=-180, angleMax=180, rotAxisX=1, rotAxisY=0, rotAxisZ=0, rotPointX=2008, rotPointY=0000, rotPointZ=2125 }, Color=null } },

            {06,new JoinConfig { Id=06, Coord=null, Color=Colors.DarkSlateGray } },
            {07,new JoinConfig { Id=07, Coord=null, Color=Colors.DarkSlateGray } },
            {08,new JoinConfig { Id=08, Coord=null, Color=Colors.DarkSlateGray } },
            {09,new JoinConfig { Id=09, Coord=null, Color=Colors.DarkSlateGray } },
            {10,new JoinConfig { Id=10, Coord=null, Color=Colors.DarkSlateGray } },
            {11,new JoinConfig { Id=11, Coord=null, Color=Colors.DarkSlateGray } },
            {12,new JoinConfig { Id=12, Coord=null, Color=Colors.DarkSlateGray } },
            {13,new JoinConfig { Id=13, Coord=null, Color=Colors.DarkSlateGray } },

            {14,new JoinConfig { Id=14, Coord=null, Color=Colors.Gray } },
            {15,new JoinConfig { Id=15, Coord=null, Color=Colors.Red } },
            {16,new JoinConfig { Id=16, Coord=null, Color=Colors.Red } },
            {17,new JoinConfig { Id=17, Coord=null, Color=Colors.Red } },
            {18,new JoinConfig { Id=18, Coord=null, Color=Colors.Gray } },
            {19,new JoinConfig { Id=19, Coord=null, Color=Colors.Gray } },
    };
        public IRB6700() : base("IRB6700",path, files, joincfg)
        {
        }
    }

    public class coords
    {
        public double angleMin { get; set; }
        public double angleMax { get; set; }
        public int rotAxisX { get; set; }
        public int rotAxisY { get; set; }
        public int rotAxisZ { get; set; }
        public int rotPointX { get; set; }
        public int rotPointY { get; set; }
        public int rotPointZ { get; set; }

        public coords()
        {
            this.angleMin = -180;
            this.angleMax = 180;
            this.rotAxisX = 0;
            this.rotAxisY = 0;
            this.rotAxisZ = 0;
            this.rotPointX = 0;
            this.rotPointY = 0;
            this.rotPointZ = 0;
        }

        public coords(double angleMin, double angleMax, int rotAxisX, int rotAxisY, int rotAxisZ, int rotPointX, int rotPointY, int rotPointZ)
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
    }

    public class JoinConfig
    {
        public int Id { get; set; }
        public coords Coord { get; set; }
        public Color? Color { get; set; }

        public JoinConfig()
        {
            this.Id = 0;
            this.Coord = new coords();
            this.Color = null;
        }

        public JoinConfig(int id, coords coord, Color color)
        {
            this.Id = id;
            this.Coord = coord;
            this.Color = color;
        }
    }
}
