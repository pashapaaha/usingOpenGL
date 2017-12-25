using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Platform.Windows;
using OpenTK.Graphics.OpenGL;

namespace WindowsFormsApp2
{
    public class GuitarPart
    {
        public Matrix m;
        protected float linesWidth;
        protected Color color;
        public virtual void DrawLines()
        {
            GL.Color3(Color.Black);
            GL.LineWidth(linesWidth);
        }

        protected void TrUgol(ref float iX, ref float iY, int iU)
        {
            float X, Y, U = ((float)(2 * Math.PI) / 360) * (float)iU;
            X = (float)(iX * Math.Cos(U) - iY * Math.Sin(U));
            Y = (float)(iX * Math.Sin(U) + iY * Math.Cos(U));
            iX = X;
            iY = Y;
        }

        public virtual void tune(int ugolX, int ugolY, float len)
        {
            for (int f = 0; f < m.Row; f++)
            {
                TrUgol(ref m.matrix[f, 1], ref m.matrix[f, 2], ugolY);
                TrUgol(ref m.matrix[f, 0], ref m.matrix[f, 2], ugolX);
                m.matrix[f, 2] -= len;
            } 
        }

        public virtual void scale(float mx, float my, float mz)
        {
            Matrix t = new Matrix(4, 4);
            t.matrix = new float[4, 4]
            {
                {mx, 0, 0, 0 },
                {0, my, 0, 0 },
                {0, 0, mz, 0 },
                {0, 0, 0, 0 }
            };

            m = m.Multiple(t);
        }
    }
    public class GuitarParts: GuitarPart
    {
        public Matrix m1;

        public override void DrawLines()
        {
            base.DrawLines();
            GL.Begin(BeginMode.LineLoop);
            for (int f = 0; f < m.Row; f++)
                GL.Vertex3(m.matrix[f, 0], m.matrix[f, 1], m.matrix[f, 2]);
            GL.End();

            GL.Begin(BeginMode.LineLoop);
            for (int f = 0; f < m1.Row; f++)
                GL.Vertex3(m1.matrix[f, 0], m1.matrix[f, 1], m1.matrix[f, 2]);
            GL.End();

            for (int f = 0; f < m.Row; f++)
            {
                GL.Begin(BeginMode.LineLoop);
                GL.Vertex3(m1.matrix[f, 0], m1.matrix[f, 1], m1.matrix[f, 2]);
                GL.Vertex3(m.matrix[f, 0], m.matrix[f, 1], m.matrix[f, 2]);
                GL.End();
            }
        }
        public override void tune(int ugolX, int ugolY, float len)
        {
            for (int f = 0; f < m.Row; f++)
            {
                TrUgol(ref m.matrix[f, 1], ref m.matrix[f, 2], ugolY);
                TrUgol(ref m.matrix[f, 0], ref m.matrix[f, 2], ugolX);
                m.matrix[f, 2] -= len;
                TrUgol(ref m1.matrix[f, 1], ref m1.matrix[f, 2], ugolY);
                TrUgol(ref m1.matrix[f, 0], ref m1.matrix[f, 2], ugolX);
                m1.matrix[f, 2] -= len;
            }
        }
        public override void scale(float mx, float my, float mz)
        {
            base.scale(mx, my, mz);
            Matrix t = new Matrix(4, 4);
            t.matrix = new float[4, 4]
            {
                {mx, 0, 0, 0 },
                {0, my, 0, 0 },
                {0, 0, mz, 0 },
                {0, 0, 0, 0 }
            };
            
            m1 = m1.Multiple(t);
        }
        public virtual void pouring(int texture)
        {
            GL.Color3(color);
            GL.BindTexture(TextureTarget.Texture2D, texture);
            for (int f = 0; f < m.Row; f++)
            {
                switch (f % 4)
                {
                    case 0:
                        GL.Normal3(1.0, 0.0, 0.0);
                        break;
                    case 1:
                        GL.Normal3(0.0, -1.0, 0.0);
                        break;
                    case 2:
                        GL.Normal3(-1.0, 0.0, 0.0);
                        break;
                    case 3:
                        GL.Normal3(0.0, 1.0, 0.0);
                        break;

                }
            
                GL.Begin(BeginMode.Quads);
                GL.TexCoord2(0, 0);
                GL.Vertex3(m1.matrix[f, 0], m1.matrix[f, 1], m1.matrix[f, 2]);
                GL.TexCoord2(1, 0);
                GL.Vertex3(m1.matrix[(f+1)%m.Row, 0], m1.matrix[(f+1)%m.Row, 1], m1.matrix[(f+1)%m.Row, 2]);
                GL.TexCoord2(1, 1);
                GL.Vertex3(m.matrix[(f+1)%m.Row, 0], m.matrix[(f+1)%m.Row, 1], m.matrix[(f+1)%m.Row, 2]);
                GL.TexCoord2(0, 1);
                GL.Vertex3(m.matrix[f, 0], m.matrix[f, 1], m.matrix[f, 2]);
                GL.End();
            }
        }
    }
    class GuitarBody : GuitarParts
    {
        public GuitarBody()
        {
            color = Color.Gray;
            linesWidth = 1;
            m = new Matrix(8, 4);
            m.matrix = new float[8, 4] { 
                {-11.5f,    4.0f,   0f, 0},
                {-7,        -0.5f,  0f, 0},
                {-8,        -3.5f,  0f, 0},
                {-3.5f,     -1.5f,  0f, 0},
                {1.5f,      -2.5f,  0f, 0},
                {-1,        0.5f,   0f, 0},
                {0.5f,      4,      0f, 0},
                {-4.5f,     2,      0f, 0} };

            m1 = new Matrix(8, 4);
            m1.matrix = new float[8, 4] {
                {-11.5f,    4.0f,   1f, 0},
                {-7,        -0.5f,  1f, 0},
                {-8,        -3.5f,  1f, 0},
                {-3.5f,     -1.5f,  1f, 0},
                {1.5f,      -2.5f,  1f, 0},
                {-1,        0.5f,   1f, 0},
                {0.5f,      4,      1f, 0},
                {-4.5f,     2,      1f, 0}};
        }

