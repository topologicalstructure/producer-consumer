using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using Sunny.UI;

namespace Producer_consumer
{
    public partial class Form2 : Form
    {
        class Queue                                                      //循环队列类
        {
            public int rear, front;                                      //队头、队尾
            public int[] elements;                                       //存队数组
            private int maxSize;                                         //队列容量
            public Queue(int a)                                          //构造函数
            {
                maxSize = a;
                front = rear = 0;
                elements = new int[a];
            }
            public void EnQueue(int item)                                 //item进队
            {
                if (IsFull())
                {
                    Console.WriteLine("队列已满，无法再进队列！");
                    return;
                }
                elements[rear] = item;
                rear = (rear + 1) % maxSize;
            }
            public void DeQueue()                                         //队头出队
            {
                if (IsEmpty())
                {
                    Console.WriteLine("队列已空，无法再出队列！");
                    return;
                }
                front = (front + 1) % maxSize;
            }
            public int GetFront()                                          //取队头值
            {
                return elements[front];
            }
            public bool IsEmpty()                                          //判断队列是否为空
            {
                return front == rear;
            }
            public bool IsFull()                                           //判断队列是否已满
            {
                return (rear + 1) % maxSize == front;
            }
        };
        class Producer_consumer_answer                                //用于解决生产者消费者问题的类
        {
            private Queue buffer = new Queue(20);                     //共享缓冲区buffer
            private Semaphore empty = new Semaphore(20, 20);          //空数目信号量
            private Semaphore full = new Semaphore(0, 20);            //满数目信号量
            private Semaphore mutex = new Semaphore(1, 1);            //互斥信号量
            private int proN, conN;                                   //生产者和消费者的数量
            public Producer_consumer_answer(int m, int n)
            {
                proN = m;
                conN = n;
            }
            private void Producer(object num)                         //生产者方法
            {
                for (int i = 0; i < conN; i++)
                {
                    for (int j = 1; j <= 20; j++)
                    {
                        empty.WaitOne();
                        mutex.WaitOne();
                        Thread.Sleep(Delaytime);
                        //修改buffer插入位置对应label的值和背景色
                        LabelArray[buffer.rear].Invoke(new EventHandler(delegate
                        {
                            LabelArray[buffer.rear].Text = j.ToString();
                            LabelArray[buffer.rear].BackColor = Color.FromArgb(255, 0x77, 0xb0, 0xe9);
                        }));
                        pronum++;
                        //修改操作数显示
                        LabelArray[20].Invoke(new EventHandler(delegate
                        {
                            LabelArray[20].Text = "已执行操作数/总操作数：" + pronum + '/' + 20 * proN *conN;
                        }));
                        //修改进度条数值
                        uiBarArray[0].Invoke(new EventHandler(delegate
                        {
                            uiBarArray[0].Value = pronum;
                        }));
                        //将生产操作记录加入表格
                        DataArray[0].Invoke(new EventHandler(delegate
                        {
                            int index = DataArray[0].Rows.Add();
                            DataArray[0].Rows[index].Cells[0].Value = "生产者" + num;
                            DataArray[0].Rows[index].Cells[1].Value = ProduceID[(int)num];
                            DataArray[0].Rows[index].Cells[2].Value = "生产数据" + j;
                        }));
                        buffer.EnQueue(j);
                        //移动in指针图像
                        PictureArray[0].Invoke(new EventHandler(delegate
                         {
                             PictureArray[0].Location = new System.Drawing.Point(LabelArray[buffer.rear].Left + LabelArray[buffer.rear].Width / 2 - PictureArray[0].Width / 2, LabelArray[buffer.rear].Top - PictureArray[0].Height);
                         }));
                        mutex.Release();
                        full.Release();
                    }
                }
            }
            private void Consumer(object num)                            //消费者方法
            {
                for (int i = 1; i <= 20 * proN * conN; i++)
                {
                    full.WaitOne();
                    mutex.WaitOne();
                    Thread.Sleep(Delaytime);
                    //清除删除位置背景色
                    LabelArray[buffer.rear].Invoke(new EventHandler(delegate
                    {
                        LabelArray[buffer.front].BackColor = Color.FromArgb(255, 255, 255, 255);
                    }));
                    connum++;
                    //修改操作数显示
                    LabelArray[21].Invoke(new EventHandler(delegate
                    {
                        LabelArray[21].Text = "已执行操作数/总操作数：" + connum + '/' + 20 * proN * conN;
                    }));
                    //修改进度条数值
                    uiBarArray[1].Invoke(new EventHandler(delegate
                    {
                        uiBarArray[1].Value = connum;
                    }));
                    //将消费操作记录加入表格
                    DataArray[1].Invoke(new EventHandler(delegate
                    {
                        int index = DataArray[1].Rows.Add();
                        DataArray[1].Rows[index].Cells[0].Value = "消费者" + num;
                        DataArray[1].Rows[index].Cells[1].Value = ConsumerID[(int)num];
                        DataArray[1].Rows[index].Cells[2].Value = "消费数据" + buffer.GetFront();
                    }));
                    buffer.DeQueue();
                    //移动out指针图像
                    PictureArray[1].Invoke(new EventHandler(delegate
                    {
                        PictureArray[1].Location = new System.Drawing.Point(LabelArray[buffer.front].Left + LabelArray[buffer.front].Width / 2 - PictureArray[1].Width / 2, LabelArray[buffer.front].Top + PictureArray[1].Height + 2);
                    }));
                    mutex.Release();
                    empty.Release();
                }

            }
            public void Answer()
            {
                Queue q = new Queue(20);
                Thread[] produce = new Thread[proN];
                Thread[] consumer = new Thread[conN];
                //创建所有所需的生产者和消费者线程并获取线程标识符
                for (int i = 0; i < proN; i++)
                {
                    produce[i] = new Thread(Producer);
                    produce[i].IsBackground = true;
                    ProduceID[i] = produce[i].ManagedThreadId.ToString();
                }
                for (int i = 0; i < conN; i++)
                {
                    consumer[i] = new Thread(Consumer);
                    consumer[i].IsBackground = true;
                    ConsumerID[i] = consumer[i].ManagedThreadId.ToString();
                }
                //启动所有线程
                for (int i = 0; i < proN; i++)
                {
                    produce[i].Start(i);
                }
                for (int i = 0; i < conN; i++)
                {
                    consumer[i].Start(i);
                }
            }
        }
        private static Label[] LabelArray;
        private static PictureBox[] PictureArray;
        private static DataGridView[] DataArray;
        private static UIProcessBar[] uiBarArray;
        private static string[] ProduceID;                   //存放所有生产者线程标识符的数组
        private static string[] ConsumerID;                  //存放所有消费者线程标识符的数组
        private static int Delaytime = 300;                  //延迟时间
        private static int pronum = 0, connum = 0;           //已经完成的生产和消费操作数
        public Form2(int m, int n)
        {
            InitializeComponent();
            //初始化控件数组
            LabelArray = new Label[] { label1, label2, label3, label4, label5, label6, label7, label8, label9, label10, label11, label12, label13, label14, label15, label16, label17, label18, label19, label20, label22, label24 };
            PictureArray = new PictureBox[] { pictureBox1, pictureBox2 };
            DataArray = new DataGridView[] { dataGridView1, dataGridView2 };
            uiBarArray = new UIProcessBar[] { uiProcessBar1, uiProcessBar2 };
            ProduceID = new string[m];
            ConsumerID = new string[n];
            //初始化信息
            label21.Text = "生产者数量：" + m;
            label22.Text = "已执行操作数/总操作数：" + 0 + '/' + 20 * m * n;
            label23.Text = "消费者数量：" + n;
            label24.Text = "已执行操作数/总操作数：" + 0 + '/' + 20 * m * n;
            uiProcessBar1.Maximum = 20 * m * n;
            uiProcessBar2.Maximum = 20 * m * n;
            //开始演示生产者消费者模型
            Producer_consumer_answer pca = new Producer_consumer_answer(m, n);
            pca.Answer();
        }
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label26_Click(object sender, EventArgs e)
        {

        }

        private void uiTrackBar1_ValueChanged(object sender, EventArgs e)    //TrackBar被拖动后修改延迟时间值
        {
            Delaytime = uiTrackBar1.Value;
        }

    }
}
