using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace example118
{
    public partial class Form1 : Form
    {
        string Conn = "Server=localhost;Database=example118;Uid=root;Pwd=qwer1234;";

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(textBox1.Text == "" || textBox2.Text == "")
            {
                MessageBox.Show("내용을 입력해주세요!");
            }
            else
            {
                //DB에 데이터 삽입
                using (MySqlConnection conn = new MySqlConnection(Conn))
                {
                    conn.Open();
                    MySqlCommand msc = new MySqlCommand("insert into example118_1(name,age) values('"+textBox1.Text+"','"+textBox2.Text+"')", conn);
                    int result = msc.ExecuteNonQuery();


                    if(result == 1)
                    {
                        MessageBox.Show("정상적으로 입력되었습니다!");
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (MySqlConnection conn = new MySqlConnection(Conn))
            {
                DataSet ds = new DataSet();
                string sql = "SELECT * FROM example118_1";
                MySqlDataAdapter adpt = new MySqlDataAdapter(sql, conn);
                adpt.Fill(ds, "example118_1");

                //받아온 table 전체를 순회
                for(int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    //한 레코드씩 리스트뷰에 집어넣는 과정
                    ListViewItem lvi = new ListViewItem();
                    lvi.Text = ds.Tables[0].Rows[i]["num"].ToString();

                    lvi.SubItems.Add(ds.Tables[0].Rows[i]["name"].ToString());
                    lvi.SubItems.Add(ds.Tables[0].Rows[i]["age"].ToString());

                    listView1.Items.Add(lvi);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if(textBox3.Text == "" || textBox4.Text == "" || textBox5.Text == "" || textBox6.Text == "" || textBox7.Text == "")
            {
                MessageBox.Show("내용을 입력해주세요!");
            }
            else
            {
                //DB INSERT
                using (MySqlConnection conn = new MySqlConnection(Conn))
                {
                    conn.Open();
                    MySqlCommand msc = new MySqlCommand("insert into example118_2(name,class1,class2,class3,class4) values('"+textBox3.Text+ "'," + textBox4.Text + "," + textBox5.Text + "," + textBox6.Text + "," + textBox7.Text + ")", conn);
                    int result = msc.ExecuteNonQuery();

                    if(result == 1)
                    {
                        MessageBox.Show("잘 저장했습니다~!");
                    }
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex != -1 && textBox8.Text != "")
            {
                //MessageBox.Show(comboBox1.SelectedItem.ToString());
                using (MySqlConnection conn = new MySqlConnection(Conn))
                {
                    DataSet ds = new DataSet();

                    string combo = comboBox1.SelectedItem.ToString();

                    string gubun = "";
                    if (combo == "국어")
                    {
                        gubun = "class1";
                    }
                    else if(combo == "수학")
                    {
                        gubun = "class2";
                    }
                    else if(combo == "사회")
                    {
                        gubun = "class3";
                    }
                    else if(combo == "과학")
                    {
                        gubun = "class4";
                    }

                    
                    string sql = "select * from example118_2 where "+ gubun + " <= "+textBox8.Text+";";
                    MySqlDataAdapter adpt = new MySqlDataAdapter(sql, conn);
                    adpt.Fill(ds, "example118_2");

                    //받아온 결과를 리스트뷰2에 출력
                    //받아온 table 전체를 순회
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        //한 레코드씩 리스트뷰에 집어넣는 과정
                        ListViewItem lvi = new ListViewItem();
                        lvi.Text = ds.Tables[0].Rows[i]["num"].ToString();

                        lvi.SubItems.Add(ds.Tables[0].Rows[i]["name"].ToString());
                        lvi.SubItems.Add(ds.Tables[0].Rows[i]["class1"].ToString());
                        lvi.SubItems.Add(ds.Tables[0].Rows[i]["class2"].ToString());
                        lvi.SubItems.Add(ds.Tables[0].Rows[i]["class3"].ToString());
                        lvi.SubItems.Add(ds.Tables[0].Rows[i]["class4"].ToString());

                        listView2.Items.Add(lvi);
                    }
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            listView2.Items.Clear();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if(timer1.Enabled == false)
            {
                timer1.Start();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //1초에 한번 센서데이터를 측정한다!
            Random rd = new Random();

            int num1 = rd.Next(0, 1024);
            int num2 = rd.Next(0, 1024);
            string date = DateTime.Now.ToString();

            //DB에 INSERT
            using (MySqlConnection conn = new MySqlConnection(Conn))
            {
                conn.Open();
                MySqlCommand msc = new MySqlCommand("insert into example118_3(sensor1,sensor2,date) values(" + num1.ToString() + "," + num2.ToString() + ",'" + date + "')", conn);
                msc.ExecuteNonQuery();
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if(timer1.Enabled == true)
            {
                timer1.Stop();
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            DataSet ds = new DataSet();
            using (MySqlConnection conn = new MySqlConnection(Conn))
            {
                
                string sql = "select * from example118_3 order by date desc limit 10";
                MySqlDataAdapter adpt = new MySqlDataAdapter(sql, conn);
                adpt.Fill(ds, "example118_3");
            }

            //이전에 그려진 내용이 있다면 초기화 한다!
            chart1.Series[0].Points.Clear();
            chart1.Series[1].Points.Clear();

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                int sensor1 = int.Parse(ds.Tables[0].Rows[i]["sensor1"].ToString());
                int sensor2 = int.Parse(ds.Tables[0].Rows[i]["sensor2"].ToString());

                chart1.Series[0].Points.AddXY(i, sensor1);
                chart1.Series[1].Points.AddXY(i, sensor2);
            }
        }
    }
}
