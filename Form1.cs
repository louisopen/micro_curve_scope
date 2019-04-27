//#####################################################################################
//★★★★★★★           http://www.cnpopsoft.com [华普软件]           ★★★★★★★
//★★★★★★★        华普软件 - VB6 & C#.NET专业论文与源码荟萃        ★★★★★★★
//#####################################################################################

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.Linq;
using System.Drawing.Drawing2D;

using System.IO.Ports;
using System.Threading;

namespace RotateTransformDemo
{
    public partial class Form1 : Form
    {
        //Timer timer = new Timer();
        private BackgroundWorker worker = null;
        Byte[] data = new Byte[8192];
        Int32 bytess, time_out=10, count=0;
        //bool SendFg = false, second_command = false;
        //SerialPort portCom = new SerialPort("COM1", 115200, Parity.None, 8, StopBits.One);
        SerialPort portCom = new SerialPort("COM1", 19200, Parity.None, 8, StopBits.One);


        float angle = 0f;
        private Graphics objGraphics; //Graphics 类提供将对象绘制到显示设备的方法
        private Bitmap objBitmap; //位图对象,來源
        private float fltWidth = 855; //图像宽度
        private float fltHeight = 400; //图像高度
        private float fltXSlice = 30; //X轴刻度宽度
        private float fltYSlice = 30; //Y轴刻度宽度
        //private float fltWidth = 480; //图像宽度
        //private float fltHeight = 248; //图像高度
        //private float fltXSlice = 50; //X轴刻度宽度
        //private float fltYSlice = 50; //Y轴刻度宽度    
        private float fltYSliceValue = 700; //Y轴刻度的数值宽度
        private float fltYSliceBegin = 0; //Y轴刻度开始值
        //private float fltYSliceValue2 = 10; //Y第二轴刻度的数值宽度
        //private float fltYSliceValue = 20; //Y轴刻度的数值宽度
        //private float fltYSliceBegin = 0; //Y轴刻度开始值
        //private string[] strTitle = new string[] { "Louis Try The Curve", "二月","20171203"}; //标题
        private float fltTension = 0.5f;
        private string strTitle = "解析度分析"; //标题
        private string strXAxisText = "時間"; //X轴说明文字
        private string strYAxisText = "數值"; //Y轴说明文字
        //private string strYAxisText2 = "速度"; //Y第二轴说明文字
        //private string[] strsKeys = new string[] { "一月", "二月", "三月", "四月", "五月", "六月", "七月", "八月", "九月", "十月", "十一月", "十二月" }; //键
        private string[] strsKeys = new string[] { "0時", "1時", "2時", "3時", "4時", "5時", "6時", "7時", "8時", "9時", "10時", "11時", "12時", "13時", "14時", "15時", "16時", "17時", "18時", "19時", "20時", "21時", "22時", "23時" }; //键(單位)
        //private float[] fltsValues = new float[] { 20.0f, 30.0f, 50.0f, 55.4f, 21.6f, 12.8f, 99.5f, 36.4f, 78.2f, 56.4f, 45.8f, 66.5f, 99.5f, 36.4f, 78.2f, 56.4f, 45.8f, 66.5f, 20.0f, 30.0f, 50.0f, 55.4f, 21.6f, 12.8f }; //值
        private float[] fltsValues = new float[96]; //值
        private Color clrBgColor = Color.Snow; //背景色
        private Color clrTextColor = Color.Black; //文字颜色
        private Color clrBorderColor = Color.Black; //整体边框颜色
        private Color clrAxisColor = Color.Black; //轴线颜色
        private Color clrAxisTextColor = Color.Black; //轴说明文字颜色
        private Color clrSliceTextColor = Color.Black; //刻度文字颜色
        private Color clrSliceColor = Color.Black; //刻度颜色
        private Color[] clrsCurveColors = new Color[] { Color.Red, Color.Blue, Color.Purple, Color.Green }; //曲线颜色
        //private Color[] clrsCurveColors = new Color[] { Color.Red, Color.Blue }; //曲线颜色
        private float fltXSpace = 50f; //图像左右距离边缘距离
        private float fltYSpace = 40f; //图像上下距离边缘距离
        private int intFontSize = 9; //字体大小号数
        private float fltXRotateAngle = 30f; //X轴文字旋转角度
        private float fltYRotateAngle = 0f; //Y轴文字旋转角度
        private int intCurveSize = 2; //曲线线条大小
        private int intFontSpace = 0; //intFontSpace 是字体大小和距离调整出来的一个比较适合的数字

