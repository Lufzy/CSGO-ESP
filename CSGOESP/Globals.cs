using Memorys;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CSGOESP
{
    class Globals
    {
        public static int Client = -1, Engine = -1;

        public static bool ESP = true, Teammate = true, Enemy = true;
        public static Color teammateColor = Color.Blue, enemyColor = Color.Red;

        public static int GetBoneMatrixAddr(int BaseAddr)
        {
            return bufferByte.Read<int>(BaseAddr + netvars.m_dwBoneMatrix);
        }

        public static Vector3 GetBonePos(int BoneMatrixAddr, int TargetBone)
        {
            Vector3 temp;
            temp.X = bufferByte.Read<float>(BoneMatrixAddr + 0x30 * TargetBone + 0x0C);
            temp.Y = bufferByte.Read<float>(BoneMatrixAddr + 0x30 * TargetBone + 0x1C);
            temp.Z = bufferByte.Read<float>(BoneMatrixAddr + 0x30 * TargetBone + 0x2C);

            return temp;
        }
    }
}
