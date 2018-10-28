using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;




namespace Imagehw1
{
    public partial class Form1 : Form
    {
        Bitmap openImage;//開檔所需的影像
        Bitmap currentImage;//暫存用影像
        Bitmap origin_rot;
        bool Has_rotate;
        int rotate_angle;

        public Form1()
        {
            InitializeComponent();

        }

        //線性插值函數，傳入座標點，傳回內插過的灰階值
        int bilinear_interpolation(float x, float y) {
            int x0, x1, y0, y1;
            if (x - (int)x == 0.0f)//剛好在整數點上
            {
                x0 = (int)x; x1 = (int)x; 
            }
            else {
                x0 = (int)x; x1 = (int)(x + 1);
            }

            if (y - (int)y == 0.0f)
            {
                y0 = (int)y; y1 = (int)y;
            }
            else
            {
                y0 = (int)y; y1 = (int)(y + 1);
            }

            int str1 = openImage.GetPixel(x0, y0).R;//擷取相鄰點的強度
            int str2 = openImage.GetPixel(x1, y0).R;
            int str3 = openImage.GetPixel(x0, y1).R;
            int str4 = openImage.GetPixel(x1, y1).R;
            float d1 = x - x0;//到四邊的距離
            float d2 = x1 - x;
            float d3 = y - y0;
            float d4 = y1 - y;
            float power1;
            if (str2 > str1)
                power1 = str1 + d1 * (str2 - str1);//上頂點
            else
                power1 = str2 + d2 * (str1 - str2);
            float power2;
            if (str4 > str3)
                power2 = str3 + d1 * (str4 - str3);
            else
                power2 = str4 + d2 * (str3 - str4);//下頂點
            float power3;
            if (power1 > power2)//對y軸方向作加權
                power3 = power1 + d3 * (power1 - power2);
            else
                power3 = power2 + d4 * (power2 - power1);
            if (power3 > 255) power3 = 255;
            if (power3 < 0) power3 = 0;
            return (int)(power3 + 0.5);//將結果四捨五入
            
        }
      

        private void btnLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog openfiledialog1;//開檔所需的對話方塊
            openfiledialog1 = new OpenFileDialog();
            openfiledialog1.Filter = "All files|*.*|Bitmap files(.bmp)|*.bmp|JPEG files(.jpg)|*.jpg";
            if (openfiledialog1.ShowDialog() == DialogResult.OK) {
                openImage = new Bitmap(openfiledialog1.FileName);
              
                picOrigin.Image = openImage;//控制功能的開放或關閉，讀檔後只能先作灰階。
                btnSave.Enabled = true;
                btnGray.Enabled = true;
                btnThreshold.Enabled = false;
                btnMedian.Enabled = false;
                btnAverage.Enabled = false;
                btnHistogram.Enabled = false;
                btnSobel.Enabled = false;
                btnRotate.Enabled = false;
                btnStrectch.Enabled = false;
                picThresholding.Visible = false;
                label9.Visible = false;
               
                pictureR.Visible = false;
                pictureG.Visible = false;
                pictureB.Visible = false;
                picBefore.Visible = false;
                picAfter.Visible = false;
           
                label6.Visible = false;
                label7.Visible = false;
                Has_rotate = true;
                rotate_angle = 0;
                picOrigin.Width = 350;
                picOrigin.Height = 350;
                picNew.Width = 350;
                picNew.Height = 350;
                picOrigin.SizeMode = PictureBoxSizeMode.StretchImage;//讓圖形方塊大小常態化
                picNew.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd;//存檔所需的對話方塊
            sfd = new SaveFileDialog();
            sfd.Filter = "All files|*.*|Bitmap files(.bmp)|*.bmp|JPEG files(.jpg)|*.jpg";
            if (sfd.ShowDialog() == DialogResult.OK) {
                openImage.Save(sfd.FileName);
                this.Text = "Save Successfully";
            }
        }

