using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;
using System.IO;



namespace spaceinvaders3
{
    public partial class Form1 : Form
    {
        class Player
        {
            public PictureBox Pic;
            public int Lives = 3;
            public int Speed = 6;
            public bool poder = false;

        }

        class Enemy
        {
            public PictureBox Pic;
            public bool Alive = true;
        }

        class Bullet
        {
            public PictureBox Pic;
            public bool IsEnemy;
            public int Speed = 8;
            public bool Active = false;
        }

        class Shield
        {
            public PictureBox Pic;
            public int Health = 10;
        }

        class Orbe
        {
            public PictureBox Pic;
            public bool ativo = false;
            public bool heart = false;
        }
        class Power
        {
            public PictureBox Pic;
            public bool ativo = false;
        }


        public void CriaInimigos()
        {
            int index = 0;
            for (int y = 0; y < 3; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    enemies[index] = new Enemy();
                    enemies[index].Pic = new PictureBox()
                    {
                        Image = Image.FromFile(Asset("herobrine.png")),
                        Size = new Size(40, 30),
                        Location = new Point(50 + x * 70, 50 + y * 40),
                        SizeMode = PictureBoxSizeMode.StretchImage
                    };
                    this.Controls.Add(enemies[index].Pic);
                    index++;
                }
            }
        }


        Player player;
        Enemy[] enemies = new Enemy[24];
        Bullet[] bullets = new Bullet[50];
        Bullet[] bullets2 = new Bullet[2];
        Shield[] shields = new Shield[3];
        PictureBox[] Vida = new PictureBox[3];
        Orbe[] orbe = new Orbe[5];
        Power habili;

        int vd = 20;

        bool gameRunning = false;
        bool paused = false;
        int level = 1;
        bool Rec = false;

        bool keyA = false;
        bool keyD = false;
        bool keySpace = false;
        bool keyB = false;
        bool Shoote = true;

        Thread threadPlayer, threadEnemies, threadGame;
        Random rnd = new Random();

        // Caminho absoluto para a pasta Assets, ao lado do .exe (independe do diretório de trabalho)
        static string Asset(string arquivo) =>
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", arquivo);

        Image imgEnemyBullet = Image.FromFile(Asset("tiro_ghast.png"));
        Image imgPlayerBullet = Image.FromFile(Asset("flecha.png"));
        SoundPlayer musica = File.Exists(Asset("C418-Moog-City.wav"))
            ? new SoundPlayer(Asset("C418-Moog-City.wav"))
            : null; // a música é opcional: sem o arquivo, o jogo roda sem som
        
        
       


        public Form1()
        {
            InitializeComponent();
            this.BackColor = Color.Black;
            this.Width = 800;
            this.Height = 600;
            this.KeyPreview = true;
            musica?.PlayLooping();
            StartGame();
        }

        // Método auxiliar para chamadas seguras de UI
        void SafeInvoke(Action action)
        {
            if (this.InvokeRequired)
                this.Invoke(action);
            else
                action();
        }

