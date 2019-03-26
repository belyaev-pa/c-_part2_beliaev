using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
// Создаем шаблон приложения, где подключаем модули
namespace MyGame
{
    static class Game
    {
        private static BufferedGraphicsContext _context;
        public static BufferedGraphics Buffer;
        // Свойства
        // Ширина и высота игрового поля
        public static int Width { get; set; }
        public static int Height { get; set; }
        public static BaseObject[] _objs;
        static Game()
        {
        }
        public static void Init(Form form)
        {
            // Графическое устройство для вывода графики            
            Graphics g;
            // Предоставляет доступ к главному буферу графического контекста для текущего приложения
            _context = BufferedGraphicsManager.Current;
            g = form.CreateGraphics();
            // Создаем объект (поверхность рисования) и связываем его с формой
            // Запоминаем размеры формы
            Width = form.ClientSize.Width;
            Height = form.ClientSize.Height;
            // Связываем буфер в памяти с графическим объектом, чтобы рисовать в буфере
            Buffer = _context.Allocate(g, new Rectangle(0, 0, Width, Height));
            Load();
            Timer timer = new Timer {Interval = 100};
            timer.Start();
            timer.Tick += Timer_Tick;

        }
        public static void Draw()
        {
            // Проверяем вывод графики
            Buffer.Graphics.Clear(Color.Black);
            Buffer.Graphics.DrawRectangle(Pens.White, new Rectangle(100, 100, 200, 200));
            Buffer.Graphics.FillEllipse(Brushes.Wheat, new Rectangle(100, 100, 200, 200));
            Buffer.Render();

            Buffer.Graphics.Clear(Color.Black);
            foreach (BaseObject obj in _objs)
                obj.Draw();
            Buffer.Render();
        }        
        public static void Load()
        {
            //_objs = new BaseObject[30];
            //for (int i = 0; i < _objs.Length; i++)
            //    _objs[i] = new BaseObject(new Point(600, i * 20), new Point(15 - i, 15 - i), new Size(20, 20));
            _objs = new BaseObject[40];
            for (int i = 0; i < _objs.Length; i++)
                _objs[i] = new ImageStar(new Point(600, i * 20), new Point(-i, 1), new Size(2, 2));
        }        
        public static void Update()
        {
            foreach (BaseObject obj in _objs)
                obj.Update();
        }
        private static void Timer_Tick(object sender, EventArgs e)
        {
            Draw();
            Update();
        }
    }
    class BaseObject
    {
        protected Point Pos;
        protected Point Dir;
        protected Size Size;
        public BaseObject(Point pos, Point dir, Size size)
        {
            Pos = pos;
            Dir = dir;
            Size = size;
        }
        public virtual void Draw()
        {
            Game.Buffer.Graphics.DrawEllipse(Pens.White, Pos.X, Pos.Y, Size.Width, Size.Height);
        }
        public virtual void Update()
        {
            Pos.X = Pos.X + Dir.X;
            Pos.Y = Pos.Y + Dir.Y;
            if (Pos.X < 0) Dir.X = -Dir.X;
            if (Pos.X > Game.Width) Dir.X = -Dir.X;
            if (Pos.Y < 0) Dir.Y = -Dir.Y;
            if (Pos.Y > Game.Height) Dir.Y = -Dir.Y;
        }
    }
    class Star: BaseObject
    {
        public Star(Point pos, Point dir, Size size):base(pos,dir,size)
        {
        } 
        public override void Draw()
        {
            Game.Buffer.Graphics.DrawLine(Pens.White, Pos.X, Pos.Y, Pos.X + Size.Width, Pos.Y + Size.Height);
            Game.Buffer.Graphics.DrawLine(Pens.White, Pos.X + Size.Width, Pos.Y, Pos.X, Pos.Y + Size.Height);
        }
        public override void Update()
        {
            Pos.X = Pos.X - Dir.X;
            if (Pos.X < 0) Pos.X = Game.Width + Size.Width;
        }
    }
    class ImageStar: BaseObject
    {
        /// <summary>
        /// Класс ImageStar    
        /// наследуется от баозового объекта     
        /// рисует случайную картинку из списка во время обновления
        /// так же добавили немного неопределенности в передвижение картинок        
        /// </summary>
        private Random _rnd = new Random();
        public ImageStar(Point pos, Point dir, Size size):base(pos,dir,size)
        {            
        } 
        public override void Draw()
        {            
            string fileName1 = @"images/star1.ico";
            string fileName2 = @"images/star2.ico";
            string fileName3 = @"images/star3.ico";            
            List<string> fileList = new List<string>() { fileName1, fileName2, fileName3 };                        
            Game.Buffer.Graphics.DrawImage(Image.FromFile(fileList[_rnd.Next(fileList.Count)]), Pos.X, Pos.Y, 8,8);
        }
        public override void Update()
        {
            Pos.X = Pos.X + Dir.X;
            Pos.Y = Pos.Y + Dir.Y;            
            if (Pos.X < 0) {
                Dir.X = -Dir.X;
                if (_rnd.Next(0, 2) == 0 && Pos.Y > 0 && Pos.Y < Game.Height)
                    Dir.Y = -Dir.Y;
            }
            if (Pos.X > Game.Width) {            
                Dir.X = -Dir.X;
                if (_rnd.Next(0, 2) == 0 && Pos.Y > 0 && Pos.Y < Game.Height)
                    Dir.Y = -Dir.Y;
            }
            if (Pos.Y < 0) {
                Dir.Y = -Dir.Y;
                if (_rnd.Next(0, 2) == 0 && Pos.X > 0 && Pos.X < Game.Width)
                    Dir.X = -Dir.X;
            }
            if (Pos.Y > Game.Height){
                Dir.Y = -Dir.Y;
                if (_rnd.Next(0, 2) == 0 && Pos.X > 0 && Pos.X < Game.Width)
                    Dir.X = -Dir.X;
            } 
        }
        
    }
    class Program
    {
        static void Main(string[] args)
        {            
            Form form = new Form();
            form.Width = 800;
            form.Height = 600;              
            Game.Init(form);
            form.Show();
            Game.Draw();
            Application.Run(form);
        }
    }
}