        private void btnGray_Click(object sender, EventArgs e)//灰階處理
        {
            label3.Visible = true;
            label3.Text = "R";
            label4.Visible = true;
                label4.Text = "G";
            label5.Visible = true;
            label5.Text = "B";
            label2.Text = "灰階";
            pictureR.Visible = true;
            pictureG.Visible = true;
            pictureB.Visible = true;

            Bitmap picR = new Bitmap(openImage);//儲存RGB三個色版用的點陣圖
            Bitmap picG = new Bitmap(openImage);
            Bitmap picB = new Bitmap(openImage);
            this.Text = "Processing Gray Level Extracting";
            
            for (int y = 0; y < openImage.Height; y++) {
                for (int x = 0; x < openImage.Width; x++) {

                    Color RGB = openImage.GetPixel(x, y);
                    int R = Convert.ToInt32(RGB.R);
                    int G = Convert.ToInt32(RGB.G);
                    int B = Convert.ToInt32(RGB.B);
                    int graylevel = Convert.ToInt32(RGB.R / 3 + RGB.G / 3 + RGB.B / 3);//平均混色
                    picR.SetPixel(x,y,Color.FromArgb(R,R,R));//萃取色版
                    picG.SetPixel(x, y, Color.FromArgb(G, G, G));
                    picB.SetPixel(x, y, Color.FromArgb(B, B, B));
                    openImage.SetPixel(x, y, Color.FromArgb(graylevel,graylevel,graylevel));//將權重平均的灰階圖輸出
                    
                }
                                    
            }
            picNew.Image = openImage;
            this.Text = "Gray Level Extracting Complete";
            pictureR.Image = picR;
            pictureG.Image = picG;
            pictureB.Image = picB;
            btnThreshold.Enabled = true;
            btnMedian.Enabled = true;
            btnAverage.Enabled = true;
            btnHistogram.Enabled = true;
            btnSobel.Enabled = true;
            btnRotate.Enabled = true;
            btnStrectch.Enabled = true;
            //處理完灰階之後，可以使用其他影像處理功能


        }

        //處理threshold的滑桿參數設定
        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            textBox1.Text = Convert.ToString(trackBar1.Value);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            int val = Convert.ToInt32(textBox1.Text);
            if (val < 0) val = 0;
            if (val > 255) val = 255;//手動輸入的值同樣不可超出範圍
            trackBar1.Value = val;
        }

        private void btnThreshold_Click(object sender, EventArgs e)//閾值處理
        {
          
                currentImage = new Bitmap(openImage);//暫存用

                this.Text = "Processing threshold...";//提示訊息，表示該項影像操作開始處理
            btnGray.Enabled = false;
            label3.Visible = false;
            label4.Visible = false;
            label5.Visible = false;
            label2.Text = "處理後";
            pictureR.Visible = false;
            pictureG.Visible = false;
            pictureB.Visible = false;
            picBefore.Visible = false;
            picAfter.Visible = false;
            label6.Visible = false;
            label7.Visible = false;
            picThresholding.Visible = false;
            label9.Visible = false;
            picNew.Width = 350;
            picNew.Height = 350;
            picOrigin.Width = 350;
            picOrigin.Height = 350;
            picOrigin.SizeMode = PictureBoxSizeMode.StretchImage;
            picNew.SizeMode = PictureBoxSizeMode.StretchImage;
        
            for (int y = 0; y < openImage.Height; y++)
            {
                for (int x = 0; x < openImage.Width; x++)
                {

                    Color RGB = openImage.GetPixel(x, y);
                    int R = Convert.ToInt32(RGB.R);

                    if (R > trackBar1.Value) R = 255; else R = 0;//根據設定的參數將影像二值化處理
                   
                    openImage.SetPixel(x, y, Color.FromArgb(R, R, R));//實際設定像素
                    
                }

            }

            picNew.Image = openImage;
            this.Text = "Threshold Complete";
            if (!chkChange.Checked) { openImage = currentImage; }//不保存結果，將改動過的影像還原
            if(chkKeep.Checked){picOrigin.Image = currentImage;}//不保存原圖，將原圖的影像覆蓋

        }