        public Form1()
        {
            InitializeComponent();

            CreateThread();

            timer1.Interval = 10;
            timer1.Enabled = true;
            //objBitmap = new Bitmap(this.pictureBox2.Image);
            //Bitmap image = new Bitmap(@"C:\Documents and Settings\Administrator\桌面\Bmp.bmp"); //读取桌面的图片并在程序里创建它
            //objBitmap = new Bitmap(this.pictureBox1.Image);   //在pictureBox1的控件里绘制Bmp
            InitializeGraph();

            Fit();    //調整比例填滿?
            CreateImage();

            timer1.Tick += new EventHandler(timer1_Tick);
            //pictureBox1.Paint += new PaintEventHandler(pictureBox1_Paint);    

            button1.BackColor = Color.Gray;
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            //angle += 5f;
            pictureBox1.Invalidate();    
            
            if (time_out != 0)
            {
                if (--time_out == 0)
                {
                }
            }
        }


        void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            //Graphics objGraphics = e.Graphics;
            /*
            float width = pictureBox1.ClientSize.Width;     //取得邊框寬
            float height = pictureBox1.ClientSize.Height;   //取得邊框高
            objGraphics.TranslateTransform(width / 2f, height / 2f);  //重新定位中心點
            objGraphics.RotateTransform(angle);   //選轉一個角度&由重新定位原點(0,0)
            objGraphics.DrawImage(image, -image.Width / 2f, -image.Height / 2f, image.Width, image.Height);
            */

            // Create solid brush.
            //SolidBrush blueBrush = new SolidBrush(Color.Blue);
            //Brush blueBrush = new SolidBrush(Color.Blue);
            //Pen blueBrush = new Pen(Color.Blue);
            // Create array of rectangles.
            //Rectangle[] rects = { new Rectangle(0, 0, 100, 200), new Rectangle(100, 200, 250, 50), new Rectangle(300, 0, 50, 100) };
            // Fill rectangles to screen.
            //e.Graphics.FillRectangles(blueBrush, rects);

        }
       
        /*     
        public DrawImage(string[] title, string[] strsKeys, float[] fltsValues, float[] fltsValues2)
        {
              strTitle = title;
              this.strsKeys = strsKeys;
              this.fltsValues = fltsValues;
              this.fltsValues2 = fltsValues2;
        }
        */ 

