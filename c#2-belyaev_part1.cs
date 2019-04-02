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
        // private static Bullet[] _bullets;
        private static Bullet _bullet;
        private static Asteroid[] _asteroids;
        private static ImageStar[] _stars;
        private static Image backgroundImage = Image.FromFile(@"images/background.jpg");
        private static Random rnd  = new Random();
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
            if (Width > 1200 || Width < 0 || Height > 750 || Height < 0) throw new ArgumentOutOfRangeException();
            // Связываем буфер в памяти с графическим объектом, чтобы рисовать в буфере
            Buffer = _context.Allocate(g, new Rectangle(0, 0, Width, Height));
            Load();
            Timer timer = new Timer {Interval = 100};
            timer.Start();
            timer.Tick += Timer_Tick;

        }
        public static void Draw()
        {
            Buffer.Graphics.Clear(Color.Black);
            Buffer.Graphics.DrawImage(backgroundImage, 0, 0 , Width, Height);
            foreach (BaseObject obj in _objs)
                obj.Draw();
            foreach (Asteroid obj in _asteroids)
                obj.Draw();
            foreach (ImageStar obj in _stars)
                obj.Draw();
            // foreach (Bullet obj in _bullets)
            //     obj.Draw();
            _bullet.Draw();
            Buffer.Render();
        }      
        public static void Load()
        {
            _objs = new BaseObject[30];
            // _bullets = new Bullet[5];
            _bullet = new Bullet(new Point(0, Height/2), new Point(10, 0), new Size(12, 3));
            _asteroids = new Asteroid[30];
            _stars = new ImageStar[30];
            for (var i = 0; i < _objs.Length; i++)
            {
                int r = rnd.Next(5, 50);
                _objs[i] = new Star(new Point(600, rnd.Next(0, Game.Height)), new Point(-r, r), new Size(3, 3));                
            }
            for (var i = 0; i < _stars.Length; i++)
            {
                int r = rnd.Next(5, 10);
                _stars[i] = new ImageStar(new Point(rnd.Next(0, Width), rnd.Next(0, Height)), 
                                          new Point(r, r), 
                                          new Size(2, 2));
            }
            for (var i = 0; i < _asteroids.Length; i++)
            {
                int r = rnd.Next(5, 50);
                _asteroids[i] = new Asteroid(new Point(600, rnd.Next(0, Game.Height)), new Point(-r / 5, r), new Size(r, r));
            }
            // for (var i = 0; i < _asteroids.Length; i++)
            // {
            //     int r = rnd.Next(5, 50);
            //     _bullets[i] = new Bullet(new Point(0, Height/2), new Point(10, 0), new Size(12, 3));
            // }
            
        }        
        public static void Update()
        {
            foreach (BaseObject obj in _objs)
                obj.Update();
            foreach (Asteroid obj in _asteroids)
            {
                obj.Update();
                if (obj.Collision(_bullet)) 
                {
                    System.Media.SystemSounds.Hand.Play(); 
                    obj.Pos.Y = rnd.Next(0, Height);
                    obj.Pos.X = rnd.Next(0, Width);
                }
            }
            foreach (ImageStar obj in _stars)
            {
                obj.Update();
                if (obj.Collision(_bullet)) 
                {
                    System.Media.SystemSounds.Hand.Play(); 
                    obj.Pos.Y = rnd.Next(0, Height);
                    obj.Pos.X = rnd.Next(0, Width);
                }
            }
            //foreach (Bullet obj in _bullets)
            //{
                //obj.Update();
                //if (obj.Collision(_bullets)) 
                //{
                    //System.Media.SystemSounds.Hand.Play(); 
                    //obj.Pos.Y = rnd.Next(0, Height);
                    //obj.Pos.X = rnd.Next(0, Width);
                //}
            //}
            _bullet.Update();
        }
        private static void Timer_Tick(object sender, EventArgs e)
        {
            Draw();
            Update();
        }
    }
    interface ICollision
    {
        bool Collision(ICollision obj);
        Rectangle Rect { get; }
    }
    abstract class BaseObject: ICollision
    {
        public Point Pos;
        protected Point Dir;
        protected Size Size;
        protected Random _rnd = new Random();

        protected BaseObject(Point pos, Point dir, Size size)
        {
            Pos = pos;
            Dir = dir;
            Size = size;
        }
        public abstract void Draw();        
        public abstract void Update();        
        // Так как переданный объект тоже должен будет реализовывать интерфейс ICollision, мы 
        // можем использовать его свойство Rect и метод IntersectsWith для обнаружения пересечения с
        // нашим объектом (а можно наоборот)
        public bool Collision(ICollision o) => o.Rect.IntersectsWith(this.Rect);
        public Rectangle Rect => new Rectangle(Pos, Size);
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
        List<Image> imageList = new List<Image>() { Image.FromFile(@"images/star1.ico"), 
                                                    Image.FromFile(@"images/star2.ico"), 
                                                    Image.FromFile(@"images/star3.ico") };
        //Image.FromFile(fileList[_rnd.Next(fileList.Count)])
        public ImageStar(Point pos, Point dir, Size size):base(pos,dir,size)
        {            
        } 
        public override void Draw()
        {                                    
            Game.Buffer.Graphics.DrawImage(imageList[_rnd.Next(0,3)], Pos.X, Pos.Y, 8,8);
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
    // Создаем класс Asteroid, так как мы теперь не можем создавать объекты абстрактного класса BaseObject
    class Asteroid: BaseObject, ICloneable
    {
        public int Power {get;set;}
        public Asteroid(Point pos, Point dir, Size size) : base(pos, dir, size)
        {
            Power=1;
        }
        public override void Draw()
        {
            Game.Buffer.Graphics.FillEllipse(Brushes.White, Pos.X,Pos.Y, Size.Width,Size.Height);
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
        public object Clone()
        {
            // Создаем копию нашего робота
            Asteroid asteroid = new Asteroid(new Point(Pos.X, Pos.Y), new Point(Dir.X, Dir.Y), new Size(Size.Width, Size.Height));
            // Не забываем скопировать новому астероиду Power нашего астероида
            // asteroid.Power = Power;
            return asteroid;
        }
    }
    class Bullet : BaseObject
    {        
        public Bullet(Point pos, Point dir, Size size) : base(pos, dir, size)
        {            
        }
        public override void Draw()
        {
            Game.Buffer.Graphics.DrawRectangle(Pens.OrangeRed, Pos.X, Pos.Y, Size.Width, Size.Height);
        }
        public override void Update()
        {            
            Pos.X = Pos.X + Dir.X;
            if (Pos.X > Game.Width || Pos.X < 0) Dir.X = -Dir.X;
        }        
    }
    class Program
    {
        static void Main(string[] args)
        {            
            Form form = new Form();
            form.Width = 1200;
            form.Height = 750;              
            Game.Init(form);
            form.Show();
            Game.Draw();
            Application.Run(form);
        }
    }
}