using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
// Создаем шаблон приложения, где подключаем модули
namespace MyGame
{
    public delegate void Message();
    static class Game
    {
        private static BufferedGraphicsContext _context;
        public static BufferedGraphics Buffer;
        private static int _score = 0;
        // private static Bullet[] _bullets;        
        private static Bullet _bullet;
        private static Asteroid[] _asteroids;
        private static ImageStar[] _stars;
        private static Aidkit[] _aidkits;
        private static Ship _ship;
        private static Image backgroundImage = Image.FromFile(@"images/background.jpg");
        private static Image aidkitImage = Image.FromFile(@"images/aidkit.png");
        private static List<Image> planetimageList = new List<Image>() { Image.FromFile(@"images/planet1.png"), 
                                                                         Image.FromFile(@"images/planet2.png"), 
                                                                         Image.FromFile(@"images/planet3.png") };
        private static Random rnd  = new Random();
        private static Timer _timer = new Timer {Interval = 100};
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
            _timer.Start();
            _timer.Tick += Timer_Tick;
            form.KeyDown += Form_KeyDown;
            Ship.MessageDie += Finish;

        }
        public static void Draw()
        {
            Buffer.Graphics.Clear(Color.Black);
            Buffer.Graphics.DrawImage(backgroundImage, 0, 0 , Width, Height);
            foreach (BaseObject obj in _objs)
                obj.Draw();
            foreach (Asteroid obj in _asteroids)
                obj?.Draw();
            foreach (Aidkit obj in _aidkits)
                obj?.Draw();
            foreach (ImageStar obj in _stars)
                obj.Draw();
            // foreach (Bullet obj in _bullets)
            //     obj.Draw();
            _bullet?.Draw();
            _ship?.Draw();
            if (_ship != null)
                Buffer.Graphics.DrawString("Energy:" + _ship.Energy, SystemFonts.DefaultFont, Brushes.White, 0, 0);
            Buffer.Graphics.DrawString("Score:" + _score, SystemFonts.DefaultFont, Brushes.White, 150, 0);    
            Buffer.Render();
        }      
        public static void Load()
        {
            _objs = new BaseObject[30];
            // _bullets = new Bullet[5];
            _bullet = new Bullet(new Point(0, Height/2), new Point(10, 0), new Size(12, 3));
            _asteroids = new Asteroid[50];
            _stars = new ImageStar[30];
            _ship = new Ship(new Point(10, 400), new Point(8, 8), new Size(40, 70));
            _aidkits = new Aidkit[5];
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
                _asteroids[i] = new Asteroid(planetimageList[rnd.Next(0,3)], new Point(600, rnd.Next(0, Game.Height)), new Point(-r / 5, r), new Size(r, r));
            }
            for (var i = 0; i < _aidkits.Length; i++)
            {                
                _aidkits[i] = new Aidkit(aidkitImage, new Point(0, 0), new Point(rnd.Next(100, Game.Width), rnd.Next(0, Game.Height)), new Size(50, 45));
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
            for (var i = 0; i < _asteroids.Length; i++)
            {
                if (_asteroids[i] == null) continue;
                _asteroids[i].Update();
                if (_bullet != null && _asteroids[i].Collision(_bullet))
                {
                    System.Media.SystemSounds.Hand.Play();
                    _asteroids[i] = null;
                    _bullet = null;
                    _score += 1;
                    continue;
                }
                if (!_ship.Collision(_asteroids[i])) continue;                
                _ship?.EnergyLow(rnd.Next(1, 10));
                System.Media.SystemSounds.Asterisk.Play();
                if (_ship.Energy <= 0) _ship?.Die();
                // if (obj.Collision(_bullet)) 
                // {
                //     System.Media.SystemSounds.Hand.Play(); 
                //     obj.Pos.Y = rnd.Next(0, Height);
                //     obj.Pos.X = rnd.Next(0, Width);
                // }
            }
            for (var i = 0; i < _aidkits.Length; i++)
            {
                if (_aidkits[i] == null) continue;
                _aidkits[i].Update();
                if (_bullet != null && _aidkits[i].Collision(_bullet))
                {
                    System.Media.SystemSounds.Hand.Play();
                    _aidkits[i] = null;
                    _bullet = null;
                    _ship?.EnergyUp(rnd.Next(10, 15));
                    continue;
                }                
            }
            foreach (ImageStar obj in _stars)
            {
                obj.Update();
                if (_bullet != null && obj.Collision(_bullet)) 
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
            _bullet?.Update();
        }
        private static void Timer_Tick(object sender, EventArgs e)
        {
            Draw();
            Update();
        }
        private static void Form_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ControlKey) _bullet = new Bullet(new Point(_ship.Pos.X + _ship.shipImage.Width, _ship.Pos.Y + _ship.shipImage.Height/2), 
                                                                   new Point(4, 0), 
                                                                   new Size(4, 1));
            if (e.KeyCode == Keys.Up) _ship.Up();
            if (e.KeyCode == Keys.Down) _ship.Down();
        }
        public static void Finish()
        {
            _timer.Stop();
            Buffer.Graphics.DrawString("The End", new Font(FontFamily.GenericSansSerif, 60, FontStyle.Underline), Brushes.White, 200, 100);
            Buffer.Render();
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
    class Asteroid: BaseObject, ICloneable, IComparable
    {
        public int Power {get;set;} = 3;
        public Image asteroidImage;
        public Asteroid(Image img, Point pos, Point dir, Size size) : base(pos, dir, size)
        {
            Power=1;
            asteroidImage = img;
        }
        public override void Draw()
        {
            Game.Buffer.Graphics.DrawImage(asteroidImage, Pos.X, Pos.Y, asteroidImage.Width, asteroidImage.Height);
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
            Asteroid asteroid = new Asteroid(asteroidImage, new Point(Pos.X, Pos.Y), new Point(Dir.X, Dir.Y), 
                                new Size(Size.Width, Size.Height)) {Power = Power};
            // Не забываем скопировать новому астероиду Power нашего астероида
            // asteroid.Power = Power;
            return asteroid;
        }                
        int IComparable.CompareTo(object obj)
        {
            if (obj is Asteroid temp)
            {
                if (Power > temp.Power)
                    return 1;
                if (Power < temp.Power)
                    return -1;
                else
                    return 0;
            }
            throw new ArgumentException("Parameter is not а Asteroid!");
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
    class Ship : BaseObject
    {
        private int _energy = 100;
        public int Energy => _energy;
        public Image shipImage = Image.FromFile(@"images/ship.png");
        public static event  Message MessageDie;

        public void EnergyLow(int n)
        {
            _energy -= n;
        }
        public void EnergyUp(int n)
        {
            _energy += n;
        }
        public Ship(Point pos, Point dir, Size size) : base(pos, dir, size)
        {
        }
        public override void Draw()
        {
            Game.Buffer.Graphics.DrawImage(shipImage, Pos.X, Pos.Y, shipImage.Width, shipImage.Height);
        }
        public override void Update()
        {
        }
        public void Up()
        {
            if (Pos.Y > 0) Pos.Y = Pos.Y - Dir.Y;
        }
        public void Down()
        {
            if (Pos.Y < Game.Height) Pos.Y = Pos.Y + Dir.Y;
        }
        public void Die()
        {
            MessageDie?.Invoke();
        }
    }
    class Aidkit: BaseObject
    {
        public Image aidkitImage;
        public int currentAngle;
        public Aidkit(Image img, Point pos, Point dir, Size size) : base(pos, dir, size)
        {            
            aidkitImage = img;
            currentAngle = 0;
            Pos.X = Dir.X + Convert.ToInt32(Size.Width*Math.Sin(currentAngle));
            Pos.Y = Dir.Y + Convert.ToInt32(Size.Width*Math.Sin(currentAngle));
        }
        public override void Draw()
        {
            Game.Buffer.Graphics.DrawImage(aidkitImage, Pos.X, Pos.Y, aidkitImage.Width, aidkitImage.Height);
        }
        public override void Update()
        {   
            currentAngle += Size.Height;         
            Pos.X = Dir.X + Convert.ToInt32(Size.Width*Math.Sin(currentAngle));
            Pos.Y = Dir.Y + Convert.ToInt32(Size.Width*Math.Sin(currentAngle));            
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