        void StartGame()
        {
            gameRunning = true;
            // --- Cria Player ---
            player = new Player();
            player.Pic = new PictureBox()
            {
                Image = Image.FromFile(Asset("steve.png")),
                Size = new Size(40, 30),
                Location = new Point(350, 500),
                SizeMode = PictureBoxSizeMode.StretchImage
            };
            this.Controls.Add(player.Pic);


            // --- Cria Vidas ---
            for (int i = 0; i < 3; i++)
            {
                Vida[i] = new PictureBox()
                {
                    Image = Image.FromFile(Asset("coracao.png")),
                    Size = new Size(25, 25),
                    Location = new Point(vd, 530),
                    SizeMode = PictureBoxSizeMode.StretchImage
                };
                this.Controls.Add(Vida[i]);
                vd += 25;
            }


            // --- Cria Inimigos ---

            CriaInimigos();

            // --- Cria Escudos ---
            for (int i = 0; i < shields.Length; i++)
            {
                shields[i] = new Shield();
                shields[i].Pic = new PictureBox()
                {
                    Image = Image.FromFile(Asset("escudo10.png")),
                    Size = new Size(60, 25),
                    Location = new Point(150 + i * 200, 420),
                    SizeMode = PictureBoxSizeMode.StretchImage
                };
                this.Controls.Add(shields[i].Pic);
            }

            // --- Cria balas ---
            for (int i = 0; i < bullets.Length; i++)
            {
                bullets[i] = new Bullet();
                bullets[i].Pic = new PictureBox()
                {
                    Size = new Size(4, 10),
                    Visible = false,
                    SizeMode = PictureBoxSizeMode.StretchImage
                };
                this.Controls.Add(bullets[i].Pic);
            }

            for (int i = 0; i < bullets2.Length; i++)
            {
                bullets2[i] = new Bullet();
                bullets2[i].Pic = new PictureBox()
                {
                    Size = new Size(4, 10),
                    Visible = false,
                    SizeMode = PictureBoxSizeMode.StretchImage
                };
                this.Controls.Add(bullets2[i].Pic);
            }

            // --- Cria orbe ---
            for (int i = 0; i < orbe.Length; i++)
            {
                orbe[i] = new Orbe();
                orbe[i].Pic = new PictureBox();
                orbe[i].Pic.Visible = false;

                this.Controls.Add(orbe[i].Pic);
            }

            //--- Cria habilidade ---
            habili = new Power();
            habili.Pic = new PictureBox()
            {
                Image = Image.FromFile(Asset("coracao.png")),
                Size = new Size(25, 25),
                Visible = false,
                SizeMode = PictureBoxSizeMode.StretchImage
            };
            this.Controls.Add(habili.Pic);

            //--- Label nivel ---
            lblNivel.Font = new Font("Press Start 2P", 14, FontStyle.Regular);
            lblNivel.ForeColor = Color.White;
            lblNivel.Text = $"Nivel: {level}, Especial: {(player.poder ? "Sim (Aperte B)" : "Não")}";
            lblNivel.AutoSize = true;


            // --- Threads ---
            threadPlayer = new Thread(PlayerLoop);
            threadEnemies = new Thread(EnemyLoop);
            threadGame = new Thread(GameLoop);

            threadPlayer.Start();
            threadEnemies.Start();
            threadGame.Start();

        }

        void PlayerLoop()
        {
            while (gameRunning)
            {
                if (!paused)
                {
                    SafeInvoke(() =>
                    {
                        if (keyA)
                            player.Pic.Left = Math.Max(0, player.Pic.Left - player.Speed);

                        if (keyD)
                            player.Pic.Left = Math.Min(this.Width - player.Pic.Width, player.Pic.Left + player.Speed);

                        if (keySpace && Shoote && !Rec)
                        {
                            Shoot(player.Pic.Left + 18, player.Pic.Top - 10, false);
                            Shoote = false;
                            Task.Run(async () =>
                            {
                                await Task.Delay(500);
                                Shoote = true;
                            });
                        }

                        if (keyB && player.poder)
                        {
                            shotHabili(player.Pic.Left + 18, player.Pic.Top - 10);
                            lblNivel.Text = $"Nivel: {level}, Especial: {(player.poder ? "Sim (Aperte B)" : "Não")}";

                        }



                    });
                }
                Thread.Sleep(30);
            }
        }