        public override void pouring(int texture)
        {
            base.pouring(texture);
            
            for (int i = 1; i < m.Row - 1; i += 2)
            {
                GL.Begin(BeginMode.Polygon);
                GL.Normal3(0.0, 0.0, 1.0);
                GL.TexCoord2(0, 0);
                GL.Vertex3(m.matrix[i, 0], m.matrix[i, 1], m.matrix[i, 2]);
                GL.TexCoord2(1, 0);
                GL.Vertex3(m.matrix[i + 1, 0], m.matrix[i + 1, 1], m.matrix[i + 1, 2]);
                GL.TexCoord2(1, 1);
                GL.Vertex3(m.matrix[i + 2, 0], m.matrix[i + 2, 1], m.matrix[i + 2, 2]);
                GL.End();
            }
            GL.Begin(BeginMode.Polygon);
            GL.Normal3(0.0, 0.0, 1.0);
            GL.TexCoord2(0, 0);
            GL.Vertex3(m.matrix[0, 0], m.matrix[0, 1], m.matrix[0, 2]);
            GL.TexCoord2(1, 0);
            GL.Vertex3(m.matrix[1, 0], m.matrix[1, 1], m.matrix[1, 2]);
            GL.TexCoord2(1, 1);
            GL.Vertex3(m.matrix[m.Row - 1, 0], m.matrix[m.Row - 1, 1], m.matrix[m.Row - 1, 2]);
            GL.End();

            GL.Begin(BeginMode.Polygon);
            GL.Normal3(0.0, 0.0, 1.0);
            GL.TexCoord2(1, 0);
            GL.Vertex3(m.matrix[1, 0], m.matrix[1, 1], m.matrix[1, 2]);
            GL.TexCoord2(1, 1);
            GL.Vertex3(m.matrix[3, 0], m.matrix[3, 1], m.matrix[3, 2]);
            GL.TexCoord2(0, 1);
            GL.Vertex3(m.matrix[5, 0], m.matrix[5, 1], m.matrix[5, 2]);
            GL.TexCoord2(0, 0);
            GL.Vertex3(m.matrix[7, 0], m.matrix[7, 1], m.matrix[7, 2]);
            GL.End();



            for (int i = 1; i < m1.Row - 1; i += 2)
            {
                GL.Begin(BeginMode.Polygon);
                GL.Normal3(0.0, 0.0, -1.0);
                GL.TexCoord2(0, 0);
                GL.Vertex3(m1.matrix[i, 0], m1.matrix[i, 1], m1.matrix[i, 2]);
                GL.TexCoord2(1, 0);
                GL.Vertex3(m1.matrix[i + 1, 0], m1.matrix[i + 1, 1], m1.matrix[i + 1, 2]);
                GL.TexCoord2(1, 1);
                GL.Vertex3(m1.matrix[i + 2, 0], m1.matrix[i + 2, 1], m1.matrix[i + 2, 2]);
                GL.End();
            }
            GL.Begin(BeginMode.Polygon);
            GL.Normal3(0.0, 0.0, -1.0);
            GL.TexCoord2(0, 0);
            GL.Vertex3(m1.matrix[0, 0], m1.matrix[0, 1], m1.matrix[0, 2]);
            GL.TexCoord2(1, 0);
            GL.Vertex3(m1.matrix[1, 0], m1.matrix[1, 1], m1.matrix[1, 2]);
            GL.TexCoord2(1, 1);
            GL.Vertex3(m1.matrix[m1.Row - 1, 0], m1.matrix[m1.Row - 1, 1], m1.matrix[m1.Row - 1, 2]);
            GL.End();

            GL.Begin(BeginMode.Polygon);
            GL.Normal3(0.0, 0.0, -1.0);
            GL.TexCoord2(0, 0);
            GL.Vertex3(m1.matrix[1, 0], m1.matrix[1, 1], m1.matrix[1, 2]);
            GL.TexCoord2(1, 0);
            GL.Vertex3(m1.matrix[3, 0], m1.matrix[3, 1], m1.matrix[3, 2]);
            GL.TexCoord2(1, 1);
            GL.Vertex3(m1.matrix[5, 0], m1.matrix[5, 1], m1.matrix[5, 2]);
            GL.TexCoord2(0, 1);
            GL.Vertex3(m1.matrix[7, 0], m1.matrix[7, 1], m1.matrix[7, 2]);
            GL.End();
        }
    }