        /*public Bitmap Draw()
        {
        gph.DrawString(title, new Font("宋体", 14), Brushes.Black, new PointF(cpt.X + 60, cpt.X));//图表标题
        //画x轴
        gph.DrawLine(pen, cpt.X, cpt.Y, cpt.Y, cpt.Y);
        //x轴三角形
        gph.DrawPolygon(pen, xpt);
        gph.FillPolygon(new SolidBrush(Color.Black), xpt);
        gph.DrawString(strXAxisText, new Font("宋体", 12), Brushes.Black, new PointF(cpt.Y + 10, cpt.Y + 10));
        //画y轴
        gph.DrawLine(pen, cpt.X, cpt.Y, cpt.X, cpt.X);
        //y轴三角形
        gph.DrawPolygon(pen, ypt);
        gph.FillPolygon(new SolidBrush(Color.Black), ypt);
        gph.DrawString(strYAxisText, new Font("宋体", 12), Brushes.Black, new PointF(0, 7));
        for (int i = 1; i < 11; i++)
        {
        //画y轴刻度
        gph.DrawString((400*i).ToString(), new Font("宋体", 11), Brushes.Black, new PointF(cpt.X - 30, cpt.Y - i * 30 - 6));
        gph.DrawLine(pen, cpt.X - 3, cpt.Y - i * 30, cpt.X, cpt.Y - i * 30);
        }
        for(int i=0;i<strsKeys.Length;i++)
        {
        //画x轴项目
        gph.DrawString(strsKeys[i], new Font("宋体", 11), Brushes.Black, new PointF(cpt.X + i * 30 - 5, cpt.Y + 5));
        }
        for (int i = 1; i <= fltsValues1.Length; i++)
        {
        //画点
        gph.DrawEllipse(pen, cpt.X + i * 30 - 1.5f, cpt.Y - fltsValues1[i - 1] * 3 - 1.5f, 3, 3);
        gph.FillEllipse(new SolidBrush(Color.Black), cpt.X + i * 30 - 1.5f, cpt.Y - fltsValues1[i - 1] * 3 - 1.5f, 3, 3);
        //画数值
        gph.DrawString(fltsValues1[i - 1].ToString(), new Font("宋体", 11), Brushes.Black, new PointF(cpt.X + i * 30, cpt.Y - fltsValues1[i - 1] * 3));
        //画折线
        if (i > 1)
        gph.DrawLine(Pens.Red, cpt.X + (i - 1) * 30, cpt.Y - fltsValues1[i - 2] * 3, cpt.X + i * 30, cpt.Y - fltsValues1[i - 1] * 3);
        }
        for (int i = 1; i <= fltsValues2.Length; i++)
        {
        //画点
        gph.DrawEllipse(pen, cpt.X + i * 30 - 1.5f, cpt.Y - fltsValues2[i - 1] * 3 - 1.5f, 3, 3);
        gph.FillEllipse(new SolidBrush(Color.Black), cpt.X + i * 30 - 1.5f, cpt.Y - fltsValues2[i - 1] * 3 - 1.5f, 3, 3);
        //画数值
        gph.DrawString(fltsValues2[i - 1].ToString(), new Font("宋体", 11), Brushes.Black, new PointF(cpt.X + i * 30, cpt.Y - fltsValues2[i - 1] * 3));
        //画折线
        if (i > 1)
        gph.DrawLine(Pens.Red, cpt.X + (i - 1) * 30, cpt.Y - fltsValues2[i - 2] * 3, cpt.X + i * 30, cpt.Y - fltsValues2[i - 1] * 3);
        }
        //保存输出图片
        //bmap.Save(Response.OutputStream, ImageFormat.Gif);
        return bmap;
        }*/
        #region 公共属性
        /// <summary>
        /// 图像的宽度
        /// </summary>
        public float Width
        {
            set
            {
                if (value < 100)
                {
                    fltWidth = 100;
                }
                else
                {
                    fltWidth = value;
                }
            }
            get
            {
                if (fltWidth <= 100)
                {
                    return 100;
                }
                else
                {
                    return fltWidth;
                }
            }
        }
        /// <summary>
        /// 图像的高度
        /// </summary>
        public float Height
        {
            set
            {
                if (value < 100)
                {
                    fltHeight = 100;
                }
                else
                {
                    fltHeight = value;
                }
            }
            get
            {
                if (fltHeight <= 100)
                {
                    return 100;
                }
                else
                {
                    return fltHeight;
                }
            }
        }
        /// <summary>
        /// X轴刻度宽度
        /// </summary>
        public float XSlice
        {
            set { fltXSlice = value; }
            get { return fltXSlice; }
        }
        /// <summary>
        /// Y轴刻度宽度
        /// </summary>
        public float YSlice
        {
            set { fltYSlice = value; }
            get { return fltYSlice; }
        }
        /// <summary>
        /// Y轴刻度的数值宽度
        /// </summary>
        public float YSliceValue
        {
            set { fltYSliceValue = value; }
            get { return fltYSliceValue; }
        }
        /// <summary>
        /// Y轴刻度开始值
        /// </summary>
        public float YSliceBegin
        {
            set { fltYSliceBegin = value; }
            get { return fltYSliceBegin; }
        }
        /// <summary>
        /// 张力系数
        /// </summary>
        public float Tension
        {
            set
            {
                if (value < 0.0f && value > 1.0f)
                {
                    fltTension = 0.5f;
                }
                else
                {
                    fltTension = value;
                }
            }
            get
            {
                return fltTension;
            }
        }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title
        {
            set { strTitle = value; }
            get { return strTitle; }
        }
        /// <summary>
        /// 键，X轴数据
        /// </summary>
        public string[] Keys
        {
            set { strsKeys = value; }
            get { return strsKeys; }
        }
        /// <summary>
        /// 值，Y轴数据
        /// </summary>
        public float[] Values
        {
            set { fltsValues = value; }
            get { return fltsValues; }
        }
        /// <summary>
        /// 背景色
        /// </summary>
        public Color BgColor
        {
            set { clrBgColor = value; }
            get { return clrBgColor; }
        }
        /// <summary>
        /// 文字颜色
        /// </summary>
        public Color TextColor
        {
            set { clrTextColor = value; }
            get { return clrTextColor; }
        }
        /// <summary>
        /// 整体边框颜色
        /// </summary>
        public Color BorderColor
        {
            set { clrBorderColor = value; }
            get { return clrBorderColor; }
        }
        /// <summary>
        /// 轴线颜色
        /// </summary>
        public Color AxisColor
        {
            set { clrAxisColor = value; }
            get { return clrAxisColor; }
        }
        /// <summary>
        /// X轴说明文字
        /// </summary>
        public string XAxisText
        {
            set { strXAxisText = value; }
            get { return strXAxisText; }
        }
        /// <summary>
        /// Y轴说明文字
        /// </summary>
        public string YAxisText
        {
            set { strYAxisText = value; }
            get { return strYAxisText; }
        }
        /// <summary>
        /// 轴说明文字颜色
        /// </summary>
        public Color AxisTextColor
        {
            set { clrAxisTextColor = value; }
            get { return clrAxisTextColor; }
        }
        /// <summary>
        /// 刻度文字颜色
        /// </summary>
        public Color SliceTextColor
        {
            set { clrSliceTextColor = value; }
            get { return clrSliceTextColor; }
        }
        /// <summary>
        /// 刻度颜色
        /// </summary>
        public Color SliceColor
        {
            set { clrSliceColor = value; }
            get { return clrSliceColor; }
        }
        /// <summary>
        /// 曲线颜色
        /// </summary>
        public Color[] CurveColors
        {
            set { clrsCurveColors = value; }
            get { return clrsCurveColors; }
        }
        /// <summary>
        /// X轴文字旋转角度
        /// </summary>
        public float XRotateAngle
        {
            get { return fltXRotateAngle; }
            set { fltXRotateAngle = value; }
        }
        /// <summary>
        /// Y轴文字旋转角度
        /// </summary>
        public float YRotateAngle
        {
            get { return fltYRotateAngle; }
            set { fltYRotateAngle = value; }
        }
        /// <summary>
        /// 图像左右距离边缘距离
        /// </summary>
        public float XSpace
        {
            get { return fltXSpace; }
            set { fltXSpace = value; }
        }
        /// <summary>
        /// 图像上下距离边缘距离
        /// </summary>
        public float YSpace
        {
            get { return fltYSpace; }
            set { fltYSpace = value; }
        }
        /// <summary>
        /// 字体大小号数
        /// </summary>
        public int FontSize
        {
            get { return intFontSize; }
            set { intFontSize = value; }
        }
        /// <summary>
        /// 曲线线条大小
        /// </summary>
        public int CurveSize
        {
            get { return intCurveSize; }
            set { intCurveSize = value; }
        }
        #endregion

