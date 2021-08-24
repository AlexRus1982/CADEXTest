using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Windows.Forms;

namespace GraphDraw {

    public partial class Form1 : Form {

        private Graphics g;                                     // panel GDC
        private List<Curve2D> curves = new List<Curve2D>();     // curves array

        public Form1() {
            InitializeComponent();
            g = this.panel1.CreateGraphics();
            generateRandomCurves();
        }

        /// <summary>
        /// Function generates list of curves, adds them to List - curves and to comboBox1.Items
        /// </summary>
        private void generateRandomCurves() {
            comboBox1.DisplayMember = "Text";
            comboBox1.ValueMember = "Value";
            for (int i = 0; i < 100; i++) {
                Curve2D curve = Curve2D.MakeRandomCurve(panel1.Width, panel1.Height, /*135*/ (float)Math.PI * 0.25f /* PI/4 */);
                curves.Add(curve);
                comboBox1.Items.Add(new { Text = (comboBox1.Items.Count + 1) + " : " + curve.StringType(), Value = curve });
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) {
            g.Clear(BackColor);
            Curve2D curve = curves[comboBox1.SelectedIndex];
            curve.Draw(g);
            textBox1.Text = "" + curve;
        }
    }

    public class Curve2D {

        public float t = 0.0f;
        protected float dt = 1.0f;
        protected string curveType = "Empty";
        public static Random rnd = new Random();

        /// <summary>
        /// Constructor that generate random curve
        /// </summary>
        /// <param name="xMax">Max width of drawing panel</param>
        /// <param name="yMax">Max height of drawing panel</param>
        /// <param name="pt">parameter t along the curve</param>
        /// <returns>Return line or ellipse</returns>
        static public Curve2D MakeRandomCurve(float xMax, float yMax, float pt = (float)Math.PI * 0.25f){
            int i = (int)rnd.Next(0, 10);
            if (i <= 5) return new Line(xMax, yMax, pt);
            if (i > 5) return new Ellipse(xMax, yMax, pt);
            return new Curve2D();
        }

        public string StringType() {
            return curveType;
        }

        public override string ToString() {
            return curveType;
        }

        public virtual void Draw(Graphics g) { }

        public virtual Vector2 GetTPoint() {
            return new Vector2(0, 0);
        }

        public virtual Vector2 GetDerivative() {
            return new Vector2(0, 0);
        }
    }

    public class Line : Curve2D {

        private Vector2 o;
        private Vector2 d;
        private Vector2 e;
        private Vector2 tPoint;
        private float drawLen = 250.0f;

        public override Vector2 GetTPoint() {
            return o + d * t;
        }

        public override Vector2 GetDerivative() {
            return d;
        }

        /// <summary>
        /// Constructor that generate random line - curve
        /// </summary>
        /// <param name="xMax">Max width of drawing panel</param>
        /// <param name="yMax">Max height of drawing panel</param>
        /// <param name="pt">parameter t along the curve</param>
        public Line(float xMax, float yMax, float pt = (float)Math.PI * 0.25f) {
            curveType = "Line";
            o = new Vector2((float)rnd.NextDouble() * xMax, (float)rnd.NextDouble() * yMax);
            e = new Vector2((float)rnd.NextDouble() * xMax, (float)rnd.NextDouble() * yMax);
            d = Vector2.Normalize(e - o);
            t = pt;
            tPoint = GetTPoint();
        }

        /// <summary>
        /// Constructor that generate line - curve
        /// </summary>
        /// <param name="startPoint">start position of line</param>
        /// <param name="dir">dirrection vector</param>
        /// <param name="pt">parameter t along the curve</param>
        public Line(Vector2 startPoint, Vector2 dir, float pt = (float)Math.PI * 0.25f) {
            curveType = "Line";
            o = startPoint;
            d = Vector2.Normalize(dir);
            e = o + d * drawLen;
            t = pt;
            tPoint = GetTPoint();
        }

        public override string ToString() {
            string s = curveType + Environment.NewLine;
            s += "--------------------------------------" + Environment.NewLine;
            s += "start point : " + o + Environment.NewLine;
            s += "t point : " + GetTPoint() + Environment.NewLine;
            s += "derivative : " + GetDerivative() + Environment.NewLine;
            string ts = ((int)(t * 1000) != (int)(Math.PI * 250.0f)) ? "" + t : "PI/4";
            s += "t = " + ts + Environment.NewLine;
            return s;
        }

