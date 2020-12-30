using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MazeGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //public int x { get; set; }
        
        public class Vector
        {
            public int x { get; set; }
            public int y { get; set; }
        
        public Vector(int X, int Y) {
                x = X;
                y = Y;
            }
        }
        static Vector size { get; set; } = new Vector(31, 31);
        bool[,] table = new bool[size.x, size.y];
        public void Clear()
        {
            table = new bool[size.x, size.y];
            Random r = new Random();

            for (int i = 0; i < size.x; i++)
            {
                for (int j = 0; j < size.y; j++)
                {
                    table[i, j] = false;
                    
                    if (i == 0 || j == 0 || i == size.x - 1 || j == size.y - 1)
                    {
                        table[i, j] = true;
                    }
                    
                }
               
            }
            table[(r.Next(1, (size.x - 1) / 2)) * 2 + 1, 0] = false;
            table[(r.Next(1, (size.x - 1) / 2)) * 2 + 1, size.y - 1] = false;

        }
        public void Line(Vector start,Vector end, int stroke = 4, Color ?color=null)
        {
            color ??= Colors.White;

            int multpX = (int)Kanvasz.Width / (size.x - 1);
            int multpY = (int)Kanvasz.Height / (size.y - 1);
            Line l = new Line
            {
                X1 = start.x * multpX,
                Y1 = start.y * multpY,
                X2 = end.x * multpX,
                Y2 = end.y * multpY,
                Stroke = new SolidColorBrush((Color)color),
                StrokeThickness = stroke
            };
            Kanvasz.Children.Add(l);
        }


        public void Draw()
        {
            Kanvasz.Children.Clear();
            for (int i = 0; i < size.x; i++)
            {
                Line(new Vector(i, 0), new Vector(i, size.y - 1),1, color: Colors.Black);
            }
            for (int i = 0; i < size.y; i++)
            {
                Line(new Vector(0, i), new Vector(size.x - 1, i),1, color: Colors.Black);
            }
            for (int y = 0; y < size.y; y++)
            {
                for (int x = 0; x < size.x; x++)
                {                   
                    if (table[x,y]==true)
                    {
                        if ( y+1<size.y &&table[x, y + 1] == true)
                        {
                            Line(new Vector(x, y), new Vector( x , y + 1));
                        }
                        if (x+1<size.x &&table[x+1, y] == true)
                        {
                            Line(new Vector(x, y), new Vector( x + 1, y));
                             
                        }
                    }
                }
            }
            

        }
        public void Division(Vector TopL,Vector BotR ) //Recursive Division
        {
            int w = BotR.x - TopL.x;
            int h = BotR.y - TopL.y;
            int door;
            //Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() => {
                    
                        
                Random random = new Random();
                if (BotR.x - TopL.x < 3 || BotR.y - TopL.y < 3)
                {
                    return;
                }

                if ((BotR.x - TopL.x) > (BotR.y - TopL.y) || ((BotR.x - TopL.x) == (BotR.y - TopL.y) && random.Next(0, 2) == 0))
                {
                    //szélesebb --> függőlegesvágás
                    int col = (random.Next(TopL.x + 2, BotR.x - 1)) / 2 * 2;
                    if (h == 3)
                    {
                        door = (random.Next(TopL.y+1, BotR.y));
                    }
                    else
                    {
                        door = (random.Next(TopL.y, BotR.y - 1)) / 2 * 2 + 1;
                    }
                    for (int i = TopL.y; i < BotR.y; i++)
                    {
                        table[col, i] = i != door;
                    }
                    Division(new Vector(TopL.x, TopL.y), new Vector(col, BotR.y));
                    Division(new Vector(col, TopL.y), new Vector(BotR.x, BotR.y));

                }
                else
                {
                    int row = (random.Next(TopL.y + 2, BotR.y - 1)) / 2 * 2;
                    if (w == 3)
                    {
                        door = (random.Next(TopL.x+1, BotR.x));
                    }
                    else
                    {
                        door = (random.Next(TopL.x, BotR.x - 1)) / 2 * 2 + 1;
                    }
                    for (int i = TopL.x; i < BotR.x; i++)
                    {
                        table[i, row] = i != door;
                    }
                    Division(new Vector(TopL.x, TopL.y), new Vector(BotR.x, row));
                    Division(new Vector(TopL.x, row), new Vector(BotR.x, BotR.y));
                }
                //Draw();
                //Thread.Sleep(30);
                //}));
        }
        
        public MainWindow()
        {
            InitializeComponent();
            DataContext = size;
            Clear();
            Division(new Vector(1, 1), new Vector(size.x - 1, size.y - 1));
            Draw();
            
        }

        private void Gomb_Click(object sender, RoutedEventArgs e)
        {
            size.x = size.x * 2 / 2 + 1;
            size.y = size.y * 2 / 2 + 1;
            Clear();
            Division(new Vector(1, 1), new Vector(size.x - 1, size.y - 1));
            Draw();
            Trace.WriteLine(size.x);
        }

    }
}