        /// <summary>
        /// 初始化和填充图像区域，画出边框，初始标题
        /// </summary>
        private void InitializeGraph()
        {
            //根据给定的高度和宽度创建一个位图图像
            //objBitmap = new Bitmap((int)Width, (int)Height);    
            objBitmap = new Bitmap(pictureBox1.ClientSize.Width, pictureBox1.ClientSize.Height);
            //从指定的 objBitmap 对象创建 objGraphics 对象 (即在objBitmap对象中画图)

            objGraphics = Graphics.FromImage(objBitmap);

            pictureBox1.BackgroundImage = objBitmap; // 設置為背景層

            //pictureBox1.CreateGraphics().DrawImage(objBitmap, 0, 0);

            //根据给定颜色(LightGray)填充图像的矩形区域 (背景)
            objGraphics.DrawRectangle(new Pen(BorderColor, 1), 0, 0, Width - 1, Height - 1); //画边框
            objGraphics.FillRectangle(new SolidBrush(BgColor), 1, 1, Width - 2, Height - 2); //填充边框
                                                                                             //画X轴,注意图像的原始X轴和Y轴计算是以左上角为原点，向右和向下计算的
            float fltX1 = XSpace;
            float fltY1 = Height - YSpace;
            float fltX2 = Width - XSpace + XSlice / 2;
            float fltY2 = fltY1;
            objGraphics.DrawLine(new Pen(new SolidBrush(AxisColor), 1), fltX1, fltY1, fltX2, fltY2);
            //画Y轴
            fltX1 = XSpace;
            fltY1 = Height - YSpace;
            fltX2 = XSpace;
            fltY2 = YSpace - YSlice / 2;
            objGraphics.DrawLine(new Pen(new SolidBrush(AxisColor), 1), fltX1, fltY1, fltX2, fltY2);
            //初始化轴线说明文字
            SetAxisText(ref objGraphics);
            //初始化X轴上的刻度和文字
            SetXAxis(ref objGraphics);
            //初始化Y轴上的刻度和文字
            SetYAxis(ref objGraphics);
            //初始化标题
            CreateTitle(ref objGraphics);
        }
        /// <summary>
        /// 初始化轴线说明文字
        /// </summary>
        /// <param name="objGraphics"></param>
        private void SetAxisText(ref Graphics objGraphics)
        {
            float fltX = Width - XSpace + XSlice / 2 - (XAxisText.Length - 1) * intFontSpace;
            float fltY = Height - YSpace - intFontSpace;
            objGraphics.DrawString(XAxisText, new Font("宋体", FontSize), new SolidBrush(AxisTextColor), fltX, fltY);
            fltX = XSpace + 5;
            fltY = YSpace - YSlice / 2 - intFontSpace;
            /*
            for (int i = 0; i < YAxisText.Length; i++)
            {
                objGraphics.DrawString(YAxisText[i].ToString(), new Font("宋体", FontSize), new SolidBrush(AxisTextColor), fltX, fltY);
                fltY += intFontSpace; //字体上下距离
            }
            */
            objGraphics.DrawString(YAxisText.ToString(), new Font("宋体", FontSize), new SolidBrush(AxisTextColor), fltX, fltY);
        }
        /// <summary>
        /// 初始化X轴上的刻度和文字
        /// </summary>
        /// <param name="objGraphics"></param>
        private void SetXAxis(ref Graphics objGraphics)
        {
            float fltX1 = XSpace;
            float fltY1 = Height - YSpace;
            float fltX2 = XSpace;
            float fltY2 = Height - YSpace;
            int iCount = 0;
            int iSliceCount = 1;
            float Scale = 0;
            float iWidth = ((Width - 2 * XSpace) / XSlice) * 50; //将要画刻度的长度分段，并乘以50，以10为单位画刻度线。
            float fltSliceHeight = XSlice / 10; //刻度线的高度
            objGraphics.TranslateTransform(fltX1, fltY1); //平移图像(原点)
            objGraphics.RotateTransform(XRotateAngle, MatrixOrder.Prepend); //旋转图像
            //objGraphics.DrawString(Keys[0].ToString(), new Font("宋体", FontSize), new SolidBrush(SliceTextColor), 0, 0);
            objGraphics.DrawString(Keys[0].ToString(), new Font("宋体", FontSize), new SolidBrush(SliceTextColor), 0, 0 + 5);
            objGraphics.ResetTransform(); //重置图像
            for (int i = 0; i <= iWidth; i += 10) //以10为单位
            {
                Scale = i * XSlice / 50;//即(i / 10) * (XSlice / 5)，将每个刻度分五部分画，但因为i以10为单位，得除以10
                if (iCount == 5)
                {
                    objGraphics.DrawLine(new Pen(new SolidBrush(AxisColor)), fltX1 + Scale, fltY1 + fltSliceHeight * 1.5f, fltX2 + Scale, fltY2 - fltSliceHeight * 1.5f);
                    //画网格虚线
                    Pen penDashed = new Pen(new SolidBrush(AxisColor));
                    penDashed.DashStyle = DashStyle.Dash;
                    objGraphics.DrawLine(penDashed, fltX1 + Scale, fltY1, fltX2 + Scale, YSpace - YSlice / 2);
                    //这里显示X轴刻度
                    if (iSliceCount <= Keys.Length - 1)
                    {
                        objGraphics.TranslateTransform(fltX1 + Scale, fltY1);
                        objGraphics.RotateTransform(XRotateAngle, MatrixOrder.Prepend);
                        //objGraphics.DrawString(Keys[iSliceCount].ToString(), new Font("宋体", FontSize), new SolidBrush(SliceTextColor), 0, 0);
                        objGraphics.DrawString(Keys[iSliceCount].ToString(), new Font("宋体", FontSize), new SolidBrush(SliceTextColor), 0, 0 + 5);
                        objGraphics.ResetTransform();
                    }
                    else
                    {
                        //超过范围，不画任何刻度文字
                    }
                    iCount = 0;
                    iSliceCount++;
                    if (fltX1 + Scale > Width - XSpace)
                    {
                        break;
                    }
                }
                else
                {
                    objGraphics.DrawLine(new Pen(new SolidBrush(SliceColor)), fltX1 + Scale, fltY1 + fltSliceHeight, fltX2 + Scale, fltY2 - fltSliceHeight);
                }
                iCount++;
            }
        }
        /// <summary>
        /// 初始化Y轴上的刻度和文字
        /// </summary>
        /// <param name="objGraphics"></param>
        private void SetYAxis(ref Graphics objGraphics)
        {
            float fltX1 = XSpace;
            float fltY1 = Height - YSpace;
            float fltX2 = XSpace;
            float fltY2 = Height - YSpace;
            int iCount = 0;
            float Scale = 0;
            int iSliceCount = 1;
            float iHeight = ((Height - 2 * YSpace) / YSlice) * 50; //将要画刻度的长度分段，并乘以50，以10为单位画刻度线。
            float fltSliceWidth = YSlice / 10; //刻度线的宽度
            string strSliceText = string.Empty;
            objGraphics.TranslateTransform(XSpace - intFontSpace * YSliceBegin.ToString().Length, Height - YSpace); //平移图像(原点)
            objGraphics.RotateTransform(YRotateAngle, MatrixOrder.Prepend); //旋转图像
            //objGraphics.DrawString(YSliceBegin.ToString(), new Font("宋体", FontSize), new SolidBrush(SliceTextColor), 0, 0);
            objGraphics.DrawString(YSliceBegin.ToString(), new Font("宋体", FontSize), new SolidBrush(SliceTextColor), 0 - 15, 0);
            objGraphics.ResetTransform(); //重置图像
            for (int i = 0; i < iHeight; i += 10)
            {
                Scale = i * YSlice / 50; //即(i / 10) * (YSlice / 5)，将每个刻度分五部分画，但因为i以10为单位，得除以10
                if (iCount == 5)
                {
                    objGraphics.DrawLine(new Pen(new SolidBrush(AxisColor)), fltX1 - fltSliceWidth * 1.5f, fltY1 - Scale, fltX2 + fltSliceWidth * 1.5f, fltY2 - Scale);
                    //画网格虚线
                    Pen penDashed = new Pen(new SolidBrush(AxisColor));
                    penDashed.DashStyle = DashStyle.Dash;
                    objGraphics.DrawLine(penDashed, XSpace, fltY1 - Scale, Width - XSpace + XSlice / 2, fltY2 - Scale);
                    //这里显示Y轴刻度
                    strSliceText = Convert.ToString(YSliceValue * iSliceCount + YSliceBegin);
                    objGraphics.TranslateTransform(XSpace - intFontSize * strSliceText.Length, fltY1 - Scale); //平移图像(原点)
                    objGraphics.RotateTransform(YRotateAngle, MatrixOrder.Prepend); //旋转图像
                    objGraphics.DrawString(strSliceText, new Font("宋体", FontSize), new SolidBrush(SliceTextColor), 0, 0);
                    objGraphics.ResetTransform(); //重置图像
                    iCount = 0;
                    iSliceCount++;
                }
                else
                {
                    objGraphics.DrawLine(new Pen(new SolidBrush(SliceColor)), fltX1 - fltSliceWidth, fltY1 - Scale, fltX2 + fltSliceWidth, fltY2 - Scale);
                }
                iCount++;
            }
        }
        /// <summary>
        /// 初始化标题
        /// </summary>
        /// <param name="objGraphics"></param>
        private void CreateTitle(ref Graphics objGraphics)
        {
            //objGraphics.DrawString(Title, new Font("宋体", FontSize), new SolidBrush(TextColor), new Point((int)(Width - XSpace) - intFontSize * Title.Length, (int)(YSpace - YSlice / 2 - intFontSpace)));
            objGraphics.DrawString(Title, new Font("宋体", FontSize), new SolidBrush(TextColor), new Point((int)(Width - XSpace) / 2, (int)(YSpace - YSlice)-intFontSpace*2));
        }
        /// <summary>
        /// 自动根据参数调整图像大小
        /// </summary>
        public void Fit()
        {
            int intx = 2;   //20171205 Louis try
            int inty = 7000; //20171205 Louis try

            //计算字体距离
            intFontSpace = FontSize + 5;
            //计算图像边距
            float fltSpace = Math.Min(Width / 6, Height / 6);
            XSpace = fltSpace;
            YSpace = fltSpace;
            //计算X轴刻度宽度
            XSlice = (Width - 2 * XSpace) / (Keys.Length - 1);
            //计算Y轴刻度宽度和Y轴刻度开始值
            float fltMinValue = 0;
            float fltMaxValue = 0;
            for (int i = 0; i < Values.Length; i++)
            {
                intx += i;   //20171205 Louis try
                Values[i] = inty / 2 * (1 - (float)Math.Sin(i * 2 * Math.PI / (intx - 1))); //玄   20171205 Louis try

                if (Values[i] < fltMinValue)
                {
                    fltMinValue = Values[i];
                }
                else if (Values[i] > fltMaxValue)
                {
                    fltMaxValue = Values[i];
                }
            }
            if (YSliceBegin > fltMinValue)
            {
                YSliceBegin = fltMinValue;
            }
            int intYSliceCount = (int)(fltMaxValue / YSliceValue);
            if (fltMaxValue % YSliceValue != 0)
            {
                intYSliceCount++;
            }
            YSlice = (Height - 2 * YSpace) / intYSliceCount;
        }