        private void Form1_Load(object sender, EventArgs e)
        {//初始化設定視窗大小
            this.Width = 1612;
            this.Height = 834;
            this.Left = 0;
            this.Top = 0;
        }

        //調控平均值濾鏡以及中位數濾鏡的窗口大小
        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            textBox2.Text = Convert.ToString(trackBar2.Value * 2 + 1);//將滑桿的值映射到文字方塊，代表遮罩的邊長的大小
        }

        private void btnAverage_Click(object sender, EventArgs e)
        {
           //平均值濾鏡
                currentImage = new Bitmap(openImage);

           

            btnGray.Enabled = false;
            label3.Visible = false;
            label4.Visible = false;
            label5.Visible = false;
            label2.Text = "處理後";
            pictureR.Visible = false;
            pictureG.Visible = false;
            pictureB.Visible = false;
            picBefore.Visible = false;
            picAfter.Visible = false;
            label6.Visible = false;
            label7.Visible = false;
            picThresholding.Visible = false;
            label9.Visible = false;
            picOrigin.Width = 350;
            picOrigin.Height = 350;
            picNew.Width = 350;
            picNew.Height = 350;
            picOrigin.SizeMode = PictureBoxSizeMode.StretchImage;
            picNew.SizeMode = PictureBoxSizeMode.StretchImage;
            int range = trackBar2.Value;
            for (int y = 0; y < openImage.Height; y++)//將每個像素抽出處理
            {
                for (int x = 0; x < openImage.Width; x++)
                {
                    int sum = 0;
                    for (int s = y - range; s <= y + range; s++) {//根據給定的範圍逐一掃描臨近的點
                        for (int t = x -  range; t <= x + range; t++) {
                            if (s >= 0 && t >= 0 && s < openImage.Height && t < openImage.Width) {
                                Color RGB = currentImage.GetPixel(t, s);
                                  sum += Convert.ToInt32(RGB.R);//將像素強度加總

                                
                            }
                        
                        }
                    
                    
                    }
                    int average = sum / ((range * 2 + 1) * (range * 2 + 1));//將像素強度總和除以正方形點的個數以求得平均
                    openImage.SetPixel(x, y, Color.FromArgb(average, average, average));
                    float progress = ((y + 1) / openImage.Height) * ((x + 1) / openImage.Width);
                    this.Text = "Average ......  " + Convert.ToString(progress * 100) + "%"; 
                }

            }

            picNew.Image = openImage;
            if (!chkChange.Checked) { openImage = currentImage; }
            if (chkKeep.Checked) { picOrigin.Image = currentImage; }
        }

        private void btnMedian_Click(object sender, EventArgs e)
        {
            currentImage = new Bitmap(openImage);

            //中位數濾鏡

            btnGray.Enabled = false;
            label3.Visible = false;
            label4.Visible = false;
            label5.Visible = false;
            label2.Text = "處理後";
            pictureR.Visible = false;
            pictureG.Visible = false;
            pictureB.Visible = false;
            picBefore.Visible = false;
            picAfter.Visible = false;
            label6.Visible = false;
            label7.Visible = false;
            picThresholding.Visible = false;
            label9.Visible = false;
            picOrigin.Width = 350;
            picOrigin.Height = 350;
            picNew.Width = 350;
            picNew.Height = 350;
            picOrigin.SizeMode = PictureBoxSizeMode.StretchImage;
            picNew.SizeMode = PictureBoxSizeMode.StretchImage;

           
            int range = trackBar2.Value;
            int[] midst = new int[(range * 2 + 1) * (range * 2 + 1)];//用來推算中位數所需要用到的暫存用陣列，其大小等於遮罩大小
            int top = -1;

            for (int y = 0; y < openImage.Height; y++)
            {
                for (int x = 0; x < openImage.Width; x++)
                {
                    top = -1;
                    for (int s = y - range; s <= y + range; s++)
                    {
                        for (int t = x - range; t <= x + range; t++)
                        {
                            if (s >= 0 && t >= 0 && s < openImage.Height && t < openImage.Width)
                            {
                                Color RGB = currentImage.GetPixel(t, s);
                                midst[++top] = Convert.ToInt32(RGB.R);//依次把點pop到midst陣列裡，以便作為中位數的遴選
                                


                            }

                        }


                    }
                    Array.Sort(midst);//將中位數陣列以遞增排序
                    int median = midst[(range * 2 + 1) * (range * 2 + 1) / 2];//取出最中間的那個元素
                    openImage.SetPixel(x, y, Color.FromArgb(median, median, median));//將圖素設置成鄰近點的中位數
                    float progress = ((y + 1) / openImage.Height) * ((x + 1) / openImage.Width);
                    this.Text = "Median ......  " + Convert.ToString(progress * 100) + "%";
                }

            }

            picNew.Image = openImage;
            if (!chkChange.Checked) { openImage = currentImage; }
            if (chkKeep.Checked) { picOrigin.Image = currentImage; }
            

        }

        private void btnHistogram_Click(object sender, EventArgs e)
        {//直方圖等化處理
            currentImage = new Bitmap(openImage);



            btnGray.Enabled = false;
            label3.Visible = false;
            label4.Visible = false;
            label5.Visible = false;
            label2.Text = "處理後";
            pictureR.Visible = false;
            pictureG.Visible = false;
            pictureB.Visible = false;
            picThresholding.Visible = false;
            label9.Visible = false;
            picOrigin.Width = 350;
            picOrigin.Height = 350;
            picNew.Width = 350;
            picNew.Height = 350;
            picOrigin.SizeMode = PictureBoxSizeMode.StretchImage;
            picNew.SizeMode = PictureBoxSizeMode.StretchImage;

            int[] origin_db = new int[256];//用來儲存原圖各強度的像素個數的陣列
            int[] eq_db = new int[256];//用來表示新的圖的像素對應關係的陣列
            int[] new_db = new int[256];//用來儲存新的圖各強度的像素個數的陣列
            int max = 0;//用來處理直方圖的上標

            this.Text = "Processing Histogram Equalization";

            for (int y = 0; y < openImage.Height; y++)
            {
                for (int x = 0; x < openImage.Width; x++)
                {

                    Color RGB = openImage.GetPixel(x, y);
                    int R = Convert.ToInt32(RGB.R);
                    origin_db[R]++;//將所對應到的像素強度遞增， 以計算其分布
                    if (origin_db[R] > max) max = origin_db[R];//找出有最多點的灰階強度
                }
            }
            int sum = 0;
            for (int i = 0; i < 256; i++) {
                sum += origin_db[i];//目前為止的個數和
                float p = sum / (float)(openImage.Height * openImage.Width) * 255;//實作直方圖等化處理
                eq_db[i] = Convert.ToInt32(Math.Round(p, 0, MidpointRounding.AwayFromZero));//四捨五入
                new_db[eq_db[i]] += origin_db[i];//計算新的圖像的像素分布
            
            
            }

            for (int y = 0; y < openImage.Height; y++)
            {
                for (int x = 0; x < openImage.Width; x++)
                {

                    Color RGB = openImage.GetPixel(x, y);
                    int R = Convert.ToInt32(RGB.R);
                    int new_R = eq_db[R];//進行轉換
                    openImage.SetPixel(x, y, Color.FromArgb(new_R, new_R, new_R));
                }
            }

            picNew.Image = openImage;
            if (!chkChange.Checked) { openImage = currentImage; }
            if (chkKeep.Checked) { picOrigin.Image = currentImage; }
            this.Text = "Histogram Equalization Complete";

            picBefore.Visible = true;
            picAfter.Visible = true;
            label6.Visible = true;
            label7.Visible = true;

            //繪製直方圖表
            Bitmap draw1 = new Bitmap(picAfter.Width, picAfter.Height);//展示原圖像素分布需要用到的點陣圖
            Graphics g;
            g = Graphics.FromImage(draw1);//將點陣圖當成畫布
            Pen mypen = new Pen(Color.Black);
            Pen redpen = new Pen(Color.Red);
            Pen greenpen = new Pen(Color.Green);//設置不同顏色的畫筆

            redpen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;//畫虛線
            
            g.DrawRectangle(mypen, 63, 13, 280, 90);//定義直方圖的外框
           Brush bluepen = new SolidBrush(Color.Blue);
           g.DrawString("Histogram of Gray Level", new Font("新細明體", 9), bluepen, 140, 0);//繪製標題

           for (int i = 0; i <= 250; i += 50)
           {
               g.DrawString(Convert.ToString(i), new Font("新細明體", 9), bluepen, i + 62, 105);
               g.DrawLine(redpen, i + 62, 13, i + 62, 103);//繪製橫軸刻度
               
           }
           Bitmap draw2 = new Bitmap(draw1);

           int multiply = 1;
           float temp_max = max;
           while (temp_max >= 10)
           {
               temp_max = temp_max / 10.0f;
               multiply = multiply * 10;
           }
           temp_max = (float)Math.Round(temp_max, 0, MidpointRounding.AwayFromZero);
           max = (int)temp_max * multiply;//將像素個數最大值以最高位數四捨五入

           for (int i = 0; i <= 4; i++)
           {
               string temp = Convert.ToString(max * (5 - i) / 5);//確定刻度內容
               while (temp.Length < 5) { temp = " " + temp; }//對齊
               g.DrawString(temp, new Font("新細明體", 9), bluepen, 30, 13 + 82 * i /5);
               g.DrawLine(redpen, 63, 13 + 82 * i / 5, 343, 13 + 82 * i / 5);//繪製縱軸座標
           }
           g.DrawString(Convert.ToString("    0"), new Font("新細明體", 9), bluepen, 30, 95);
          
       

           for (int i = 0; i <= 255; i++)
           {
               
               int length = (int)(Math.Round((origin_db[i] / (float)max) * 90, 0, MidpointRounding.AwayFromZero));
               //推算需要呈現的長度
               g.DrawLine(greenpen, i + 63, 103 - length, i + 63, 103);//從直方圖底部畫線
           }

            picBefore.Image = draw1;
            g.Dispose();



           
            Graphics g2;//處理後的像素分布
            g2 = Graphics.FromImage(draw2);
            max = 0;
            for (int i = 0; i <= 255; i++) {
                if (new_db[i] > max) max = new_db[i];
            }

            multiply = 1;
             temp_max = max;
            while (temp_max >= 10)
            {
                temp_max = temp_max / 10.0f;
                multiply = multiply * 10;
            }
            temp_max = (float)Math.Round(temp_max, 0, MidpointRounding.AwayFromZero);
            max = (int)temp_max * multiply;

            for (int i = 0; i <= 4; i++)
            {
                string temp = Convert.ToString(max * (5 - i) / 5);
                while (temp.Length < 5) { temp = " " + temp; }
                g2.DrawString(temp, new Font("新細明體", 9), bluepen, 30, 13 + 82 * i / 5);
                g2.DrawLine(redpen, 63, 13 + 82 * i / 5, 343, 13 + 82 * i / 5);
            }
            g2.DrawString(Convert.ToString("    0"), new Font("新細明體", 9), bluepen, 30, 95);

           

            for (int i = 0; i <= 255; i++)
            {

                int length = (int)(Math.Round((new_db[i] / (float)max) * 90, 0, MidpointRounding.AwayFromZero));

                g2.DrawLine(greenpen, i + 63, 103 - length, i + 63, 103);
            }


            picAfter.Image = draw2;
            g2.Dispose();
          


        }
        //Sobel Edge Detection 會用到的閾值滑桿
        private void trackBar4_ValueChanged(object sender, EventArgs e)
        {
            textBox3.Text = Convert.ToString(trackBar4.Value);
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            int val = Convert.ToInt32(textBox3.Text);
            if (val < 0) val = 0;
            if (val > 255) val = 255;
            trackBar4.Value = val;
        }

        private void trackBar4_Scroll(object sender, EventArgs e)
        {

        }

        private void btnSobel_Click(object sender, EventArgs e)
        {//Sobel Edge Detection and Overlapping
            currentImage = new Bitmap(openImage);
            Bitmap gx = new Bitmap(openImage);//對x方向處理的結果
            Bitmap gy = new Bitmap(openImage);//對y方向處理的結果
            Bitmap gc = new Bitmap(openImage);//結合上面兩者
            Bitmap th = new Bitmap(openImage);//綜合起來二值化後的結果
            
            int[] serialx = new int[]{-1,0,1,-2,0,2 ,-1, 0, 1};//x方向的模板
            int[] serialy = new int[] { -1, -2, -1, 0, 0, 0, 1, 2, 1 }; //y方向的模板

            btnGray.Enabled = false;
            label3.Visible = true;
            label3.Text = "Vertical";
            label4.Visible = true;
            label4.Text = "Horizontal";
            label5.Visible = true;
            label5.Text = "Combined";
            label2.Text = "處理後";
            pictureR.Visible = true;
            pictureG.Visible = true;
            pictureB.Visible = true;
            picBefore.Visible = false;
            picAfter.Visible = false;
            this.Text = "Processing Sobel Edge Detection...";
            int range = 1;
            int max = 0;
            picOrigin.Width = 350;
            picOrigin.Height = 350;
            picNew.Width = 350;
            picNew.Height = 350;
            picOrigin.SizeMode = PictureBoxSizeMode.StretchImage;
            picNew.SizeMode = PictureBoxSizeMode.StretchImage;
           
            short[,] gx_table = new short[openImage.Width,openImage.Height];//暫存像素處理過的值
            short[,] gy_table = new short[openImage.Width, openImage.Height];
            //Vertical
            for (int y = 0; y < openImage.Height; y++)
            {
                for (int x = 0; x < openImage.Width; x++)
                {
                    int sum = 0;
                    int sumy = 0;
                    int order = 0;
                    for (int s = y - range; s <= y + range; s++)
                    {
                        for (int t = x - range; t <= x + range; t++)
                        {
                            if (s >= 0 && t >= 0 && s < openImage.Height && t < openImage.Width)
                            {
                                Color RGB = openImage.GetPixel(t, s);
                                sum += Convert.ToInt32(RGB.R) * serialx[order];
                                sumy += Convert.ToInt32(RGB.R) * serialy[order++];//實際執行sobel運算
                                
                                
                            }
                           
                            gx_table[x, y] = (short)sum;
                            gy_table[x, y] = (short)sumy;
                        }
                      
                    }
                                  
                                                    }

               
                    }

           
            max = 0;
            for (int y = 0; y < openImage.Height; y++)
            {
                for (int x = 0; x < openImage.Width; x++)
                {
                    int realsumx = gx_table[x,y];
                    if (realsumx < 0) realsumx = 0;
                    if (realsumx > 255) realsumx = 255;
                
                    int realsumy = gy_table[x, y];
                    if (realsumy < 0) realsumy = 0;
                    if (realsumy > 255) realsumy = 255;
                    if (Math.Abs(gx_table[x, y]) + Math.Abs(gy_table[x, y]) > max) max = Math.Abs(gx_table[x, y]) + Math.Abs(gy_table[x, y]);//記錄最大值，以便進行伸縮。結合的結果是兩者絕對值相加
                    gx.SetPixel(x,y,Color.FromArgb(realsumx,realsumx,realsumx));//輸出對x方向處理的結果
                    gy.SetPixel(x, y, Color.FromArgb(realsumy, realsumy, realsumy));//輸出對y方向處理的結果
                }
            }

            pictureR.Image = gx;
            pictureG.Image = gy;
            float coef = 255.0f / max;
            for (int y = 0; y < openImage.Height; y++)
            {
                for (int x = 0; x < openImage.Width; x++)
                {
                    //threshold 處理
                    int realsum = (int)((Math.Abs(gx_table[x, y]) + Math.Abs(gy_table[x, y])) * coef);
                    gc.SetPixel(x, y, Color.FromArgb(realsum, realsum, realsum));
                    if (realsum > trackBar4.Value) { th.SetPixel(x, y, Color.FromArgb(255, 255, 255)); 
                    openImage.SetPixel(x,y,Color.FromArgb(0,255,0));
                    }
                    else { th.SetPixel(x, y, Color.FromArgb(0, 0, 0)); }
                   
                }
            }
       

            //Combined
            label9.Visible = true;
            picThresholding.Visible = true;
            picThresholding.Image = th;

            

            pictureB.Image = gc;
             picNew.Image = openImage;
            if (!chkChange.Checked) { openImage = currentImage; }
            if (chkKeep.Checked) { picOrigin.Image = currentImage; }
            this.Text = "Sobel Edge Detection Complete";


        }

        private void trackBar3_ValueChanged(object sender, EventArgs e)
        {
            textBox4.Text = Convert.ToString(trackBar3.Value);
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnRotate_Click(object sender, EventArgs e)
        {
            currentImage = new Bitmap(openImage);

            this.Text = "Processing Rotation...";

            btnGray.Enabled = false;
            label3.Visible = false;
            label4.Visible = false;
            label5.Visible = false;
            label2.Text = "處理後";
            pictureR.Visible = false;
            pictureG.Visible = false;
            pictureB.Visible = false;
            picBefore.Visible = false;
            picAfter.Visible = false;
            label6.Visible = false;
            label7.Visible = false;
            picThresholding.Visible = false;
            label9.Visible = false;
            picOrigin.Width = 350;
            picOrigin.Height = 350;
            picNew.Width = 350;
            picNew.Height = 350;
               picOrigin.SizeMode = PictureBoxSizeMode.StretchImage;
                picNew.SizeMode = PictureBoxSizeMode.StretchImage;

            float theta = (float)( trackBar3.Value * Math.PI / 180.0f);//旋轉的角度，以弳度表示
            double tcos = Math.Cos(theta);
            double tsin = Math.Sin(theta);
            //定義原圖的四個頂點
            int x1 = 0, x2 = openImage.Width - 1;
            int y1 = 0, y2 = 0;
            int x3 = openImage.Width - 1,y3 = openImage.Height - 1;
            int x4 = 0,y4 = openImage.Height - 1;
           
            //定義旋轉後的新頂點
            double[] px = new double[4];
            double[] py = new double[4];
            px[0] = x1 * tcos - y1 * tsin;
            px[1] = x2 * tcos - y2 * tsin;
            px[2] = x3 * tcos - y3 * tsin;
            px[3]= x4 * tcos - y4 * tsin;
            py[0] = x1 * tsin + y1 * tcos;
            py[1] = x2 * tsin + y2 * tcos;
            py[2] = x3 * tsin + y3 * tcos;
            py[3] = x4 * tsin + y4 * tcos;
           double xmax = px[0],  ymax = py[0];
           double xmin = px[0], ymin = py[0];//尋找xy方向的極值，以便求出邊長
            for (int i = 0; i < 4; i++) {
                if (px[i] > xmax) xmax = px[i];
                if (py[i] > ymax) ymax = py[i];
                if (px[i] < xmin) xmin = px[i];
                if (py[i] < ymin) ymin = py[i];
            }
            //定義新圖形的邊長
            int new_width = (int)(xmax - xmin + 0.5);
            int new_height = (int)(ymax - ymin + 0.5);
            Bitmap rot = new Bitmap(new_width, new_height);//旋轉後的圖

            double offset_x, offset_y;
            offset_x = (-((new_width - 1) / 2)) * tcos - ((new_height - 1) / 2) * tsin + (openImage.Width - 1) / 2;//定義沿著圖像中心作旋轉產生的偏移向量
            offset_y = ((new_width - 1) / 2) * tsin - ((new_height - 1) / 2) * tcos + (openImage.Height - 1) / 2;

            for (int y = 0; y < new_height; y++) {
                for (int x = 0; x < new_width; x++) {
                    float origin_x = (float)(x * tcos + y * tsin + offset_x + 0.5);
                    float origin_y = (float)(y * tcos - x * tsin + offset_y + 0.5);//推算旋轉後每一個新的點對應到哪一個舊的點
                    if (origin_x >= 0 && origin_x < openImage.Width - 1 && origin_y >= 0 && origin_y < openImage.Height - 1)//判斷算出來的點是否合法，若超出邊界，則設為黑色
                    {
                      
                        int R = bilinear_interpolation(origin_x, origin_y);//運用雙線性內插法求出像素強度。
                        rot.SetPixel(x, y, Color.FromArgb(R, R, R));


                    }
                    else {
                        rot.SetPixel(x, y, Color.FromArgb(0, 0,0));
                    }
                }
            }

            picNew.Image = rot;
            this.Text = "Rotation Complete";
            if (chkChange.Checked) { 
                openImage = rot;
                Has_rotate = true;
                rotate_angle += trackBar3.Value;
            }
            if (chkKeep.Checked) { picOrigin.Image = currentImage; }

        }

        private void trackBar5_ValueChanged(object sender, EventArgs e)
        {
            textBox5.Text = Convert.ToString(trackBar5.Value / 10.0f);
        }

        private void trackBar6_ValueChanged(object sender, EventArgs e)
        {
            textBox6.Text = Convert.ToString(trackBar6.Value / 10.0f);
        }

        private void btnStrectch_Click(object sender, EventArgs e)
        {//伸縮圖形
            currentImage = new Bitmap(openImage);
            this.Text = "Processing strectch...";
            btnGray.Enabled = false;
            label3.Visible = false;
            label4.Visible = false;
            label5.Visible = false;
            label2.Text = "處理後";
            pictureR.Visible = false;
            pictureG.Visible = false;
            pictureB.Visible = false;
            picBefore.Visible = false;
            picAfter.Visible = false;
            label6.Visible = false;
            label7.Visible = false;
            picThresholding.Visible = false;
            label9.Visible = false;

            picOrigin.SizeMode = PictureBoxSizeMode.AutoSize;//將圖形框改為自動調整大小的模式，以便看出前後變化。
            picNew.SizeMode = PictureBoxSizeMode.AutoSize;

            double coefx = 1.0 / Convert.ToDouble(textBox5.Text);//新的點對映到舊的點所需要乘上的係數
            double coefy = 1.0 / Convert.ToDouble(textBox6.Text);
            int new_width = (int)(openImage.Width * Convert.ToDouble(textBox5.Text));//新圖形預期的長寬
            int new_height = (int)(openImage.Height * Convert.ToDouble(textBox6.Text));
            Bitmap stc = new Bitmap(new_width, new_height);
            for (int y = 0; y < new_height ; y++) {
                for (int x = 0; x < new_width ; x++) {
                    double origin_x = x * coefx;
                    double origin_y = y * coefy;
                    if (origin_x >= 0 && origin_x < openImage.Width - 1 && origin_y >= 0 && origin_y < openImage.Height - 1)
                    {

                        int R = bilinear_interpolation((float)origin_x, (float)origin_y);//雙線性內插
                       stc.SetPixel(x, y, Color.FromArgb(R, R, R));


                    }
                    
                }
            }

            picNew.Image = stc;
            if (chkChange.Checked) { openImage = stc; }
            if (chkKeep.Checked) { picOrigin.Image = currentImage; }
            this.Text = "Strectch Complete";
        }

       
    }
}