        public override void Draw(Graphics g) {
            SolidBrush br = new SolidBrush(Color.Purple);
            Pen pen = new Pen(br, 2);

            pen.Color = Color.Red;
            g.DrawLine(pen, o.X, o.Y, e.X, e.Y);

            pen.Color = Color.Blue;
            g.DrawEllipse(pen, o.X - 2.5f, o.Y - 2.5f, 5, 5);

            br.Color = Color.Black;
            g.DrawString("Start point", new Font("Arial", 12.0f), br, o.X, o.Y);

            pen.Color = Color.Green;
            g.DrawEllipse(pen, tPoint.X - 2.5f, tPoint.Y - 2.5f, 5, 5);
            g.DrawString("T point", new Font("Arial", 12.0f), br, tPoint.X, tPoint.Y);
        }
    }

    public class Ellipse : Curve2D {
        private Vector2 o;
        private float xR;
        private float yR;
        private Vector2 tPoint;
        private Vector2 dPoint;
        private Vector2 derivative;

        public override Vector2 GetTPoint() {
            return new Vector2((float)(xR * Math.Cos(Math.PI * t / 180.0f)),
                               (float)(yR * Math.Sin(Math.PI * t / 180.0f))) * 0.5f + o;
        }

        public override Vector2 GetDerivative() {
            tPoint = GetTPoint();
            dPoint = new Vector2((float)(xR * Math.Cos(Math.PI * (t + dt) / 180.0f)),
                                 (float)(yR * Math.Sin(Math.PI * (t + dt) / 180.0f))) * 0.5f + o;

            return Vector2.Normalize(dPoint - tPoint);
        }

        /// <summary>
        /// Constructor that generate random ellipse - curve
        /// </summary>
        /// <param name="xMax">Max width of drawing panel</param>
        /// <param name="yMax">Max height of drawing panel</param>
        /// <param name="pt">parameter t along the curve</param>
        public Ellipse(float xMax, float yMax, float pt = (float)Math.PI * 0.25f) {
            curveType = "Ellipse";
            o = new Vector2(xMax, yMax) * 0.5f;
            xR = (float)rnd.NextDouble() * xMax;
            yR = (float)rnd.NextDouble() * yMax;
            t = pt;
            derivative = GetDerivative();
        }

        /// <summary>
        /// Constructor that generate ellipse - curve
        /// </summary>
        /// <param name="centerPoint"> center point of ellipse</param>
        /// <param name="radiusX">radius according X axe</param>
        /// <param name="radiusY">radius according Y axe</param>
        /// <param name="pt">parameter t along the curve</param>
        public Ellipse(Vector2 centerPoint, float radiusX, float radiusY, float pt = (float)Math.PI * 0.25f) {
            curveType = "Ellipse";
            o = centerPoint;
            xR = (float)rnd.NextDouble() * radiusX;
            yR = (float)rnd.NextDouble() * radiusY;
            t = pt;
            derivative = GetDerivative();
        }

        public override string ToString() {
            string s = curveType + Environment.NewLine;
            s += "--------------------------------------" + Environment.NewLine;
            s += "center point : " + o + Environment.NewLine;
            s += "X radius : " + xR + Environment.NewLine;
            s += "Y radius : " + yR + Environment.NewLine;
            s += "t point : " + GetTPoint() + Environment.NewLine;
            s += "derivative : " + GetDerivative() + Environment.NewLine;
            string ts = ((int)(t * 1000) != (int)(Math.PI * 250.0f)) ? "" + t : "PI/4";
            s += "t = " + ts + Environment.NewLine;
            return s;
        }

        public override void Draw(Graphics g) {
            SolidBrush br = new SolidBrush(Color.Purple);
            Pen pen = new Pen(br, 2);

            pen.Color = Color.Red;
            g.DrawEllipse(pen, o.X - xR * 0.5f, o.Y - yR * 0.5f, xR, yR);

            pen.Color = Color.Blue;
            g.DrawEllipse(pen, o.X - 2.5f, o.Y - 2.5f, 5, 5);

            pen.Color = Color.Green;
            g.DrawEllipse(pen, tPoint.X - 2.5f, tPoint.Y - 2.5f, 5, 5);
            g.DrawString("T point", new Font("Arial", 12.0f), br, tPoint.X, tPoint.Y);

            pen.Color = Color.Purple;
            g.DrawEllipse(pen, dPoint.X - 2.5f, dPoint.Y - 2.5f, 5, 5);
        }
    }
}
