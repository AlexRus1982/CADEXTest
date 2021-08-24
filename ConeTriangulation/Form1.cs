using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Numerics;
using SharpGL;

namespace ConeTriangulation {
    public partial class Form1 : Form {

        private OpenGL gl;
        private float coneHeight        = 1.5f;
        private float coneRadius        = 0.5f;
        private int numberOfSegments    = 16;
        private float rotX              = 0.0f;
        private float rotY              = 0.0f;
        private float rotZ              = 0.0f;
        private Vector3 coneHip;
        private List<Vector3> points    = new List<Vector3>();

        public Form1() {
            InitializeComponent();
            generateCone(coneHeight, coneRadius, numberOfSegments);
        }

        private void generateCone(float height, float radius, int segments) {
            coneHip = new Vector3(0.0f, -height * 0.5f, 0.0f);
            points.Clear();
            for (int i = 0; i < segments; i++) {
                float y = height * 0.5f;
                float x = (float)(coneRadius * Math.Cos(2 * Math.PI * i / numberOfSegments));
                float z = (float)(coneRadius * Math.Sin(2 * Math.PI * i / numberOfSegments));
                points.Add(new Vector3(x, y, z));
            }
        }

        private void drawSurface(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 n, float color) {
            gl.Color(color, color, color);
            gl.Begin(OpenGL.GL_TRIANGLES);
            gl.Vertex(v1.X, v1.Y, v1.Z);
            gl.Vertex(v2.X, v2.Y, v2.Z);
            gl.Vertex(v3.X, v3.Y, v3.Z);
            gl.End();

            gl.Color(0, 0, 0);
            gl.Begin(OpenGL.GL_LINES);
            gl.Vertex(v1.X, v1.Y, v1.Z);
            gl.Vertex(v2.X, v2.Y, v2.Z);

            gl.Vertex(v2.X, v2.Y, v2.Z);
            gl.Vertex(v3.X, v3.Y, v3.Z);

            gl.Vertex(v3.X, v3.Y, v3.Z);
            gl.Vertex(v1.X, v1.Y, v1.Z);
            gl.End();

            gl.Color(1.0f, 0, 0);
            gl.Begin(OpenGL.GL_LINES);

            gl.Vertex(v2.X, v2.Y, v2.Z);
            gl.Vertex(n.X, n.Y, n.Z);

            gl.End();
        }

        private void openGLControl1_OpenGLDraw(object sender, SharpGL.RenderEventArgs args) {
            //  Get the OpenGL instance that's been passed to us.
            gl = openGLControl1.OpenGL;

            gl.ClearColor(1.0f, 1.0f, 1.0f, 1.0f);
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            gl.MatrixMode(OpenGL.GL_PROJECTION);                                //  Set the projection matrix.
            gl.LoadIdentity();                                                  //  Load the identity.            
            gl.Perspective(60.0f, (double)Width / (double)Height, 0.01, 100.0); //  Create a perspective transformation.           
            gl.LookAt(0, 0, -2.0f, 0, 0, 0, 0, 1, 0);                           //  Use the 'look at' helper function to position and aim the camera.

            gl.MatrixMode(OpenGL.GL_MODELVIEW);                                 //  Set the modelview matrix.
            gl.LoadIdentity();                                                  //  Load the identity.

            //  Rotate around the XYZ axis.
            gl.Rotate(rotX, 1.0f, 0.0f, 0.0f);
            gl.Rotate(rotY, 0.0f, 1.0f, 0.0f);
            gl.Rotate(rotZ, 0.0f, 0.0f, 1.0f);

            gl.Enable(OpenGL.GL_MULTISAMPLE);

            Vector3 p1 = points[0];
            Vector3 p2 = points[0];
            Vector3 n1 = points[0];
            Vector3 o = new Vector3(0, coneHeight * 0.5f, 0);

            float c = 0.3f;

            for (int i = 0; i < numberOfSegments - 1; i++) {
                p1 = points[i];
                p2 = points[i + 1];
                n1 = p1 + Vector3.Normalize(p1 - o) * 0.1f;

                drawSurface(coneHip, p1, p2, n1, c);

                c += 0.02f;
            }

            p1 = points[numberOfSegments - 1];
            p2 = points[0];
            n1 = p1 + Vector3.Normalize(p1 - o) * 0.1f;

            drawSurface(coneHip, p1, p2, n1, c);

            rotX += 0.5f;
            rotY += 1.0f;
            rotZ += 1.5f;

            gl.Flush();
        }
    }
}
