using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExtendedXmlSerialization;
using PDIWT_MS_CZ.Models;

namespace TestEntendedXmlSerializer
{
    class Program
    {
        static void Main(string[] args)
        {
            var lockheadParameters = new LockHeadParameters
            {
                LH_BaseBoard = new BaseBoard
                {
                    BaseBoardLength = 1000,
                    BaseBoardHeight = 1000,
                    BaseBoardWidth = 1000,
                    EntranceWidth = 1000,
                    IsGrooving = true,
                    IGrooving = new ShapeIGrooving
                    {
                        GroovingBootomLength = 100,
                        GroovingGradient = 100,
                        GroovingHeight = 100,
                        GroovingTopLength = 11
                    },
                    TGrooving = new ShapeTGrooving
                    {
                        GroovingHeight = 100,
                        GroovingGradient = 100,
                        GroovingBackLength = 1100,
                        GroovingFrontLength = 100,
                        GroovingWidth = 12
                    }
                },
                LH_SidePier = new SidePier
                {
                    PierXY_A = 1,
                    PierXY_B = 2,
                    PierXY_C = 3,
                    PierXY_D = 5,
                    PierXY_E = 6,
                    PierXY_F = 7,
                    PierHeight = 8,
                    PierChamfer_Tx = 9,
                    PierChamfer_Ty = 10,
                    PierChamfer_R = 11
                },
                LH_EmptyRectBoxs = new List<EmptyRectBox>
                {
                    new EmptyRectBox
                    {
                        XDis = 1,
                        YDis = 2,
                        ZDis = 3,
                        EmptyBoxHeight = 4,
                        EmptyBoxLength = 5,
                        EmptyBoxWidth = 6,
                        ChameferInfos = new List<EmptyBoxEdgeChameferInfo>
                        {
                            new EmptyBoxEdgeChameferInfo()
                            {
                                EdgeFlag = EdgeIndicator.right,
                                ChamferLength = 2,
                                ChamferWidth = 3
                            },
                            new EmptyBoxEdgeChameferInfo()
                            {
                                EdgeFlag = EdgeIndicator.y5,
                                ChamferLength = 2,
                                ChamferWidth = 3
                            },
                            new EmptyBoxEdgeChameferInfo()
                            {
                                EdgeFlag = EdgeIndicator.x1,
                                ChamferLength = 2,
                                ChamferWidth = 3
                            }
                        }
                    },
                    new EmptyRectBox()
                    {
                        XDis = 1,
                        YDis = 2,
                        ZDis = 3,
                        EmptyBoxHeight = 4,
                        EmptyBoxLength = 5,
                        EmptyBoxWidth = 6,
                        ChameferInfos =new List<EmptyBoxEdgeChameferInfo>
                        {
                            new EmptyBoxEdgeChameferInfo()
                            {
                                EdgeFlag =  EdgeIndicator.all,
                                ChamferLength = 2,
                                ChamferWidth = 3
                            },
                            new EmptyBoxEdgeChameferInfo()
                            {
                                EdgeFlag = EdgeIndicator.back,
                                ChamferLength = 2,
                                ChamferWidth = 3
                            },
                            new EmptyBoxEdgeChameferInfo()
                            {
                                EdgeFlag = EdgeIndicator.bottom,
                                ChamferLength = 2,
                                ChamferWidth = 3
                            }
                        }
                    }
                },
                LH_EmptyZPlanBoxs = new List<EmptyZPlanBox>
                {
                    new EmptyZPlanBox
                    {
                        XDis =1,
                        YDis=2,
                        ZDis =3,
                        EmptyBoxHeight =4,
                        Point2Ds = new List<Bentley.GeometryNET.DPoint2d>()
                        {
                            new Bentley.GeometryNET.DPoint2d(1,1),
                            new Bentley.GeometryNET.DPoint2d(2,2),
                            new Bentley.GeometryNET.DPoint2d(3,3)
                        },
                        ChameferInfos = new List<EmptyBoxEdgeChameferInfo>
                        {
                            new EmptyBoxEdgeChameferInfo
                            {
                                EdgeFlag = EdgeIndicator.none,
                                ChamferLength =1,
                                ChamferWidth=1
                            },
                            new EmptyBoxEdgeChameferInfo
                            {
                                EdgeFlag = EdgeIndicator.x0,
                                ChamferLength =2,
                                ChamferWidth=2
                            },
                            new EmptyBoxEdgeChameferInfo
                            {
                                EdgeFlag = EdgeIndicator.xplan,
                                ChamferLength =3,
                                ChamferWidth=3
                            },
                        }
                    },
                    new EmptyZPlanBox
                    {
                        XDis =1,
                        YDis=2,
                        ZDis =3,
                        EmptyBoxHeight =4,
                        Point2Ds = new List<Bentley.GeometryNET.DPoint2d>()
                        {
                            new Bentley.GeometryNET.DPoint2d(1,1),
                            new Bentley.GeometryNET.DPoint2d(2,2),
                            new Bentley.GeometryNET.DPoint2d(3,3)
                        },
                        ChameferInfos = new List<EmptyBoxEdgeChameferInfo>
                        {
                            new EmptyBoxEdgeChameferInfo
                            {
                                EdgeFlag = EdgeIndicator.y6,
                                ChamferLength =1,
                                ChamferWidth=1
                            },
                            new EmptyBoxEdgeChameferInfo
                            {
                                EdgeFlag = EdgeIndicator.back,
                                ChamferLength =2,
                                ChamferWidth=2
                            },
                            new EmptyBoxEdgeChameferInfo
                            {
                                EdgeFlag = EdgeIndicator.right,
                                ChamferLength =3,
                                ChamferWidth=3
                            },
                        }
                    }
                },
                LH_DoorSill = new DoorSill
                {
                    DoorSill_A = 1,
                    DoorSill_B = 2,
                    DoorSill_C = 3,
                    DoorSill_D = 4,
                    DoorSill_E = 5,
                    DoorSill_F = 6,
                    DoorSillHeight = 7
                },
                LH_LocalConcertationCulvert = new LocalConcertationCulvert
                {
                    Culvert_Height = 1,
                    Culvert_A = 2,
                    Culvert_B = 3,
                    Culvert_C = 4,
                    Culvert_D = 5,
                    Culvert_E = 6,
                    Culvert_F = 7,
                    Culvert_Chamfer_R1 = 8,
                    Culvert_Chamfer_R2 = 9,
                    Culvert_Chamfer_R3 = 10,
                    Culvert_Chamfer_R4 = 11,
                    Culvert_Pier_BackDis = 12,
                    WaterDivision_A = 13,
                    WaterDivision_B = 14,
                    WaterDivision_R1 = 15,
                    WaterDivision_R2 = 16,
                    WaterDivision_R3 = 17
                },
                LH_ShortCulvert = new ShortCulvert
                {
                    Culvert_Width = 1,
                    Culvert_A = 2,
                    Culvert_B = 3,
                    Culvert_C = 4,
                    Culvert_D = 5,
                    Culvert_E = 6,
                    Culvert_R1 = 7,
                    Culvert_R2 = 8,
                    Culvert_Pier_BackDis = 9
                },
                LH_EnergyDisspater = new EnergyDisspater
                {
                    Grille_TwolineInterval = 1,
                    GrillWidthList = { 1, 2, 4, 5, 6 }
                },
                LH_Baffle = new Baffle
                {
                    Baffle_Height = 1,
                    Baffle_Width = 2,
                    Baffle_MidMidDis = 3
                }
            };
            ExtendedXmlSerializer seriliSerializer = new ExtendedXmlSerializer();
            var xml = seriliSerializer.Serialize(lockheadParameters);
            Console.WriteLine(xml);
            File.WriteAllText("text.xml",xml);
            Console.ReadKey();
        }
    }
}