        void EnemyLoop()
        {
            int dir = 1;
            while (gameRunning)
            {
                if (!paused)
                {
                    for (int i = 0; i < enemies.Length; i++)
                    {
                        if (enemies[i] != null && enemies[i].Alive)
                        {
                            SafeInvoke(() =>
                            {
                                if (level < 3)
                                {
                                    enemies[i].Pic.Left += dir * (6 + level);

                                    // Troca direção se bater borda
                                    if (enemies[i].Pic.Left < 20 || enemies[i].Pic.Right > this.Width - 40)
                                        dir *= -1;
                                }
                                else
                                {
                                    enemies[i].Pic.Left += dir * (4 + level);

                                    // Troca direção se bater borda
                                    if (enemies[i].Pic.Left < 20 || enemies[i].Pic.Right > this.Width - 40)
                                    {
                                        dir *= -1;
                                        for (int j = 0; j < enemies.Length; j++)
                                        {
                                            enemies[j].Pic.Top += 10 + (level * 2);
                                            if (enemies[j].Pic.Bottom >= 500)
                                            {
                                                player.Lives = 0;
                                            }
                                        }
                                    }
                                }
                            });

                            // Chance de atirar
                            if (rnd.Next(0, 123) < 3 * (1 + level / 2))
                                Shoot(enemies[i].Pic.Left + 18, enemies[i].Pic.Bottom, true);
                        }
                    }
                }
                if (Rec)
                {
                    Thread.Sleep(2000);
                    SafeInvoke(() => CriaInimigos());
                    Rec = false;
                }
                Thread.Sleep(380);
            }
        }

        void GameLoop()
        {
            while (gameRunning)
            {
                if (!paused)
                {
                    MoveBullets();
                    CheckCollisions();
                    CheckGameOver();

                    poder();
                    DescePoder();
                    MovePoder();


                }
                Thread.Sleep(40);
            }
        }

        void Shoot(int x, int y, bool enemy)
        {
            for (int i = 0; i < bullets.Length; i++)
            {
                if (!bullets[i].Active)
                {
                    bullets[i].Active = true;
                    bullets[i].IsEnemy = enemy;
                    bullets[i].Speed = enemy ? 5 + level : 7 + level;

                    SafeInvoke(() =>
                    {
                        bullets[i].Pic.Visible = true;
                        bullets[i].Pic.Location = new Point(x, y);
                        if (enemy)
                        {
                            bullets[i].Pic.Size = new Size(16, 11);
                            bullets[i].Pic.Image = imgEnemyBullet;
                        }
                        else
                        {
                            bullets[i].Pic.Size = new Size(8, 24);
                            bullets[i].Pic.Image = imgPlayerBullet;
                            Task.Delay(3000);

                        }
                    });

                    break;
                }
            }
        }

