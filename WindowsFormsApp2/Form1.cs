using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace WindowsFormsApp2
{
    partial class mainForm : Form
    {
        Guitar guitar;
        float f = 0.0f;
        int texture;
        bool loaded = false;
        public mainForm()
        {
            InitializeComponent();            
        }

#region hide
        float Grad = (float)(2 * Math.PI) / 360; //радиан в градусе
        float R = -50;
        bool TrMouseD = false; //нажата ли мышь
        int TrMouseDX; //координата по Х, где нажата мышь
        int TrMouseDY;
        int TrMouseX; //координата по Х, где мышь находится в данный  момент
        int TrMouseY;
        int TrUgolEX; //углы наклона экрана по Х
        int TrUgolEY;
        int TrUgolDEX; //углы наклона экрана по Х в момент нажатия мыши
        int TrUgolDEY;
        /// <summary>
        /// движение мыши
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void glControl1_MouseMove(object sender, MouseEventArgs e)
        {
            TrMouseX = e.X;
            TrMouseY = e.Y;

            if (TrMouseD)
            {
                TrUgolEX = TrUgolDEX + (TrMouseX - TrMouseDX);
                TrUgolEY = TrUgolDEY + (TrMouseY - TrMouseDY);

                //label1.Text = TrUgolEX + " " + TrUgolEY + " " + R;
            }
        }

        /// <summary>
        /// прокрутка колесика мыши
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void glControl1_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta < 0) { R = R - 1; }
            if (e.Delta > 0) { R = R + 1; }
            R = (R < -99) ? -99 : R;
        }
        /// <summary>
        /// нажатие мыши
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void glControl1_MouseDown(object sender, MouseEventArgs e)
        {
            TrMouseD = true;
            TrMouseDX = e.X;
            TrMouseDY = e.Y;
            TrUgolDEX = TrUgolEX;
            TrUgolDEY = TrUgolEY;
        }
        /// <summary>
        /// отпустить мышь
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void glControl1_MouseUp(object sender, MouseEventArgs e)
        {
            TrMouseD = false;
        }

        private void glControl1_Load(object sender, EventArgs e)
        {
            loaded = true;
            GL.ClearColor(Color.SkyBlue);
            GL.Enable(EnableCap.DepthTest);

            Matrix4 p = Matrix4.CreatePerspectiveFieldOfView((float)(80 * Math.PI / 180), 1, 20, 500);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref p);

            Matrix4 modelview = Matrix4.LookAt(0, 0, -100, 0, 0, 0, 0, 1, 0);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref modelview);

            GL.Enable(EnableCap.Lighting); // lighting
            GL.Enable(EnableCap.ColorMaterial);

            float[] light_position = { 20, 20, 50 };
            float[] light_diffuse = { 1.0f, 1.0f, 1.0f };
            GL.Light(LightName.Light0, LightParameter.Position, light_position);
            GL.Light(LightName.Light0, LightParameter.Diffuse, light_diffuse);
            GL.Enable(EnableCap.Light0);

            //texturing
            GL.Enable(EnableCap.Texture2D);
            GL.GenTextures(1, out texture);
            GL.BindTexture(TextureTarget.Texture2D, texture);
            System.Drawing.Imaging.BitmapData texData = loadImage(@"D:\\PGU\\Компьютерная графика\\test\\usingOpenGL\\Swietenia_macrophylla_wood.jpg");
            GL.TexImage2D(TextureTarget.Texture2D, 0, OpenTK.Graphics.OpenGL.PixelInternalFormat.Rgb, texData.Width, texData.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgr, PixelType.UnsignedByte, texData.Scan0);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            loaded = true;//<--------------------------------------
        }

        System.Drawing.Imaging.BitmapData loadImage(string filePath)
        {
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(filePath);
            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height);
            System.Drawing.Imaging.BitmapData bmpdata = bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            bmp.UnlockBits(bmpdata);
            return bmpdata;
        }

#endregion

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            if (!loaded)
                return;

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            guitar = new Guitar();
            guitar.scale(6, 6, 6);
            //guitar.tune(TrUgolEX, TrUgolEY, R);
            guitar.DrawLines();
            guitar.pouring(texture);

            label1.Text = TrUgolEX + " " + TrUgolEY + " " + f;
            f += 0.01f;

            glControl1.SwapBuffers();
        }

        /// <summary>
        /// процедура для управления программой с клавиатуры
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (!loaded) return false;

            switch (keyData)
            {
                case Keys.Up:
                    GL.MatrixMode(MatrixMode.Modelview);
                    GL.Rotate(2, -1, 0, 0);
                    break;
                case Keys.Down:
                    GL.MatrixMode(MatrixMode.Modelview);
                    GL.Rotate(2, 1, 0, 0);
                    break;
                case Keys.Right:
                    GL.MatrixMode(MatrixMode.Modelview);
                    GL.Rotate(2, 0, -1, 0);
                    break;
                case Keys.Left:
                    GL.MatrixMode(MatrixMode.Modelview);
                    GL.Rotate(2, 0, 1, 0);
                    break;
                case Keys.PageUp:
                    GL.MatrixMode(MatrixMode.Modelview);
                    GL.Rotate(2, 0, 0, 1);
                    break;
                case Keys.PageDown:
                    GL.MatrixMode(MatrixMode.Modelview);
                    GL.Rotate(2, 0, 0, -1);
                    break;
            }

            //if (e.KeyCode == Keys.A)
            //{
            //    GL.MatrixMode(MatrixMode.Projection);
            //    GL.Rotate(30, 0, 0, 1);
            //}
            //if (e.KeyCode == Keys.B)
            //{
            //    GL.MatrixMode(MatrixMode.Modelview);
            //    GL.Rotate(30, 0, 0, 1);
            //}

            glControl1.Invalidate();
            return true;
        }
    }
}