    class GuitarSaddle: GuitarParts
    {
        public GuitarSaddle()
        {
            color = Color.DarkGray;
            linesWidth = 0.75f;
            m = new Matrix(4, 4);
            m1 = new Matrix(4, 4);
            m.matrix = new float[4, 4]
            {
                {-6.0f, 1.5f, -0.2f , 0},
                {-6.5f, 1.5f, -0.2f , 0},
                {-6.5f, -0.5f, -0.2f , 0},
                {-6.0f, -0.5f, -0.2f , 0}
                
            };
            m1.matrix = new float[4, 4]
            {
                {-6.0f, 1.5f, 0f , 0},
                {-6.5f, 1.5f, 0f , 0},
                {-6.5f, -0.5f, 0f , 0},
                {-6.0f, -0.5f, 0f , 0}
            };
        }
        public override void pouring(int texture)
        {
            base.pouring(texture);
            GL.Begin(BeginMode.Polygon);
            GL.Normal3(0.0, 0.0, 1.0);
            for (int i = 0; i < m.Row; i++)
                GL.Vertex3(m.matrix[i, 0], m.matrix[i, 1], m.matrix[i, 2]);
            GL.End();

            GL.Begin(BeginMode.Polygon);
            GL.Normal3(0.0, 0.0, -1.0);
            for (int i = 0; i < m1.Row; i++)
                GL.Vertex3(m1.matrix[i, 0], m1.matrix[i, 1], m1.matrix[i, 2]);
            GL.End();
        }
    }