        /// <summary>
        /// 生成图像并返回bmp图像对象
        /// </summary>
        /// <returns></returns>
        public Bitmap CreateImage()
        {
            //InitializeGraph();
            int intKeysCount = Keys.Length;
            int intValuesCount = Values.Length;
            if (intValuesCount % intKeysCount == 0)
            {
                int intCurvesCount = intValuesCount / intKeysCount;
                for (int i = 0; i < intCurvesCount; i++)
                {
                    float[] fltCurrentValues = new float[intKeysCount];
                    for (int j = 0; j < intKeysCount; j++)
                    {
                        fltCurrentValues[j] = (Values[i * intKeysCount + j]);
                    }
                    DrawContent(ref objGraphics, fltCurrentValues, clrsCurveColors[i]); //重複畫線
                }
            }
            else
            {
                objGraphics.DrawString("发生错误，Values的长度必须是Keys的整数倍!", new Font("宋体", FontSize + 5), new SolidBrush(TextColor), new Point((int)XSpace, (int)(Height / 2)));
            }
            return objBitmap;
        }
        /// <summary>
        /// 画曲线
        /// </summary>
        /// <param name="objGraphics"></param>
        private void DrawContent(ref Graphics objGraphics, float[] fltCurrentValues, Color clrCurrentColor)
        {
            Pen CurvePen = new Pen(clrCurrentColor, CurveSize);
            PointF[] CurvePointF = new PointF[Keys.Length];
            float keys = 0;
            float values = 0;
            /*
            float Offset1 = (Height - 100) + YSliceBegin;
            float Offset2 = (YSlice / 50) * (50 / YSliceValue);
            for (int i = 0; i < Keys.Length; i++)
            {
                keys = XSlice * i + 100;
                values = Offset1 - Values[i] * Offset2;
                CurvePointF[i] = new PointF(keys, values);
            }
            objGraphics.DrawLines(CurvePen, CurvePointF);
            */
            for (int i = 0; i < Keys.Length; i++)
            {
                keys = XSlice * i + XSpace;
                values = (Height - YSpace) + YSliceBegin - YSlice * (fltCurrentValues[i] / YSliceValue);
                CurvePointF[i] = new PointF(keys, values);
            }    
            objGraphics.DrawCurve(CurvePen, CurvePointF, Tension);
        }

