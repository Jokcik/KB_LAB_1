using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace lab_1_CSharp
{
    public partial class Form1 : Form
    {
        // Угол поворота лепестков
        private int _angle;
        private readonly Random _random = new Random();

        private int _n;
        private readonly List<float> _listHeight = new List<float>();
        private readonly List<Brush> _listFill = new List<Brush>();
        
        public Form1()
        {
            InitializeComponent();
            var text = new TextBox
            {
                Left = 5,
                Top = 5,
                Width = 100
            };

            var button = new Button
            {
                Left = 5,
                Top = text.Height + 10,
                Width = 100,
                Text = @"Построить"
            };
            button.Click += (sender, args) =>
            {
                int result;
                if (!int.TryParse(text.Text, out result))
                {
                    MessageBox.Show(this, $@"'{text.Text}' не является числом");
                    return;
                }
                _listHeight.Clear();
                _listFill.Clear();
                _angle = 0;
                _n = int.Parse(text.Text);
                Invalidate();
            };

            Controls.Add(text);
            Controls.Add(button);
            
            // Включаем двойную буферизацию
            SetStyle(ControlStyles.DoubleBuffer |
                     ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.ResizeRedraw, // Перерисовывать при изменении размера окна
                true);
            UpdateStyles();
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            if (_n == 0) return;
            
            var g = e.Graphics;

            var bounds = g.VisibleClipBounds;

            // Радиус большей окружности
            float radius;
            if (bounds.Width > bounds.Height)
                radius = bounds.Height / 5; // 
            else radius = bounds.Width / 5;

            // Если размеры окна маленькие, ничего не выводить
            if (bounds.Width < 30 || bounds.Height < 30)
                return;

            // Координаты центра окружности
            var center = new PointF(bounds.Width / 2, bounds.Height / 2);

            // Задаём область прорисовки круга
            var rect = new RectangleF(center.X - radius, center.Y - radius, radius * 2, radius * 2);
                
            // Рисуем круг
            g.FillEllipse(Brushes.Yellow, rect);

            var dlinaCircle = 2 * Math.PI * radius;
            var h = (float)dlinaCircle / _n;

//            var brush = ;
            var solid = 165 / (_n/2);
            for (int i = 1; i <= _n / 2; ++i)
            {                
                _listFill.Add(new SolidBrush(Color.FromArgb(255, (byte) 255, (byte) i * solid, (byte) 0)));
            }

            for (int i = 0; i < _n / 2; ++i)
            {
                _listFill.Add(_listFill[(_n / 2) - 1 - i]);
            }
            
            if (_listHeight.Count < _n)
            {
                _listHeight.Add(1 + (float)_random.NextDouble() * 2);   
            }
            Tuple<float, float> tuple = DrawTriangle(g, _listHeight[0], center.X, center.Y, radius, center.X, center.Y - radius, h, false);
            for (int i = 0; i < Math.Abs(_angle); ++i)
            {
                tuple = DrawTriangle(g, _listHeight[1], center.X, center.Y, radius, tuple.Item1, tuple.Item2, h, false);
      
            }
            
            for (var i = 0; i < _n; ++i)
            {
                if (_listHeight.Count < _n)
                {
                    _listHeight.Add(1 + (float)_random.NextDouble() * 2);   
                }
                tuple = DrawTriangle(g, _listHeight[i], center.X, center.Y, radius, tuple.Item1, tuple.Item2, h, true, 
                    _listFill[i % _listFill.Count]);    
            }
            
            base.OnPaint(e);
        }

        private Tuple<float, float> DrawTriangle(Graphics g, float k, float x0C, float y0C, float r0, float x1C, float y1C, 
            float r1, bool draw = true, Brush brush = null)
        {
            var d = r0;
            var b = (r0 * r0 - r1 * r1 + d * d) / (2 * d);
            var px1 = x0C + b * (x1C - x0C) / d;
            var py1 = y0C + b * (y1C - y0C) / d;

            var h = Math.Sqrt(r0 * r0 - b * b);
            
            var ax = (float)(px1 + h * (y1C - y0C) / d);
            var ay = (float)(py1 - h * (x1C - x0C) / d);
            var bx = (float)(px1 - h * (y1C - y0C) / d);
            var by = (float)(py1 + h * (x1C - x0C) / d);

            if (_angle < 0)
            {
                ax = bx;
                ay = by;
            }
            
            var cx = (x1C + ax) / 2;
            var cy = (y1C + ay) / 2;
            
            var vx = (cx - x0C) * k + x0C;            
            var vy = (cy - y0C) * k + y0C;
            
            if (draw && brush != null)
            {
                g.FillPolygon(brush, 
                    new[] {new PointF(x1C, y1C), new PointF(ax, ay), new PointF(vx, vy)});
            }
            return new Tuple<float, float>(ax, ay);
        }

        // Обработчик события прокрутки колеса мыши
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            _angle += e.Delta > 0 ? 1 : -1;
            Invalidate(); // Обновляем окно
            base.OnMouseWheel(e);
        }
    }
    
   
}