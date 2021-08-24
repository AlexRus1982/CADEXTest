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

        private float coneHeight        = 0.1f;
        private float coneRadius        = 0.1f;
        private int numberOfSegments    = 8;
        private float rotX              = 1.0f;
        private float rotY              = 2.0f;
        private float rotZ              = 3.0f;
        private Vector3 coneHip;
        private List<Vector3> points    = new List<Vector3>();

        public Form1() {
            InitializeComponent();
            generateCone(coneHeight, coneRadius, numberOfSegments);
        }

        private void generateCone(float heigt, float radius, int segments) {
            coneHip = new Vector3(0.0f, -Height * 0.5f, 0.0f);
            points.Clear();
            for (int i = 0; i < segments; i++) {
                float y = Height * 0.5f;
                float x = (float)(coneRadius * Math.Cos(2 * Math.PI * i / numberOfSegments));
                float z = (float)(coneRadius * Math.Sin(2 * Math.PI * i / numberOfSegments));
                points.Add(new Vector3(x, y, z));
            }
        }

        private void openGLControl1_OpenGLDraw(object sender, SharpGL.RenderEventArgs args) {
            //  Get the OpenGL instance that's been passed to us.
            OpenGL gl = openGLControl1.OpenGL;

            gl.ClearColor(0.5f, 0.5f, 0.5f, 1.0f);
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);


            gl.MatrixMode(OpenGL.GL_PROJECTION);

            //  Load the identity.
            gl.LoadIdentity();
            gl.Translate(0, 0, -0.6f);

            //  Create a perspective transformation.
            //gl.Perspective(60.0f, (double)Width / (double)Height, 0.01, 100.0);

            //  Use the 'look at' helper function to position and aim the camera.
            //gl.LookAt(0, 0, -50, 0, 0, 0, 0, 1, 0);

            //  Set the modelview matrix.
            gl.MatrixMode(OpenGL.GL_MODELVIEW);

            //gl.Translate(0, 0, -5.0f);


            //  Rotate around the XYZ axis.
            //gl.Rotate(rotX, 1.0f, 0.0f, 0.0f);
            //gl.Rotate(rotY, 0.0f, 1.0f, 0.0f);
            //gl.Rotate(rotZ, 0.0f, 0.0f, 1.0f);

            //  Draw a coloured pyramid.
            gl.Begin(OpenGL.GL_LINES);

            gl.Color(1.0f, 1.0f, 1.0f);

            //gl.Vertex(0.0f, -0.5f, 0.0f);
            //gl.Vertex(0.5f, 0.5f, 0.0f);

            //gl.Vertex(0.5f, 0.5f, 0.0f);
            //gl.Vertex(-0.5f, 0.5f, 0.0f);

            //gl.Vertex(-0.5f, 0.5f, 0.0f);
            //gl.Vertex(0.0f, -0.5f, 0.0f);

            for (int i = 0; i < numberOfSegments - 1; i++) {
                Vector3 p1 = points[i];
                Vector3 p2 = points[i + 1];
                gl.Vertex(coneHip.X, coneHip.Y, coneHip.Z);
                gl.Vertex(p1.X, p1.Y, p1.Z);

                gl.Vertex(p1.X, p1.Y, p1.Z);
                gl.Vertex(p2.X, p2.Y, p2.Z);

                gl.Vertex(p2.X, p2.Y, p2.Z);
                gl.Vertex(coneHip.X, coneHip.Y, coneHip.Z);
            }


            // a few more calls like this omitted for clarity!
            gl.End();
        }
    }
}