        void MoveBullets()
        {
            for (int i = 0; i < bullets.Length; i++)
            {
                if (!bullets[i].Active) continue;

                SafeInvoke(() =>
                {
                    if (bullets[i].IsEnemy)
                        bullets[i].Pic.Top += bullets[i].Speed;
                    else
                        bullets[i].Pic.Top -= bullets[i].Speed;

                    if (bullets[i].Pic.Top < 0 || bullets[i].Pic.Bottom > this.Height)
                    {
                        bullets[i].Active = false;
                        bullets[i].Pic.Visible = false;
                    }
                });
            }

            for (int i = 0; i < bullets2.Length; i++)
            {
                if (!bullets2[i].Active) continue;

                SafeInvoke(() =>
                {
                    if (i == 0)
                        bullets2[i].Pic.Left -= bullets2[i].Speed;
                    else
                        bullets2[i].Pic.Left += bullets2[i].Speed;

                    if (bullets2[i].Pic.Left < 0 || bullets2[i].Pic.Left > this.Width)
                    {
                        bullets2[i].Active = false;
                        bullets2[i].Pic.Visible = false;


                    }
                });
            }

        }
        void TryRecreate()
        {
            foreach (var e in enemies)
            {
                if (e.Alive)
                {
                    return;
                }
            }
            Rec = true;
            level++;
            lblNivel.Text = $"Nivel: {level}, Especial: {(player.poder ? "Sim (Aperte B)" : "Não")}";
        }
        void CheckCollisions()
        {
            foreach (var b in bullets)
            {
                if (!b.Active) continue;

                SafeInvoke(() =>
                {
                    if (b.IsEnemy)
                    {
                        // Acertou o jogador
                        if (b.Pic.Bounds.IntersectsWith(player.Pic.Bounds))
                        {
                            player.Lives--;
                            this.Controls.Remove(Vida[player.Lives]);
                            vd -= 25;
                            b.Active = false;
                            b.Pic.Visible = false;
                        }

                        // Escudo
                        foreach (var s in shields)
                        {
                            if (s != null && b.Pic.Bounds.IntersectsWith(s.Pic.Bounds) && s.Health > 0)
                            {
                                s.Health--;
                                s.Pic.Image = Image.FromFile(Asset($"escudo{s.Health}.png"));
                                if (s.Health <= 0)
                                {
                                    this.Controls.Remove(s.Pic);
                                }
                                b.Active = false;
                                b.Pic.Visible = false;
                                break;
                            }
                        }
                    }
                    else
                    {
                        // Bala do jogador atinge inimigo
                        foreach (var e in enemies)
                        {
                            if (e != null && e.Alive && b.Pic.Bounds.IntersectsWith(e.Pic.Bounds))
                            {
                                e.Alive = false;
                                this.Controls.Remove(e.Pic);
                                b.Active = false;
                                b.Pic.Visible = false;
                                TryRecreate();
                                break;
                            }
                        }
                        foreach (var s in shields)
                        {
                            if (s != null && b.Pic.Bounds.IntersectsWith(s.Pic.Bounds) && s.Health > 0)
                            {
                                b.Active = false;
                                b.Pic.Visible = false;
                                break;
                            }
                        }
                    }
                });
            }

            foreach (var e in orbe)
            {
                if (e.ativo)
                {
                    SafeInvoke(() =>
                    {
                        // Acertou o jogador
                        if (e.Pic.Bounds.IntersectsWith(player.Pic.Bounds))
                        {
                            if (!e.heart)
                            {
                                player.poder = true;
                                e.ativo = false;
                                e.Pic.Visible = false;

                                lblNivel.Text = $"Nivel: {level}, Especial: {(player.poder ? "Sim (Aperte B)" : "Não")}";
                            }
                            else
                            {
                                e.heart = false;
                                e.ativo = false;
                                e.Pic.Visible = false;
                                if (player.Lives < 3)
                                {

                                    Vida[player.Lives] = new PictureBox()
                                    {
                                        Image = Image.FromFile(Asset("coracao.png")),
                                        Size = new Size(25, 25),
                                        Location = new Point(vd, 530),
                                        SizeMode = PictureBoxSizeMode.StretchImage
                                    };
                                    this.Controls.Add(Vida[player.Lives]);
                                    vd += 25;
                                    player.Lives++;
                                }
                            }
                        }


                    });
                }
            }

            if (habili.ativo)
            {
                SafeInvoke(() =>
                {
                    foreach (var e in enemies)
                    {
                        if (e != null && e.Alive && habili.Pic.Bounds.IntersectsWith(e.Pic.Bounds))
                        {
                            int x = e.Pic.Bounds.X;
                            int y = e.Pic.Bounds.Y;
                            e.Alive = false;
                            this.Controls.Remove(e.Pic);
                            habili.ativo = false;
                            habili.Pic.Visible = false;

                            for (int i = 0; i < bullets2.Length; i++)
                            {
                                if (!bullets2[i].Active)
                                {
                                    bullets2[i].Active = true;
                                    bullets2[i].Speed = 7 + level;

                                    SafeInvoke(() =>
                                    {
                                        bullets2[i].Pic.Visible = true;
                                        bullets2[i].Pic.Location = new Point(x, y + 15);
                                        bullets2[i].Pic.Size = new Size(24, 8);

                                        if (i == 0)
                                            bullets2[i].Pic.Image = Image.FromFile(Asset("flecha_e.png"));
                                        else
                                            bullets2[i].Pic.Image = Image.FromFile(Asset("flecha_d.png"));
                                    });
                                }
                            }

                            TryRecreate();
                            break;
                        }
                    }

                    foreach (var s in shields)
                    {
                        if (s != null && habili.Pic.Bounds.IntersectsWith(s.Pic.Bounds) && s.Health > 0)
                        {
                            if (s.Health < 10)
                            {
                                s.Health++;
                                s.Pic.Image = Image.FromFile(Asset($"escudo{s.Health}.png"));
                            }

                            habili.ativo = false;
                            habili.Pic.Visible = false;
                            break;
                        }
                    }
                });
            }


            foreach (var b in bullets2)
            {
                if (!b.Active) continue;

                SafeInvoke(() =>
                {

                    foreach (var e in enemies)
                    {
                        if (e != null && e.Alive && b.Pic.Bounds.IntersectsWith(e.Pic.Bounds))
                        {
                            e.Alive = false;
                            this.Controls.Remove(e.Pic);
                            b.Active = false;
                            b.Pic.Visible = false;
                            TryRecreate();
                            break;
                        }
                    }


                });
            }

        }
        // poder
        void poder()
        {
            if (rnd.Next(0, 360) < 1.1 + (level * 1.4))
            {
                for (int i = 0; i < orbe.Length; i++)
                {
                    if (!orbe[i].ativo)
                    {
                        orbe[i].ativo = true;

                        SafeInvoke(() =>
                        {
                            orbe[i].Pic.Visible = true;
                            orbe[i].Pic.Location = new Point(rnd.Next(20, this.Width), 20);
                            orbe[i].Pic.Size = new Size(25, 25);
                            int temp = rnd.Next(0, 4);
                            if (temp > 1)
                            {
                                orbe[i].Pic.Image = Image.FromFile(Asset("coracao.png"));
                                orbe[i].heart = true;
                            }
                            else
                            {
                                orbe[i].Pic.Image = Image.FromFile(Asset("totem.png"));
                                orbe[i].heart = false;
                            }

                            //orbe[i].Pic.Image = Image.FromFile(Asset("coracao.png"));
                            orbe[i].Pic.SizeMode = PictureBoxSizeMode.StretchImage;


                        });

                        break;
                    }
                }

            }
        }
        void DescePoder()
        {
            for (int i = 0; i < orbe.Length; i++)
            {
                if (orbe[i].ativo)
                {
                    SafeInvoke(() =>
                    {

                        orbe[i].Pic.Top += 6 + level;

                        if (orbe[i].Pic.Bottom > this.Height)
                        {
                            orbe[i].ativo = false;
                            orbe[i].Pic.Visible = false;
                        }
                    });
                }
            }
        }

