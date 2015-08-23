using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;
using System.Data.SQLite;

namespace PhotoTransformByte__
{
    public partial class Form1 : Form
    {
        string ConnStr = @"Data Source=E:\环境&Navicat\库文件\001.db;";//建立连接字符串
        SQLiteConnection sqlconn;//连接对象
        SQLiteCommand cmd;//SQLite命令对象
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "选择要转换的图片";
            dlg.Filter = "Image files (*.jpg;*.bmp;*.gif)|*.jpg*.jpeg;*.gif;*.bmp|AllFiles (*.*)|*.*";
            if (DialogResult.OK == dlg.ShowDialog())
            {
                ImgToBase64String(dlg.FileName);
            }  
        }
        private void ImgToBase64String(string Imagefilename)
        {
            try
            {
                Bitmap bmp = new Bitmap(Imagefilename);
                this.pictureBox1.Image = bmp;
                FileStream fs = new FileStream(Imagefilename + ".txt", FileMode.Create);
                StreamWriter sw = new StreamWriter(fs);

                MemoryStream ms = new MemoryStream();
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] arr = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(arr, 0, (int)ms.Length);
                ms.Close();
                String strbaser64 = Convert.ToBase64String(arr);
                sw.Write(strbaser64);


                sqlconn = new SQLiteConnection(ConnStr);//建立SQLite连接
                sqlconn.Open();

                cmd = new SQLiteCommand(sqlconn);//建立SQLite命令对象
                string sqlline = "insert into SQLite04 values("+ textBox1.Text +",'" + strbaser64 + "')";//构造SQL语句
                cmd.CommandText = sqlline;//向命令对象传递SQL语句
                cmd.ExecuteNonQuery();//执行SQL语句,返回更改行数
                sqlconn.Close();


                sw.Close();
                fs.Close();
                MessageBox.Show("转换成功!");             
            }
            catch (Exception ex)
            {
                MessageBox.Show("ImgToBase64String 转换失败/nException:" + ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "选择要转换的base64编码的文本";
            dlg.Filter = "txt files|*.txt";
            if (DialogResult.OK == dlg.ShowDialog())
            {
                Base64StringToImage(dlg.FileName);
            }  
        }
        private void Base64StringToImage(string txtFileName)
        {
            try
            {
                FileStream ifs = new FileStream(txtFileName, FileMode.Open, FileAccess.Read);
                StreamReader sr = new StreamReader(ifs);

                String inputStr = sr.ReadToEnd();
                byte[] arr = Convert.FromBase64String(inputStr);
                MemoryStream ms = new MemoryStream(arr);
                Bitmap bmp = new Bitmap(ms);

                bmp.Save(txtFileName + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                //bmp.Save(txtFileName + ".bmp", ImageFormat.Bmp);  
                //bmp.Save(txtFileName + ".gif", ImageFormat.Gif);  
                //bmp.Save(txtFileName + ".png", ImageFormat.Png);  
                ms.Close();
                sr.Close();
                ifs.Close();
                this.pictureBox1.Image = bmp ;
                MessageBox.Show("转换成功！");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Base64StringToImage 转换失败/nException：" + ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            sqlconn = new SQLiteConnection(ConnStr);//建立SQLite连接
            sqlconn.Open();

            cmd = new SQLiteCommand(sqlconn);//建立SQLite命令对象
            string sqlline = "select 照片 from SQLite04 where 学号=" + textBox1.Text;//构造SQL语句
            cmd.CommandText = sqlline;//向命令对象传递SQL语句

            SQLiteDataReader sldr ;
            sldr = cmd.ExecuteReader();//将sqlline中的值写入sldr
           

//          textBox2.Text = sldr.GetString(1);

            StringBuilder photo = new StringBuilder();
            while (sldr.Read())
            {
                photo.Append(sldr.GetString(0));
            }
   //         textBox2.Text = photo.ToString();

            sqlconn.Close();

            String inputStr = photo.ToString();
            byte[] arr = Convert.FromBase64String(inputStr);
            MemoryStream ms = new MemoryStream(arr);
            Bitmap bmp = new Bitmap(ms);
            ms.Close();

            this.pictureBox1.Image = bmp;
        }

    }
}