    class GuitarNeck: GuitarParts
    {
        public GuitarNeck()
        {
            color = Color.RosyBrown;
            linesWidth = 0.75f;
            m = new Matrix(7, 4);
            m.matrix = new float[7, 4] {
                {-5.0f,     1.0f,   -0.1f, 0},
                {8.0f,      1.0f,   -0.1f, 0},
                {8.5f,      1.3f,   -0.1f, 0},
                {11.0f,     0.0f,   -0.1f, 0},
                {11.1f,     -1.0f,  -0.1f, 0},
                {8.0f,      0.0f,   -0.1f, 0},
                {-4.6f,     0.0f,   -0.1f, 0}};

            m1 = new Matrix(7, 4);
            m1.matrix = new float[7, 4] {
                {-5.0f,     1.0f,   0, 0},
                {8.0f,      1.0f,   0, 0},
                {8.5f,      1.3f,   0, 0},
                {11.0f,     0.0f,   0, 0},
                {11.1f,     -1.0f,  0, 0},
                {8.0f,      0.0f,   0, 0},
                {-4.6f,     0.0f,   0, 0}};
        }

        public override void pouring(int texture)
        {
            base.pouring(texture);
            GL.Begin(BeginMode.Polygon);
            GL.Normal3(0.0, 0.0, 1.0);
            GL.TexCoord2(0, 0);
            GL.Vertex3(m.matrix[0, 0], m.matrix[0, 1], m.matrix[0, 2]);
            GL.TexCoord2(1, 0);
            GL.Vertex3(m.matrix[1, 0], m.matrix[1, 1], m.matrix[1, 2]);
            GL.TexCoord2(1, 1);
            GL.Vertex3(m.matrix[5, 0], m.matrix[5, 1], m.matrix[5, 2]);
            GL.TexCoord2(0, 1);
            GL.Vertex3(m.matrix[6, 0], m.matrix[6, 1], m.matrix[6, 2]);
            GL.End();

            GL.Begin(BeginMode.Polygon);
            GL.Normal3(0.0, 0.0, 1.0);
            GL.TexCoord2(0, 0);
            GL.Vertex3(m.matrix[1, 0], m.matrix[1, 1], m.matrix[1, 2]);
            GL.TexCoord2(1, 0);
            GL.Vertex3(m.matrix[2, 0], m.matrix[2, 1], m.matrix[2, 2]);
            GL.TexCoord2(1, 1);
            GL.Vertex3(m.matrix[5, 0], m.matrix[5, 1], m.matrix[5, 2]);
            GL.End();

            GL.Begin(BeginMode.Polygon);
            GL.Normal3(0.0, 0.0, 1.0);
            for (int i = 2; i < 6; i++)
                GL.Vertex3(m.matrix[i, 0], m.matrix[i, 1], m.matrix[i, 2]);
            GL.End();

            GL.Begin(BeginMode.Polygon);
            GL.Normal3(0.0, 0.0, -1.0);
            GL.TexCoord2(0, 0);
            GL.Vertex3(m1.matrix[0, 0], m1.matrix[0, 1], m1.matrix[0, 2]);
            GL.TexCoord2(1, 0);
            GL.Vertex3(m1.matrix[1, 0], m1.matrix[1, 1], m1.matrix[1, 2]);
            GL.TexCoord2(1, 1);
            GL.Vertex3(m1.matrix[5, 0], m1.matrix[5, 1], m1.matrix[5, 2]);
            GL.TexCoord2(0, 1);
            GL.Vertex3(m1.matrix[6, 0], m1.matrix[6, 1], m1.matrix[6, 2]);
            GL.End();

            GL.Begin(BeginMode.Polygon);
            GL.Normal3(0.0, 0.0, -1.0);
            GL.TexCoord2(0, 0);
            GL.Vertex3(m1.matrix[1, 0], m1.matrix[1, 1], m1.matrix[1, 2]);
            GL.TexCoord2(1, 0);
            GL.Vertex3(m1.matrix[2, 0], m1.matrix[2, 1], m1.matrix[2, 2]);
            GL.TexCoord2(1, 1);
            GL.Vertex3(m1.matrix[5, 0], m1.matrix[5, 1], m1.matrix[5, 2]);
            GL.End();

            GL.Begin(BeginMode.Polygon);
            GL.Normal3(0.0, 0.0, -1.0);
            for (int i = 2; i < 6; i++)
                GL.Vertex3(m1.matrix[i, 0], m1.matrix[i, 1], m1.matrix[i, 2]);
            GL.End();
        }
    }

