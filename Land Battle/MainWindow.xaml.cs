using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace Land_Battle
{
    public partial class MainWindow : Window
    {
        //массивы элементов игрового поля
        private Lines[,] verticalLines = new Lines[8, 9];
        private Lines[,] horizontalLines = new Lines[9, 8];
        private Rectangle[,] squares = new Rectangle[9, 9];

        private SolidColorBrush colorLinesDefault = new SolidColorBrush(Color.FromRgb(100, 103, 108));
        private SolidColorBrush colorSquaresDefault = new SolidColorBrush(Color.FromRgb(77, 79, 83));
        private SolidColorBrush colorRed = new SolidColorBrush(Color.FromRgb(215, 35, 35));
        private SolidColorBrush colorBlue = new SolidColorBrush(Color.FromRgb(0, 146, 202));
        private SolidColorBrush colorRedDim = new SolidColorBrush(Color.FromRgb(141, 41 ,41));
        private SolidColorBrush colorBlueDim = new SolidColorBrush(Color.FromRgb(23, 129, 141));
        private bool playerColorRed = true;

        private DispatcherTimer ShowHide = new DispatcherTimer();

        private bool onlineMode = false;

        private struct MoveCanvas
        {
            public Canvas cnv;
            public int x;
            public int angle;
        }
        MoveCanvas moveSettings = new MoveCanvas();

        public MainWindow()
        {
            InitializeComponent();

            cnvMainChat.Margin = new Thickness(350, 0, 0, 0);
            cnvMainChat.Tag = false;
            cnvMainUserList.Margin = new Thickness(0, 0, 200, 0);
            cnvMainUserList.Tag = false;

            ShowHide.Interval = new TimeSpan(0, 0, 0, 0, 20);

            #region создание элементов на игровом поле
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    //создание квадратов
                    squares[i, j] = CreateRectangle(26, 26, i * 33 + 2, j * 33 + 2, colorSquaresDefault);
                    cnvBattleField.Children.Add(squares[i, j]);
                    //по периметру
                    if (i == 0 || i == 8 || j == 0 || j == 8)
                    {
                        //угловые
                        if ((i == 0 && j == 0) ||
                            (i == 0 && j == 8) ||
                            (i == 8 && j == 0) ||
                            (i == 8 && j == 8))
                        {
                            squares[i, j].Tag = 2;
                        }
                        else
                            squares[i, j].Tag = 1;
                    }
                    //остальные
                    else
                    {
                        squares[i, j].Tag = 0;
                    }
                    
                    //создание вертикальных линий
                    if(i != 8)
                    {
                        Lines line = new Lines(CreateRectangle(3, 30, (i + 1) * 33 - 3, j * 33, colorLinesDefault), i, j);
                        
                        //обработка нажатия на вертикальную линию
                        line.Rct.MouseLeftButtonDown += (s, a) =>
                        {
                            PaintLine(true, line.X, line.Y);

                            IncrementSquareTag(line.X, line.Y);
                            IncrementSquareTag(line.X + 1, line.Y);

                            NextPlayer();
                        };

                        verticalLines[i, j] = line;
                        cnvBattleField.Children.Add(verticalLines[i, j].Rct);
                    }

                    //создание горизонтальный линий
                    if (j != 8)
                    {
                        Lines line = new Lines(CreateRectangle(30, 3, i * 33, (j + 1) * 33 - 3, colorLinesDefault), i, j);

                        //обработка нажатия на горизонтальную линию
                        line.Rct.MouseLeftButtonDown += (s, a) =>
                        {
                            PaintLine(false, line.X, line.Y);

                            IncrementSquareTag(line.X, line.Y);
                            IncrementSquareTag(line.X, line.Y + 1);

                            NextPlayer();
                        };

                        horizontalLines[i, j] = line;
                        cnvBattleField.Children.Add(horizontalLines[i, j].Rct);
                    }
                }
            }

            foreach (var line in cnvBattleField.Children.OfType<Rectangle>().Where(rct => rct.Tag == null))
            {
                line.MouseEnter += (s, a) =>
                {
                    if (playerColorRed)
                        line.Fill = colorRedDim;
                    else
                        line.Fill = colorBlueDim;
                };
                line.MouseLeave += (s, a) =>
                {
                    if(line.Tag == null)
                        line.Fill = colorLinesDefault;
                };
            }
            #endregion

            #region анимация кнопок при наведении
            foreach (var rectangle in cnvMainWindow.Children.OfType<Rectangle>())
            {
                rectangle.MouseEnter += (s, a) =>
                {
                    rectangle.Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                };
                rectangle.MouseLeave += (s, a) =>
                {
                    rectangle.Stroke = null;
                };
            }
            rctSend.MouseEnter += (s, a) =>
            {
                rctSend.Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            };
            rctSend.MouseLeave += (s, a) =>
            {
                rctSend.Stroke = null;
            };
            #endregion

            int k = 0;
            ShowHide.Tick += (s, a) =>
            {
                if (k < 10)
                {
                    if (moveSettings.cnv.Width == 350)
                    {
                        moveSettings.cnv.Margin = new Thickness(moveSettings.cnv.Margin.Left + moveSettings.x, 0, 0, 0);
                    }
                    else
                    {
                        moveSettings.cnv.Margin = new Thickness(0, 0, moveSettings.cnv.Margin.Right + moveSettings.x, 0);
                    }
                }
                else
                {
                    if (moveSettings.cnv == cnvMainChat)
                        rctChat.RenderTransform = new RotateTransform(moveSettings.angle, 13, 13);
                    else
                        rctUserList.RenderTransform = new RotateTransform(moveSettings.angle, 13, 13);

                    rctChat.IsEnabled = true;
                    rctUserList.IsEnabled = true;
                    k = -1;
                    ShowHide.Stop();
                }
                k++;
            };
        }

        private Rectangle CreateRectangle(int width, int height, int x, int y, SolidColorBrush color)
        {
            Rectangle rectangle = new Rectangle();
            rectangle.Width = width;
            rectangle.Height = height;
            rectangle.Fill = color;
            rectangle.Margin = new Thickness(x, y, 0, 0);
            return rectangle;
        }

        private void PaintLine(bool isVertical, int x, int y)
        {
            if (playerColorRed)
            {
                if (isVertical)
                {
                    verticalLines[x, y].Rct.Fill = colorRed;
                    verticalLines[x, y].Rct.IsEnabled = false;
                    verticalLines[x, y].Rct.Tag = 0;
                }
                else
                {
                    horizontalLines[x, y].Rct.Fill = colorRed;
                    horizontalLines[x, y].Rct.IsEnabled = false;
                    horizontalLines[x, y].Rct.Tag = 0;
                }
            }
            else
            {
                if (isVertical)
                {
                    verticalLines[x, y].Rct.Fill = colorBlue;
                    verticalLines[x, y].Rct.IsEnabled = false;
                    verticalLines[x, y].Rct.Tag = 0;
                }
                else
                {
                    horizontalLines[x, y].Rct.Fill = colorBlue;
                    horizontalLines[x, y].Rct.IsEnabled = false;
                    horizontalLines[x, y].Rct.Tag = 0;
                }
            }

            if (onlineMode)
            {

            }
        }

        private void IncrementSquareTag(int i, int j)
        {
            if(squares[i, j].Fill == colorSquaresDefault)
            {
                squares[i, j].Tag = (int)squares[i, j].Tag + 1;
                if((int)squares[i, j].Tag == 4)
                {
                    if (playerColorRed)
                    {
                        squares[i, j].Fill = colorRed;
                        lblRedCount.Content = int.Parse(lblRedCount.Content.ToString()) + 1;
                    }
                    else
                    {
                        squares[i, j].Fill = colorBlue;
                        lblBlueCount.Content = int.Parse(lblBlueCount.Content.ToString()) + 1;
                    }

                    if (onlineMode)
                    {

                    }
                }
            }
        }

        private void NextPlayer()
        {
            if (onlineMode)
            {
                foreach (var line in cnvBattleField.Children.OfType<Rectangle>().Where(rct => rct.Tag == null))
                {
                    line.IsEnabled = false;
                }
            }
            else
                if (playerColorRed)
                    playerColorRed = false;
                else
                    playerColorRed = true;
        }

        private void rctMinimize_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void rctClose_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void cnvMainWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try { DragMove(); }
            catch { }
        }

        private void rctChat_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            rctChat.IsEnabled = false;
            rctUserList.IsEnabled = false;

            if ((bool)cnvMainChat.Tag == false)
            {
                moveSettings.cnv = cnvMainChat;
                moveSettings.x = -35;
                moveSettings.angle = 180;
                ShowHide.Start();
                cnvMainChat.Tag = true;
            }
            else
            {
                moveSettings.cnv = cnvMainChat;
                moveSettings.x = 35;
                moveSettings.angle = 0;
                ShowHide.Start();
                cnvMainChat.Tag = false;
            }
        }

        private void rctUserList_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            rctChat.IsEnabled = false;
            rctUserList.IsEnabled = false;

            if ((bool)cnvMainUserList.Tag == false)
            {
                moveSettings.cnv = cnvMainUserList;
                moveSettings.x = -20;
                moveSettings.angle = 180;
                ShowHide.Start();
                cnvMainUserList.Tag = true;
            }
            else
            {
                moveSettings.cnv = cnvMainUserList;
                moveSettings.x = 20;
                moveSettings.angle = 0;
                ShowHide.Start();
                cnvMainUserList.Tag = false;
            }
        }
    }
}