        private void CreateThread()
        {
            worker = new BackgroundWorker();
            // Specify that the background worker provides progress notifications            
            worker.WorkerReportsProgress = true;
            // Specify that the background worker supports cancellation
            worker.WorkerSupportsCancellation = true;
            // The DoWork event handler is the main work function of the background thread
            worker.DoWork += new DoWorkEventHandler(backgroundWorker1_DoWork);
            // Specify the function to use to handle progress
            worker.ProgressChanged += new ProgressChangedEventHandler(backgroundWorker1_ProgressChanged);
            //worker.ProgressChanged += new ProgressChangedEventHandler(progressForm.OnProgressChanged);
            // Specify the function to run when the background worker finishes
            // There are three conditions possible that should be handled in this function:
            // 1. The work completed successfully
            // 2. The work aborted with errors
            // 3. The user cancelled the process
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker1_RunWorkerCompleted);
            //worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(progressForm.OnProcessCompleted);

            //If your background operation requires a parameter, 
            //call System.ComponentModel.BackgroundWorker.RunWorkerAsync 
            //with your parameter. Inside the System.ComponentModel.BackgroundWorker.DoWork 
            //event handler, you can extract the parameter from the 
            //System.ComponentModel.DoWorkEventArgs.Argument property.
            worker.RunWorkerAsync();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            int len;
            //if (radioButton1.Checked)       //Seclect Serial port.
            //{
            bytess = 0;
            while (true)
            {
                try
                {
                    if (portCom.IsOpen)
                    {
                        Thread.Sleep(1);        //Sleep 5ms. to 150ms.
                                                 ////////////  Meth 2 for receiver /////////////////////////////////////
                                                 //bytess = portCom.BytesToRead;
                        len = portCom.Read(data, bytess, portCom.BytesToRead);
                        if (len == 0)
                        {
                            time_out = 10;                      
                            if (bytess != 0)
                            {
                                worker.ReportProgress(3);  //trigger ProgressChanged
                            }
                            continue;
                        }
                        else
                        {
                            bytess += len;
                        }
                    }
                    else
                    {
                        Thread.Sleep(250);        //Sleep 500ms. 200ms
                    }
                }
                catch (Exception ex)
                {
                    //if (bytess != 0)
                    bytess = 0;
                    //portCom.Close();
                    //button2.Text = "Connect";
                    //MessageBox.Show("資料收發問題:" + ex.Message);
                }
            }
        }


        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (bytess >= 7)        
            {
                if((data[0] == 0x55) && (data[1] == 0))
                {                 
                    Int16 sum = 0;
                    sum = Convert.ToInt16(data[3] * 256);
                    sum += data[2];
                    label1.Text = Convert.ToString(sum);
                    count++;
                    label3.Text = Convert.ToString(count);
                }
                bytess = 0;
            }
            else
            {

            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                //MessageBox.Show("資料發送問題");
            }
            else
            {
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (portCom.IsOpen)
            {
                if (button1.Text == "High")
                {
                    button1.Text = "Low";
                    portCom.RtsEnable = true;   //High +7.5V
                }
                else
                {
                    button1.Text = "High";
                    portCom.RtsEnable = false;   //High -7.5V
                }
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            if (button2.Text == "Connect")
            {
                count = 0;
                portCom.PortName = comboBox1.Text;
                try
                {
                    portCom.Open();
                }
                catch (Exception)
                {
                    return;
                }
                button2.Text = "Disconnect";
            }
            else
            {
                portCom.Close();
                button2.Text = "Connect";
            }


            if (portCom.RtsEnable)
            {
                button1.Text = "Low";
            }
            else
            {
                button1.Text = "High";
            }
            button1.BackColor = Color.Yellow;
        }
    }
}