using System;
using System.Linq;
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
        private static int _startCount = 25;
        private static List<Bullet> _bullets = new List<Bullet>();
        private static List<Asteroid> _asteroids = new List<Asteroid>();
        private static Aidkit[] _aidkits;
        private static Ship _ship;
        private static Image backgroundImage = Image.FromFile(@"images/background.jpg");
        private static Image aidkitImage = Image.FromFile(@"images/aidkit.png");
        private static Image bulletImage = Image.FromFile(@"images/bullet.png");
        private static List<Image> planetimageList = new List<Image>() { Image.FromFile(@"images/planet1.png"), 
                                                                         Image.FromFile(@"images/planet2.png"), 
                                                                         Image.FromFile(@"images/planet3.png") };
        private static Random rnd  = new Random();
        private static Timer _timer = new Timer {Interval = 100};
        // Свойства
        // Ширина и высота игрового поля
        public static int Width { get; set; }
        public static int Height { get; set; }
        public static int Level { get; set; }
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
            Level = 1;
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
            foreach (Bullet obj in _bullets)
                 obj.Draw();
            _ship?.Draw();
            if (_ship != null)
                Buffer.Graphics.DrawString("Energy:" + _ship.Energy, SystemFonts.DefaultFont, Brushes.White, 0, 0);
            Buffer.Graphics.DrawString("Score:" + _score, SystemFonts.DefaultFont, Brushes.White, 150, 0);
            Buffer.Graphics.DrawString("Level:" + Game.Level, SystemFonts.DefaultFont, Brushes.White, 300, 0);
            Buffer.Render();
        }      
        public static void Load()
        {
            _objs = new BaseObject[30];                                    
            _ship = new Ship(new Point(10, 400), new Point(8, 8), new Size(40, 70));
            _aidkits = new Aidkit[5];
            for (var i = 0; i < _objs.Length; i++)
            {
                int r = rnd.Next(5, 50);
                _objs[i] = new Star(new Point(600, rnd.Next(0, Game.Height)), new Point(-r, r), new Size(3, 3));                
            }            
            Generate_Asteroids();            
            for (var i = 0; i < _aidkits.Length; i++)
            {                
                _aidkits[i] = new Aidkit(aidkitImage, new Point(0, 0), new Point(rnd.Next(100, Game.Width), rnd.Next(0, Game.Height)), new Size(50, 45));
            }            
        }
        public static void Update()
        {
            if (_asteroids.Count == 0)            
            {
                Game.Level++;
                _bullets.Clear();
                System.GC.Collect();                
                Generate_Asteroids();
            }
            foreach (BaseObject obj in _objs)
                obj.Update();
            for (var i = 0; i < _asteroids.Count; i++)
            {                
                _asteroids[i].Update();
                if (_asteroids[i] != null && _ship.Collision(_asteroids[i]))
                {
                    _ship?.EnergyLow(rnd.Next(1, 10));
                    System.Media.SystemSounds.Asterisk.Play();
                    _asteroids.RemoveAt(i);                    
                    i--;
                    continue;
                }
                for (int j = 0; j < _bullets.Count; j++)
                    if (_asteroids[i] != null && _bullets[j].Collision(_asteroids[i]))
                    {
                        System.Media.SystemSounds.Hand.Play();
                        _asteroids.RemoveAt(i);
                        i--;
                        _bullets.RemoveAt(j);
                        j--;
                        _score += 1;    
                        break;
                    }
                if (_ship.Energy <= 0) _ship?.Die();
            }
            for (var i = 0; i < _aidkits.Length; i++)
            {
                if (_aidkits[i] == null) continue;
                _aidkits[i].Update();
                for (int j = 0; j < _bullets.Count; j++)                    
                    if (_aidkits[i] != null && _bullets[j].Collision(_aidkits[i]))
                    {
                        System.Media.SystemSounds.Hand.Play();
                        _aidkits[i] = null;
                        _bullets.RemoveAt(j);
                        j--;
                        _ship?.EnergyUp(rnd.Next(10, 15));                        
                    }
            }                        
            foreach (Bullet obj in _bullets)        
                obj.Update();              
        }
        private static void Timer_Tick(object sender, EventArgs e)
        {
            Draw();
            Update();
        }
        private static void Form_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ControlKey) 
            {
                _bullets.Add(new Bullet(bulletImage,
                                        new Point(_ship.Pos.X + _ship.shipImage.Width - bulletImage.Width/2, _ship.Pos.Y + _ship.shipImage.Height/2 - bulletImage.Height/2), 
                                        new Point(7+2*Game.Level, 0), 
                                        new Size(4, 1)));
            }
            if (e.KeyCode == Keys.Up) _ship.Up();
            if (e.KeyCode == Keys.Down) _ship.Down();
            if (e.KeyCode == Keys.Right) _ship.Right();
            if (e.KeyCode == Keys.Left) _ship.Left();
        }
        private static void Generate_Asteroids()
        {            
            for (var i = 0; i < _startCount+Game.Level*5; i++)
            {                
                Image im = planetimageList[rnd.Next(0,3)];
                _asteroids.Add(new Asteroid(im, new Point(rnd.Next(Game.Width/3, Game.Width), rnd.Next(0, Game.Height)), 
                                            new Point((rnd.Next(0,2)*2-1)*7+2*Game.Level, (rnd.Next(0,2)*2-1)*7+3*Game.Level), 
                                            new Size(im.Width, im.Height)));
            }
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
    // Создаем класс Asteroid, так как мы теперь не можем создавать объекты абстрактного класса BaseObject
    class Asteroid: BaseObject, ICloneable, IComparable
    {
        public int Power {get;set;} = 3;
        public Image asteroidImage;
        public Asteroid(Image img, Point pos, Point dir, Size size) : base(pos, dir, size)
        {
            Power = 1;
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
        public Image bulletImage;      
        public Bullet(Image img, Point pos, Point dir, Size size) : base(pos, dir, size)
        {    
            bulletImage = img;        
        }
        public override void Draw()
        {
            Game.Buffer.Graphics.DrawImage(bulletImage, Pos.X, Pos.Y, bulletImage.Width, bulletImage.Height);
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
            if (Pos.Y+shipImage.Height < Game.Height) Pos.Y = Pos.Y + Dir.Y;
        }
        public void Left()
        {
            if (Pos.X > 0) Pos.X = Pos.X - Dir.X;
        }
        public void Right()
        {
            if (Pos.X < Game.Width/5) Pos.X = Pos.X + Dir.X;
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