    class GuitarStrings: GuitarPart
    {
        public GuitarStrings()
        {
            color = Color.Black;
            linesWidth = 0.25f;
            m = new Matrix(14, 4);
            m.matrix = new float[14, 4] {
                {-6.3f,     0.8f,   -0.2f, 0},
                {8.8f,      0.8f,   -0.2f, 0},
                {-6.3f,     0.7f,   -0.2f, 0},
                {9.0f,      0.7f,   -0.2f, 0},
                {-6.3f,     0.6f,   -0.2f, 0},
                {9.2f,      0.6f,   -0.2f, 0},
                {-6.3f,     0.5f,   -0.2f, 0},
                {9.4f,      0.5f,   -0.2f, 0},
                {-6.3f,     0.4f,   -0.2f, 0},
                {9.6f,      0.4f,   -0.2f, 0},
                {-6.3f,     0.3f,   -0.2f, 0},
                {9.8f,      0.3f,   -0.2f, 0},
                {-6.3f,     0.2f,   -0.2f, 0},
                {10.0f,     0.2f,   -0.2f, 0}
            };
        }

        public override void DrawLines()
        {
            base.DrawLines();
            GL.Color3(color);
            GL.LineWidth(linesWidth);
            GL.Begin(BeginMode.Lines);
            for (int f = 0; f < m.Row; f++)
                GL.Vertex3(m.matrix[f, 0], m.matrix[f, 1], m.matrix[f, 2]);
            GL.End();
        }
    }

    /// <summary>
    /// Класс, содержащий поля и методы для отрисовки и преобразования гитары
    /// </summary>
    class Guitar
    {
        public GuitarBody body;
        public GuitarNeck neck;
        public GuitarStrings strings;
        public GuitarSaddle saddle;

        public Guitar()
        {
            body = new GuitarBody();
            neck = new GuitarNeck();
            strings = new GuitarStrings();
            saddle = new GuitarSaddle();
        }

        public void DrawLines()
        {
            body.DrawLines();
            neck.DrawLines();
            strings.DrawLines();
            saddle.DrawLines();
        }
        public void tune(int ugolX, int ugolY, float len)
        {
            body.tune(ugolX, ugolY, len);
            neck.tune(ugolX, ugolY, len);
            strings.tune(ugolX, ugolY, len);
            saddle.tune(ugolX, ugolY, len);
        }
        public void scale(float mx, float my, float mz)
        {
            body.scale(mx, my, mz);
            neck.scale(mx, my, mz);
            strings.scale(mx, my, mz);
            saddle.scale(mx, my, mz);
        }
        public void pouring(int texture)
        {
            body.pouring(texture);
            neck.pouring(texture);
            saddle.pouring(texture);
        }

        public void toDimetrix()
        {
            float Grad = (float)(2 * Math.PI) / 360;
            Matrix transform = new Matrix(4,4);
            float psi = 53.7f * Grad;
            float fi = 7f * Grad;
            transform.matrix = new float[4, 4]
            {
                {(float)Math.Cos(psi), (float)(Math.Sin(fi)*Math.Cos(psi)),     0, 0 },
                {0,                    (float)Math.Cos(psi),                    0, 0 },
                {(float)Math.Sin(psi), -(float)(Math.Sin(psi)*Math.Cos(psi)),   0, 0 },
                {0,                    0,                                       0, 1 }
            };
            //body.m = body.m.Multiple(transform);
            //body.m1 = body.m1.Multiple(transform);


        }

    }

}
