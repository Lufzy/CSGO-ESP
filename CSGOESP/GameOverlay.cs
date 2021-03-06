﻿using Memorys;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSGOESP
{
    class GameOverlay
    {
        public static BufferedGraphics bufferedGraphics;
        public static Graphics g;
        public static Font font = new Font(FontFamily.GenericSansSerif, 14, FontStyle.Bold);

        public static RECT rect;
        //public const string WINDOW_NAME = "League of Legends (TM) Client";
        public const string WINDOW_NAME = "Counter-Strike: Global Offensive";
        public static IntPtr handle = Imports.FindWindow(null, WINDOW_NAME);

        public struct RECT
        {
            public int left, top, right, bottom;
        }

        public class Imports
        {
            [DllImport("user32.dll")]
            public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

            [DllImport("user32.dll", SetLastError = true)]
            public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

            [DllImport("user32.dll", SetLastError = true)]
            public static extern IntPtr FindWindow(string ipClassName, string ipWindowName);

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

            [DllImport("user32.dll")]
            public static extern IntPtr GetForegroundWindow();

            [DllImport("user32.dll")]
            public static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

            [DllImport("user32.dll")]
            public static extern short GetAsyncKeyState(System.Windows.Forms.Keys vKey);
        }

        public static Form Overlay = new Form();
        public static void InıtOverlay()
        {
            Overlay.BackColor = Color.FromArgb(2, 2, 2); //System.Drawing.Color.Black; Daha iyi Gözükür
            Overlay.TransparencyKey = Color.FromArgb(2, 2, 2); //System.Drawing.Color.Black; Daha iyi Gözükür
            Overlay.TopMost = true; //açıldığında devamlı önde kalmasını sağlar
            Overlay.FormBorderStyle = FormBorderStyle.None; //programın penceri kısmını siler
            Overlay.ShowInTaskbar = false; //görev çubuğunda gözükmemesini sağlar
            Overlay.Paint += new PaintEventHandler(DrawingEvent);

            System.Reflection.PropertyInfo prop =
             typeof(System.Windows.Forms.Control).GetProperty(
             "DoubleBuffered",
             System.Reflection.BindingFlags.NonPublic |
             System.Reflection.BindingFlags.Instance);
            prop.SetValue(Overlay, true, null);

            int initialStyle = Imports.GetWindowLong(Overlay.Handle, -20);
            Imports.SetWindowLong(Overlay.Handle, -20, initialStyle | 0x80000 | 0x20); //mousenin formun içinde geçmesini sağlar

            Imports.GetWindowRect(handle, out rect);
            Overlay.Size = new Size(rect.right - rect.left, rect.bottom - rect.top);
            Overlay.Top = rect.top;
            Overlay.Left = rect.left;

            #region Timers
            System.Windows.Forms.Timer Resize = new System.Windows.Forms.Timer();
            Resize.Tick += new EventHandler(Resize_Tick);
            Resize.Interval = 1;
            Resize.Start();

            System.Windows.Forms.Timer onlyFocus = new System.Windows.Forms.Timer();
            onlyFocus.Tick += new EventHandler(onlyFocus_Tick);
            onlyFocus.Interval = 10;
            onlyFocus.Start();

            System.Windows.Forms.Timer onlyWindow = new System.Windows.Forms.Timer();
            onlyWindow.Tick += new EventHandler(onlyWindow_Tick);
            onlyWindow.Interval = 500;
            onlyWindow.Start();
            #endregion

            Overlay.Show();
        }

        private static void DrawingEvent(object sender, PaintEventArgs e)
        {
            //initialize graphics
            g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.CompositingQuality = CompositingQuality.HighQuality;

            if(Globals.ESP)
            {
                if(Globals.Watermark)
                {
                    if(Globals.WatermarkBG)
                    {
                        g.FillRectangle(new SolidBrush(Color.FromArgb(100, Globals.WatermarkBGColor)), 10, 10, 530, 165);
                        g.DrawRectangle(new Pen(Color.FromArgb(100, Color.White)), 10, 10, 530, 165);
                    }

                    DrawStringOutlined("CSGOESP | free-software | " + DateTime.Now.ToString(), new Point(15, 15), new Font("Consolas", 15), Brushes.White, Pens.Black);

                    DrawStringOutlined("---------------------------------------------", new Point(15, 30), new Font("Consolas", 15), Brushes.White, Pens.Black);
                    //DrawStringOutlined("------------------------|--------------------", new Point(15, 30), new Font("Consolas", 15), Brushes.White, Pens.Black);
                    if(Globals.BoxESP)
                    {
                        DrawStringOutlined("BoxESP             [F6] | " + Globals.BoxESP.ToString(), new Point(15, 45), new Font("Consolas", 15), Brushes.Lime, Pens.Black);
                    }
                    else
                    {
                        DrawStringOutlined("BoxESP             [F6] | " + Globals.BoxESP.ToString(), new Point(15, 45), new Font("Consolas", 15), Brushes.Red, Pens.Black);
                    }
                    //DrawStringOutlined("BoxESP             [F5] | " + Globals.BoxESP.ToString(), new Point(15, 45), new Font("Consolas", 15), Brushes.White, Pens.Black);

                    if(Globals.SkeletonESP)
                    {
                        DrawStringOutlined("SkeletonESP        [F7] | " + Globals.SkeletonESP.ToString(), new Point(15, 65), new Font("Consolas", 15), Brushes.Lime, Pens.Black);
                    }
                    else
                    {
                        DrawStringOutlined("SkeletonESP        [F7] | " + Globals.SkeletonESP.ToString(), new Point(15, 65), new Font("Consolas", 15), Brushes.Red, Pens.Black);
                    }
                    //DrawStringOutlined("SkeletonESP        [F6] | " + Globals.SkeletonESP.ToString(), new Point(15, 65), new Font("Consolas", 15), Brushes.White, Pens.Black);

                    if(Globals.Snapline)
                    {
                        DrawStringOutlined("Snaplines          [F8] | " + Globals.Snapline.ToString(), new Point(15, 85), new Font("Consolas", 15), Brushes.Lime, Pens.Black);
                    }
                    else
                    {
                        DrawStringOutlined("Snaplines          [F8] | " + Globals.Snapline.ToString(), new Point(15, 85), new Font("Consolas", 15), Brushes.Red, Pens.Black);
                    }
                    //DrawStringOutlined("Snaplines          [F7] | " + Globals.Snapline.ToString(), new Point(15, 85), new Font("Consolas", 15), Brushes.White, Pens.Black);

                    DrawStringOutlined("---------------------------------------------", new Point(15, 100), new Font("Consolas", 15), Brushes.White, Pens.Black);
                    DrawStringOutlined("github.com/Lufzy/CSGO-ESP", new Point(15, 115), new Font("Consolas", 15), Brushes.White, Pens.Black);
                    DrawStringOutlined("Offsets Date : 18.09.2020", new Point(15, 135), new Font("Consolas", 15), Brushes.White, Pens.Black);
                    DrawStringOutlined("---------------------------------------------", new Point(15, 150), new Font("Consolas", 15), Brushes.White, Pens.Black);
                }

                int engine = bufferByte.Read<int>(Globals.Engine + signatures.dwClientState);
                int gameState = bufferByte.Read<int>(engine + signatures.dwClientState_State);
                if(gameState == 6)
                {
                    int maxPlayer = bufferByte.Read<int>(engine + signatures.dwClientState_MaxPlayer);
                    int localPlayer = bufferByte.Read<int>(Globals.Client + signatures.dwLocalPlayer);
                    int team = bufferByte.Read<int>(localPlayer + netvars.m_iTeamNum);
                    int health = bufferByte.Read<int>(localPlayer + netvars.m_iTeamNum);

                    for (int i = 0; i < maxPlayer; i++)
                    {
                        int entity = bufferByte.Read<int>(Globals.Client + signatures.dwEntityList + i * 0x10);
                        if (entity <= 0) continue;
                        if (entity == localPlayer) continue;
                        int hishealth = bufferByte.Read<int>(entity + netvars.m_iHealth);
                        if (hishealth <= 0) continue;
                        int hisTeam = bufferByte.Read<int>(entity + netvars.m_iTeamNum);
                        

                        if(team == hisTeam && Globals.Teammate) // if teammate
                        {
                            Vector3 v3position = bufferByte.Read<Vector3>(entity + netvars.m_vecOrigin);
                            Vector2 v2position = bufferByte.WorldToScreen(new Vector3(v3position.X, v3position.Y, v3position.Z - 5), Overlay.Width, Overlay.Height, bufferByte.ReadMatrix<float>(Globals.Client + signatures.dwViewMatrix, 16));
                            Vector2 v2headpos = bufferByte.WorldToScreen(Globals.GetBonePos(Globals.GetBoneMatrixAddr(entity), 8)/*new Vector3(v3position.X, v3position.Y, v3position.Z + 10)*/, Overlay.Width, Overlay.Height, bufferByte.ReadMatrix<float>(Globals.Client + signatures.dwViewMatrix, 16)); // bone id : https://www.unknowncheats.me/forum/attachments/counterstrike-global-offensive/13413d1480413236-csgo-bone-id-8f0bb9a93378477388dee312b2fad4ca-png

                            if (Globals.Snapline && v2position != new Vector2(0, 0))
                            {
                                g.DrawLine(new Pen(Globals.teammateColor), Overlay.Width / 2, Overlay.Height, v2position.X, v2position.Y);
                            }
                            if (Globals.BoxESP && v2position != new Vector2(0, 0))
                            {
                                float BoxHeight = v2position.Y - v2headpos.Y;
                                float BoxWidth = (BoxHeight / 2) * 1.25f; //little bit wider box
                                DrawBorderedRectangle(new Pen(Globals.teammateColor), Pens.Black, v2position.X - (BoxWidth / 2), v2headpos.Y, BoxWidth, BoxHeight);
                                try
                                {
                                    Point textPos = new Point((int)v2position.X - (int)(BoxWidth / 2), (int)v2headpos.Y);
                                    //DrawStringOutlined("Mate", textPos, new Font("Consolas", BoxWidth / 8 + 1), Brushes.White, Pens.Black);
                                    DrawStringOutlined("Health : " + hishealth.ToString(), textPos, new Font("Consolas", BoxWidth / 16 + 1), Brushes.White, Pens.Black);
                                }
                                catch { }
                            }

                            if(Globals.SkeletonESP) // buggly | https://www.unknowncheats.me/forum/counterstrike-global-offensive/218227-skeleton-esp-bone-ids.html
                            {
                                // skeleton esp | Bone Ids : https://www.unknowncheats.me/forum/attachments/counterstrike-global-offensive/13413d1480413236-csgo-bone-id-8f0bb9a93378477388dee312b2fad4ca-png

                                DrawBone(8, 7, entity, new Pen(Globals.teammateColor)); // Neck
                                DrawBone(7, 41, entity, new Pen(Globals.teammateColor)); // Left Shoulder
                                DrawBone(7, 11, entity, new Pen(Globals.teammateColor)); // Right Shoulder
                                DrawBone(41, 42, entity, new Pen(Globals.teammateColor)); // Left Arm 1
                                DrawBone(11, 12, entity, new Pen(Globals.teammateColor)); // Right Arm 1
                                DrawBone(42, 43, entity, new Pen(Globals.teammateColor)); // Left Arm 2
                                DrawBone(12, 13, entity, new Pen(Globals.teammateColor)); // Right Arm 2
                                DrawBone(7, 6, entity, new Pen(Globals.teammateColor)); // Upper Body
                                DrawBone(6, 5, entity, new Pen(Globals.teammateColor)); // Middle Body
                                DrawBone(5, 3, entity, new Pen(Globals.teammateColor)); // Lower Body
                                DrawBone(3, 77, entity, new Pen(Globals.teammateColor)); // Left Pelvis
                                DrawBone(3, 70, entity, new Pen(Globals.teammateColor)); // Right Pelvis
                                DrawBone(77, 78, entity, new Pen(Globals.teammateColor)); // Left Leg
                                DrawBone(70, 71, entity, new Pen(Globals.teammateColor)); // Right Leg
                                DrawBone(78, 79, entity, new Pen(Globals.teammateColor)); // Left Shin
                                DrawBone(71, 72, entity, new Pen(Globals.teammateColor)); // Right Shin
                            }
                        }
                        else if(team != hisTeam && Globals.Enemy)
                        {
                            Vector3 v3position = bufferByte.Read<Vector3>(entity + netvars.m_vecOrigin);
                            Vector2 v2position = bufferByte.WorldToScreen(new Vector3(v3position.X, v3position.Y, v3position.Z /*- 5*/), Overlay.Width, Overlay.Height, bufferByte.ReadMatrix<float>(Globals.Client + signatures.dwViewMatrix, 16));
                            Vector2 v2headpos = bufferByte.WorldToScreen(Globals.GetBonePos(Globals.GetBoneMatrixAddr(entity), 8)/*new Vector3(v3position.X, v3position.Y, v3position.Z + 10)*/, Overlay.Width, Overlay.Height, bufferByte.ReadMatrix<float>(Globals.Client + signatures.dwViewMatrix, 16));

                            if (Globals.Snapline && v2position != new Vector2(0, 0)) 
                            {
                                g.DrawLine(new Pen(Globals.enemyColor), Overlay.Width / 2, Overlay.Height, v2position.X, v2position.Y);
                            }
                            if(Globals.BoxESP && v2position != new Vector2(0, 0))
                            {
                                float BoxHeight = v2position.Y - v2headpos.Y;
                                float BoxWidth = (BoxHeight / 2) * 1.25f; //little bit wider box

                                DrawBorderedRectangle(new Pen(Globals.enemyColor), Pens.Black, v2position.X - (BoxWidth / 2), v2headpos.Y, BoxWidth, BoxHeight);
                                try
                                {
                                    Point textPos = new Point((int)v2position.X - (int)(BoxWidth / 2), (int)v2headpos.Y);
                                    //DrawStringOutlined("Enemy", new Point((int)v2position.X - (int)(BoxWidth / 2), (int)v2headpos.Y), new Font("Consolas", BoxWidth / 8 + 1), Brushes.White, Pens.Black);
                                    DrawStringOutlined("Health : " + hishealth.ToString(), textPos, new Font("Consolas", BoxWidth / 16 + 1), Brushes.White, Pens.Black);
                                }
                                catch { }
                            }

                            if(Globals.SkeletonESP) // buggly | https://www.unknowncheats.me/forum/counterstrike-global-offensive/218227-skeleton-esp-bone-ids.html
                            {
                                // skeleton esp | Bone Ids : https://www.unknowncheats.me/forum/attachments/counterstrike-global-offensive/13413d1480413236-csgo-bone-id-8f0bb9a93378477388dee312b2fad4ca-png

                                DrawBone(8, 7, entity, new Pen(Globals.enemyColor)); // Neck
                                DrawBone(7, 41, entity, new Pen(Globals.enemyColor)); // Left Shoulder
                                DrawBone(7, 11, entity, new Pen(Globals.enemyColor)); // Right Shoulder
                                DrawBone(41, 42, entity, new Pen(Globals.enemyColor)); // Left Arm 1
                                DrawBone(11, 12, entity, new Pen(Globals.enemyColor)); // Right Arm 1
                                DrawBone(42, 43, entity, new Pen(Globals.enemyColor)); // Left Arm 2
                                DrawBone(12, 13, entity, new Pen(Globals.enemyColor)); // Right Arm 2
                                DrawBone(7, 6, entity, new Pen(Globals.enemyColor)); // Upper Body
                                DrawBone(6, 5, entity, new Pen(Globals.enemyColor)); // Middle Body
                                DrawBone(5, 3, entity, new Pen(Globals.enemyColor)); // Lower Body
                                DrawBone(3, 77, entity, new Pen(Globals.enemyColor)); // Left Pelvis
                                DrawBone(3, 70, entity, new Pen(Globals.enemyColor)); // Right Pelvis
                                DrawBone(77, 78, entity, new Pen(Globals.enemyColor)); // Left Leg
                                DrawBone(70, 71, entity, new Pen(Globals.enemyColor)); // Right Leg
                                DrawBone(78, 79, entity, new Pen(Globals.enemyColor)); // Left Shin
                                DrawBone(71, 72, entity, new Pen(Globals.enemyColor)); // Right Shin
                            }
                        }
                    }
                }
            }
        }

        public static void DrawBorderedRectangle(Pen mainColor, Pen borderColor, float x, float y, float width, float height)
        {
            g.DrawRectangle(mainColor, x, y, width, height);
            //g.DrawRectangle(borderColor, x - 1, x - 1, width + 2, height + 2);
            //g.DrawRectangle(borderColor, x + 1, x + 1, width - 2, height - 2);
        }

        public static void DrawStringOutlined(string text, Point pos, Font font, Brush colorText, Pen colorOutline)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddString(text,
                font.FontFamily, (int)font.Style,
                g.DpiY * font.Size / 72, // convert to em size
                pos, new StringFormat());
            g.DrawPath(colorOutline, path);
            g.FillPath(colorText, path);
        }

        public static void DrawBone(int bone, int bone1, int entity, Pen color)
        {
            Vector3 v3Bone = Globals.GetBonePos(Globals.GetBoneMatrixAddr(entity), bone);
            Vector3 v3Bone1 = Globals.GetBonePos(Globals.GetBoneMatrixAddr(entity), bone1);
            Vector2 v2Bone = bufferByte.WorldToScreen(v3Bone, Overlay.Width, Overlay.Height, bufferByte.ReadMatrix<float>(Globals.Client + signatures.dwViewMatrix, 16));
            Vector2 v2Bone1 = bufferByte.WorldToScreen(v3Bone1, Overlay.Width, Overlay.Height, bufferByte.ReadMatrix<float>(Globals.Client + signatures.dwViewMatrix, 16));
            g.DrawLine(color, v2Bone.X, v2Bone.Y, v2Bone1.X, v2Bone1.Y);
        }

        private static void Resize_Tick(object sender, EventArgs e)
        {
            Imports.GetWindowRect(handle, out rect);
            Overlay.Size = new Size(rect.right - rect.left, rect.bottom - rect.top);
            Overlay.Top = rect.top;
            Overlay.Left = rect.left;

            Overlay.Invalidate();
        }

        private static string GetActiveWindowTitle()
        {
            const int nChars = 256;
            StringBuilder Buff = new StringBuilder(nChars);
            IntPtr handle = Imports.GetForegroundWindow();

            if (Imports.GetWindowText(handle, Buff, nChars) > 0)
            {
                return Buff.ToString();
            }
            return null;
        }

        public static bool IsKeyPushedDown(System.Windows.Forms.Keys vKey)
        {
            return 0 != (Imports.GetAsyncKeyState(vKey) & 0x8000);
        }


        private static void onlyFocus_Tick(object sender, EventArgs e)
        {
            Process[] p = Process.GetProcessesByName("csgo");
            // If Notting Found (Length = 0)
            if (p.Length == 0)
            {
                Application.Exit();
                Environment.Exit(0);
            }

            if(IsKeyPushedDown(Keys.F6))
            {
                Globals.BoxESP = !Globals.BoxESP;
                Thread.Sleep(100);
                Console.Beep(500, 500);
            }
            else if (IsKeyPushedDown(Keys.F7))
            {
                Globals.SkeletonESP = !Globals.SkeletonESP;
                Thread.Sleep(100);
                Console.Beep(500, 500);
            }
            else if(IsKeyPushedDown(Keys.F8))
            {
                Globals.Snapline = !Globals.Snapline;
                Thread.Sleep(100);
                Console.Beep(500, 500);
            }
        }

        private static void onlyWindow_Tick(object sender, EventArgs e)
        {
            if (GetActiveWindowTitle() == WINDOW_NAME)
            {
                Overlay.TopMost = true;
            }
            else
            {
                Overlay.TopMost = false;
            }
        }
    }
}