        void shotHabili(int x, int y)
        {
            player.poder = false;

            SafeInvoke(() =>
            {
                habili.Pic.Visible = true;
                habili.Pic.Location = new Point(x - 10, y);
                habili.Pic.Size = new Size(25, 25);
                habili.Pic.Image = Image.FromFile(Asset("endcristal.png"));
                habili.ativo = true;
            });
        }

        void MovePoder()
        {
            if (habili.ativo)
            {

                SafeInvoke(() =>
                {

                    habili.Pic.Top -= 12 + level;

                    if (habili.Pic.Top < 0)
                    {
                        habili.Pic.Visible = false;
                        habili.ativo = false;

                    }
                });
            }

        }


        void CheckGameOver()
        {
            if (player.Lives <= 0)
            {
                gameRunning = false;
                SafeInvoke(() =>
                {
                    MessageBox.Show("Game Over!");
                    Application.Restart();
                });
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                paused = !paused;
                return;
            }

            if (!gameRunning || paused) return;

            if (e.KeyCode == Keys.A)
                keyA = true;

            if (e.KeyCode == Keys.D)
                keyD = true;

            if (e.KeyCode == Keys.Space)
                keySpace = true;

            if (e.KeyCode == Keys.B)
                keyB = true;
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A)
                keyA = false;

            if (e.KeyCode == Keys.D)
                keyD = false;

            if (e.KeyCode == Keys.Space)
                keySpace = false;

            if (e.KeyCode == Keys.B)
                keyB = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            musica?.Stop();
            gameRunning = false;
            threadPlayer.Abort();
            threadEnemies.Abort();
            threadGame.Abort();

            base.OnFormClosing(e);
        }


    }
}