using Memorys;
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
            Timer Resize = new Timer();
            Resize.Tick += new EventHandler(Resize_Tick);
            Resize.Interval = 1;
            Resize.Start();

            Timer onlyFocus = new Timer();
            onlyFocus.Tick += new EventHandler(onlyFocus_Tick);
            onlyFocus.Interval = 10;
            onlyFocus.Start();

            Timer onlyWindow = new Timer();
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

            if(Globals.ESP) // 01:44
            {
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
                        else if(team != hisTeam && Globals.Enemy)
                        {
                            Vector3 v3position = bufferByte.Read<Vector3>(entity + netvars.m_vecOrigin);
                            Vector2 v2position = bufferByte.WorldToScreen(new Vector3(v3position.X, v3position.Y, v3position.Z /*- 5*/), Overlay.Width, Overlay.Height, bufferByte.ReadMatrix<float>(Globals.Client + signatures.dwViewMatrix, 16));
                            Vector2 v2headpos = bufferByte.WorldToScreen(Globals.GetBonePos(Globals.GetBoneMatrixAddr(entity), 8)/*new Vector3(v3position.X, v3position.Y, v3position.Z + 10)*/, Overlay.Width, Overlay.Height, bufferByte.ReadMatrix<float>(Globals.Client + signatures.dwViewMatrix, 16));

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

        private static void onlyFocus_Tick(object sender, EventArgs e)
        {
            Process[] p = Process.GetProcessesByName("csgo");
            // If Notting Found (Length = 0)
            if (p.Length == 0)
            {
                Application.Exit();
                Environment.Exit(0);
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
