using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace 엉덩국
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        KeyboardState keyboardState;
        KeyboardState previousKeyboardState;

        //배경
        Rectangle viewportRect;
        Texture2D backgroundTexture;
        //게임오버
        Texture2D gameOver;
        //엔딩
        Texture2D end;

        //주인공
        GameObject hero,state;
        bool nomal = true;
        GameObject mp, hp;
        Texture2D gage;

        GameObject hero_bat;//야구방망이
        bool bat = false; //야방
        bool attack_delay = false;
        // 무기
        GameObject hero_gun;
        GameObject[] cannonBalls;
        bool gun = false;//새총

        //프롤로그
        Texture2D prolog, prolog2 , prolog3;
        int prolnum = -1; //프롤로그 컷수
        bool isProl = false; //프롤로그 여부

        //메뉴
        Texture2D menu;
        bool isMenu = true;
        int menunum = 0;

        //핑크 (적)
        bool Pinkflip = true; //핑크 좌우 플립
        bool PinkAt1 = false; //핑크 기술1
        bool PinkAt2 = false; //핑크 기술2
        bool PinkAt3 = false; //핑크 기술3
        bool PinkHT = false; //핑크 아퍼
        bool PinkAttack = false; //핑크 공격상태
        bool PinkOnWalking = true; //핑크 걷기상태
        int hitnum = 0;

        //점프변수
        bool isGround = true;
        bool isJumping = false;
        bool isDirUp = false;
        bool isInAir = false;//점프를 위한 bool

        bool isRight = false;

        int maxHeight;
        int currentHeight;//점프시 높이

        GameObject Pink;

        float delay = 0.5f;//공격딜레이
        const int maxCannonBalls = 3;       // 최대 무기 발사 수.
        float effect_T = 0.2f;//모션 나오는 시간

        float extime;
        float CurrentTime;//무기공격 이펙트 시간 조절

        int aCount = 1;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            // 화면 크기
            graphics.PreferredBackBufferHeight = 600;
            graphics.PreferredBackBufferWidth = 1000;
            // 화면 풀스크린 여부
            this.graphics.IsFullScreen = false;
        }
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            base.Initialize();
            Window.Title = "학고행 하이테크";
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            backgroundTexture = Content.Load<Texture2D>("Sprites\\back");       // 배경 생성
            viewportRect = new Rectangle(0, 0, 1000, 600);                      // 배경 범위,위치

            gameOver = Content.Load<Texture2D>("Sprites\\gameover");            // 게임오버생성
            end = Content.Load<Texture2D>("Sprites\\end");                      // 엔딩생성

            state = new GameObject(Content.Load<Texture2D>("Sprites\\hero"));   //현재상태
            state.position = new Vector2(50, 300);

            hero = new GameObject(Content.Load<Texture2D>("Sprites\\hero"));    // 주인공 생성
            hero.position = new Vector2(50, 300);                                // 주인공 위치

            gage = Content.Load<Texture2D>("Sprites\\gage");

            hp = new GameObject(Content.Load<Texture2D>("Sprites\\hp"));
            hp.position = new Vector2(0.1f, 0.1f);                                         //생성위치
            hp.framecount = 50;

            mp = new GameObject(Content.Load<Texture2D>("Sprites\\mp"));        //마력
            mp.position = new Vector2(0.1f, 0.1f);                                         //생성위치
            mp.framecount = 50;

            hero_bat = new GameObject(Content.Load<Texture2D>("Sprites\\hero bat"));//방망이든 주인공 생성
            hero_bat.position = new Vector2(50, 300);//방망이 위치

            hero_gun = new GameObject(Content.Load<Texture2D>("Sprites\\hero gun"));//새총든 주인공 생성
            hero_gun.position = new Vector2(50, 300);//새총 위치

            Pink = new GameObject(Content.Load<Texture2D>("Sprites\\PinkActingTest"));      //핑크생성
            Pink.position = new Vector2(100, 300);

            prolog = Content.Load<Texture2D>("Sprites\\prolog");                //프롤로그 생성
            prolog2 = Content.Load<Texture2D>("Sprites\\prolog2");              //프롤로그 생성
            prolog3 = Content.Load<Texture2D>("Sprites\\prolog3");

            menu = Content.Load<Texture2D>("Sprites\\menu");                    //메뉴 생성

            cannonBalls = new GameObject[maxCannonBalls];
            for (int p = 0; p < maxCannonBalls; p++)
            {
                cannonBalls[p] = new GameObject(Content.Load<Texture2D>("Sprites\\cannonball"));
            }
            //포탄

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // 각 위치를 히어로와 위치와 일치시킨다.
            state.position = hero.position;
            hero_bat.position = hero.position;
            hero_gun.position = hero.position;
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            keyboardState = Keyboard.GetState();
            if (isMenu == true)     // 메뉴 화면에서
            {
                if (keyboardState.IsKeyDown(Keys.Down) && previousKeyboardState.IsKeyUp(Keys.Down))     //메뉴 이동
                {
                    if (menunum < 3)
                        menunum++;
                    else
                        menunum = 3;
                }
                else if (keyboardState.IsKeyDown(Keys.Up) && previousKeyboardState.IsKeyUp(Keys.Up))
                {
                    if (menunum >= 1)
                        menunum--;
                    else
                        menunum = 0;

                }
                // 메뉴 선택
                else if (keyboardState.IsKeyDown(Keys.Enter) && previousKeyboardState.IsKeyUp(Keys.Enter))
                {   //메뉴 번호에 따라서 이벤트
                    switch (menunum)
                    {
                        case 0:     //메뉴 선택 안함
                            break;
                        case 1:     //프롤로그 시작
                            isProl = true;
                            isMenu = false;
                            break;
                        case 2:         //게임 방법 화면
                            menunum = 4;
                            break;
                        case 3: //종료
                            Exit();
                            break;
                        default:
                            break;
                    }
                }
            }

            if (isProl == true)     //프롤로그 중
            {   //엔터를 누르면 다음 화면으로
                if (keyboardState.IsKeyDown(Keys.Enter) && previousKeyboardState.IsKeyUp(Keys.Enter))
                {
                    prolnum++;
                }
            }

            if (menunum == 4)       // 게임 방법 화면
            {
                if (keyboardState.IsKeyDown(Keys.Escape))
                {
                    menunum--;          //화면 나가기
                }
            }

            if (hp.framecount < 10 || hitnum > 30)
            {           // 캐릭터 사망
                if (keyboardState.IsKeyDown(Keys.Enter))
                {
                    Exit();
                }

            }

            //캐릭터움직임
            HeroMove(gameTime);

            if (isProl == false)        //프롤로그가 끝나면 적 캐릭터 움직임
            {
                PinkMove();
            }
            if (hero.position.X - Pink.position.X == -300 || hero.position.X - Pink.position.X == 300)      // 적 캐릭터와 내 캐릭터 위치 차이 300이면 적캐릭터 원거리 공격
            {
                PinkAttack1();
            }
            else if (hero.position.X - Pink.position.X == -50 || hero.position.X - Pink.position.X == 50)   // 가까우면 근접 공격
            {
                PinkAttack2();
            }
            else
            {
                PinkOnWalking = true;       // 적캐릭터 움직임
            }

            previousKeyboardState = keyboardState;

            base.Update(gameTime);
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            //배경그림
            spriteBatch.Draw(backgroundTexture, viewportRect, Color.White);

            //메뉴 그림
            if (isMenu == true)
            {
                spriteBatch.Draw(menu, new Vector2(0, 0), new Rectangle(0, (int)(menunum) * 600, 1000, 600), Color.White);
            }


            //프롤로그 그림
            if (isProl == true)
            {   // 모든 프롤로그를 모아 두고, 보여주는 화면을 이동함
                if (prolnum < 6)
                {
                    spriteBatch.Draw(prolog, new Vector2(0, 0), new Rectangle(0, (int)(prolnum) * 600, 1000, 600), Color.White);
                }
                else if (prolnum < 11 && prolnum >= 6)
                {
                    spriteBatch.Draw(prolog2, new Vector2(0, 0), new Rectangle(0, ((int)(prolnum) - 6) * 600, 1000, 600), Color.White);
                }
                else if (prolnum >= 11 && prolnum < 12)
                {
                    spriteBatch.Draw(prolog3, new Vector2(0, 0), new Rectangle(0, ((int)(prolnum)-11) * 600, 1000, 600), Color.White);

                }
                else
                    isProl = false;

            }
            //프롤로그후 주인공 핑크그림
            if (isProl == false && isMenu ==false)
            {
                spriteBatch.Draw(hero.sprite, hero.position, new Rectangle((int)(hero.framecount / 10) * 300, 0, 300, 280), Color.White, hero.delay, hero.center, 1.0f,
                   SpriteEffects.None, 0);
                if (PinkAttack)     //적 공격
                {
                    if (!Pinkflip)
                    {
                        if (PinkAt3)
                        {
                            spriteBatch.Draw(Pink.sprite, Pink.position, new Rectangle((int)(Pink.framecount / 20) * 300, 750, 300, 250), Color.White, Pink.delay, Pink.center, 1.0f,
                            SpriteEffects.None, 0);
                        }

                        if (PinkAt2)
                        {
                            spriteBatch.Draw(Pink.sprite, Pink.position, new Rectangle((int)(Pink.framecount / 20) * 300, 500, 300, 250), Color.White, Pink.delay, Pink.center, 1.0f,
                            SpriteEffects.None, 0);
                        }
                        if (PinkAt1)
                        {
                            spriteBatch.Draw(Pink.sprite, Pink.position, new Rectangle((int)(Pink.framecount / 20) * 300, 250, 300, 250), Color.White, Pink.delay, Pink.center, 1.0f,
                            SpriteEffects.None, 0);
                        }
                    }
                    else
                    {
                        if (PinkAt3)
                        {
                            spriteBatch.Draw(Pink.sprite, Pink.position, new Rectangle((int)(Pink.framecount / 20) * 300, 750, 300, 250), Color.White, Pink.delay, Pink.center, 1.0f,
                            SpriteEffects.FlipHorizontally, 0);
                        }

                        if (PinkAt2)
                        {
                            spriteBatch.Draw(Pink.sprite, Pink.position, new Rectangle((int)(Pink.framecount / 20) * 300, 500, 300, 250), Color.White, Pink.delay, Pink.center, 1.0f,
                            SpriteEffects.FlipHorizontally, 0);
                        }
                        if (PinkAt1)
                        {
                            spriteBatch.Draw(Pink.sprite, Pink.position, new Rectangle((int)(Pink.framecount / 20) * 300, 250, 300, 250), Color.White, Pink.delay, Pink.center, 1.0f,
                            SpriteEffects.FlipHorizontally, 0);
                        }
                    }
                }
                else
                {
                    if (!Pinkflip)
                    {
                        spriteBatch.Draw(Pink.sprite, Pink.position, new Rectangle((int)(Pink.framecount / 10) * 300, 0, 300, 250), Color.White, Pink.delay, Pink.center, 1.0f,
                        SpriteEffects.None, 0);
                    }
                    else
                    {
                        spriteBatch.Draw(Pink.sprite, Pink.position, new Rectangle((int)(Pink.framecount / 10) * 300 + 900, 0, 300, 250), Color.White, Pink.delay, Pink.center, 1.0f,
                        SpriteEffects.None, 0);
                        //SpriteEffects.FlipHorizontally를 써도됨                                  그러면 +900 할필요X
                    }
                }
            }

            //체력, 마력바
            if (isProl == false && isMenu == false)
            {
                spriteBatch.Draw(gage, new Vector2(0.1f, 0.1f), Color.White);
                spriteBatch.Draw(hp.sprite, hp.position, new Rectangle(0, 0, hp.framecount * 7, 86), Color.White);
                spriteBatch.Draw(mp.sprite, mp.position, new Rectangle(0, 0, mp.framecount * 7, 86), Color.White);
            }

            foreach (GameObject ball in cannonBalls)
            {
                if (ball.alive)
                {
                    spriteBatch.Draw(ball.sprite,
                        ball.position, Color.White);
                }
            }

            //엔딩
            if (hitnum > 30)
            {
                spriteBatch.Draw(end, viewportRect, Color.White);
            }

            //겜오버
            if (hp.framecount < 15)
            {
                spriteBatch.Draw(gameOver, viewportRect, Color.White);
            }




            spriteBatch.End();

            base.Draw(gameTime);
        }



        /// <summary>
        /// 위에서 쓰일 함수 들어갑니다!!!
        /// </summary>

        public void PinkMove()
        {
            if (PinkOnWalking)
            {
                if (Pink.position.X < hero.position.X) //Pink X축 쫄쫄이
                {
                    PinkAttack = false;     //공격
                    Pink.framecount++;
                    PinkAt1 = false;
                    PinkAt2 = false;
                    PinkAt3 = false;
                    Pinkflip = true;
                    Pink.position.X += 2.0f;        // 이동 속도
                    if (Pink.framecount >= 30)
                    {
                        Pink.framecount = 0;
                    }
                }
                else if (Pink.position.X > hero.position.X)     // 좌우 이동 반대로
                {
                    PinkAttack = false;
                    Pink.framecount++;
                    PinkAt1 = false;
                    PinkAt2 = false;
                    PinkAt3 = false;
                    Pinkflip = false;
                    Pink.position.X -= 2.0f;
                    if (Pink.framecount >= 30)
                    {
                        Pink.framecount = 0;
                    }
                }
                else if (Pink.position.X == hero.position.X)        //공격
                {
                    //Pink.framecount = 0;
                    PinkAttack = true;
                    PinkAt1 = false;
                    PinkAt2 = false;
                    PinkAt3 = true;
                    Pink.framecount++;
                    PinkAttack3();

                }
                else { } //이렇게 안하면 캐릭터가 요동을 침... 이상함

                if (Pink.position.Y < hero.position.Y) //Pink Y축      내 캐릭터가 점프하면 같이 점프함.
                {
                    Pink.position.Y += 1.0f;
                }
                else//(Pink.position.Y > hero.position.Y)
                {
                    Pink.position.Y -= 1.0f;
                }
            }
            //End Pink Move
        }

        public void PinkAttack1()       //1번 공격
        {
            PinkOnWalking = false;
            PinkAttack = true;
            PinkAt2 = false;
            PinkAt3 = false;
            PinkAt1 = true;
            Pink.framecount++;
            if (Pink.framecount >= 60)
            {
                Pink.framecount = 0;
                PinkOnWalking = true;
                PinkAttack = false;
                hp.framecount -= 4;
            }
        }

        public void PinkAttack2()           //2번 공격
        {
            PinkOnWalking = false;
            PinkAttack = true;
            PinkAt1 = false;
            PinkAt3 = false;
            PinkAt2 = true;
            Pink.framecount++;
            if (Pink.framecount >= 60)
            {
                Pink.framecount = 0;
                PinkOnWalking = true;
                PinkAttack = false;
                hp.framecount -= 4;
            }
        }

        public void PinkAttack3()
        {
            hero.framecount = 100;
            PinkOnWalking = false;
            PinkAttack = true;
            PinkAt1 = false;
            PinkAt3 = true;
            PinkAt2 = false;
            Pink.framecount++;
            if (Pink.framecount >= 60) //Pink_Attack_3
            {
                Pink.framecount = 0;
                if (!Pinkflip)
                {
                    hero.position.X -= 200;
                    hero.framecount = 30;
                    hp.framecount -= 4;
                }
                else
                {
                    hero.position.X += 200;
                    hero.framecount = 0;
                    hp.framecount -= 4;
                }
            }
        }


        public void HeroMove(GameTime gameTime)     // 영웅 움직임
        {
            if (hero.framecount != 60 && hero.framecount != 70 && PinkAt1 ==false && PinkAt3 ==false )  // 핑크가 원거리, 잡기 공격않고 있으면 이동 가능
            {
                if (keyboardState.IsKeyDown(Keys.Left))     // 왼쪽으로 이동
                {
                    hero.framecount++;
                    hero.position.X -= 4.0f;
                    isRight = false;

                    if (hero.framecount >= 30)
                    {
                        hero.framecount = 0;
                    }
                }

                if (keyboardState.IsKeyDown(Keys.Right))        //오른쪽으로 이동
                {
                    hero.framecount++;
                    hero.position.X += 4.0f;
                    isRight = true;

                    if (hero.framecount >= 60 || hero.framecount < 30)
                    {
                        hero.framecount = 30;

                    }
                }

                if (keyboardState.IsKeyDown(Keys.Space) && previousKeyboardState.IsKeyUp(Keys.Space))   // 점프
                {
                    if (isGround == true && isJumping == false) //땅에 있으면 점프 가능
                    {
                        isJumping = true;
                        isDirUp = true;
                        isInAir = false;

                        maxHeight = (int)hero.position.Y - 50;      //  50만큼 올라갔다가 내려온다.
                        currentHeight = (int)hero.position.Y;
                    }
                    else
                    {   // 땅이아니고 점프중이면 점ㅁ프 불가능
                        isGround = false;
                    }
                }
                if (keyboardState.IsKeyDown(Keys.F) && previousKeyboardState.IsKeyUp(Keys.F))       //무기 바꾸기
                {   // 일반, 몽둥이, 새총 순으로 변경
                    if (nomal == true)
                    {
                        nomal = false;
                        bat = true;
                    }
                    else if (bat == true)
                    {
                        bat = false;
                        gun = true;
                    }
                    else if (gun == true)
                    {
                        gun = false;
                        nomal = true;
                    }
                    if (bat == true)
                        hero = hero_bat;
                    else if (nomal == true)
                        hero = state;
                    else if (gun == true)
                        hero = hero_gun;
                }
            }

            if (bat==true)      // 몽둥이 공격
            {
                CurrentTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f;     //현재 시간 저장
                batRectangle();     // 몽둥이 타격 범위 생성

                if(attack_delay == false)       // 딜레이가 있는 경우가 아니면 가능
                {
                    if(keyboardState.IsKeyDown(Keys.C) && previousKeyboardState.IsKeyUp(Keys.C))//공격
                    {
                        extime = CurrentTime;

                        if (isRight == false)
                                hero.framecount = 60;//Rectangle = bat1
                        else if (isRight == true)
                                hero.framecount = 70;//Rectangle = bat2
                        aCount=1;
                    }
                }
                if (CurrentTime - extime > effect_T)
                {
                        //캐릭터 화면 구성
                    if (aCount < 2)
                    {
                        if (hero.framecount == 60)
                        {
                            hero.framecount = 20;
                        }
                        else
                        {
                            hero.framecount = 50;
                        }
                    }
                    aCount++;
                }
            }
            else if (gun == true)       //새총 공격
            {
                CurrentTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f;
                if (attack_delay == false)      //딜레이 중이 아니면
                {
                    if (mp.framecount > 10)     //마나가 있어야 가능
                    {
                    if (keyboardState.IsKeyDown(Keys.C) && previousKeyboardState.IsKeyUp(Keys.C))//공격
                    {
                        mp.framecount -= 7;     //마나 -
                            foreach (GameObject ball in cannonBalls)
                            {
                                if (!ball.alive)    // 사용하지 않았다면
                                {   //  발사
                                    ball.position.X = hero.position.X + 150;
                                    ball.position.Y = hero.position.Y + 100;
                                    ball.alive = true;  //사용중
                                    if (isRight == true)
                                        ball.velocity = new Vector2(1, 0) * 10.0f; // 발사체 이동
                                    else
                                        ball.velocity = new Vector2(-1, 0) * 10.0f;
                               }
                           }
                        extime = CurrentTime;

                        if (isRight == false)
                            hero.framecount = 60;
                        else if (isRight == true)
                            hero.framecount = 70;


                        aCount = 1;


                    }
                    }
                }
                if (CurrentTime - extime > effect_T)
                {
                    if (aCount < 2)
                    {
                        if (hero.framecount == 60)
                        {
                            hero.framecount = 20;
                        }

                        else
                        {
                            hero.framecount = 50;
                        }
                    }
                    aCount++;
                }
            }
            HeroJump();     //점프
            UpdateCannonBalls();    //캐논볼 업데이트
        }

        public void HeroJump()
        {
            if (isJumping == true)
            {
                if (isDirUp == true)
                {
                    if (isInAir == false)
                    {
                        hero.position.Y -= 3;

                        if (isRight == false)
                            hero.framecount = 60;
                        else if (isRight == true)
                            hero.framecount = 70;

                        if (hero.position.Y <= maxHeight)       //계속 위로 이동
                            isInAir = true;
                    }
                    else
                    {
                        isDirUp = false;    //다운
                    }
                }
                else    //최대 점프력을 도달하면 이제 내려옴
                {
                    hero.position.Y += 3;

                    if (hero.framecount == 60)
                        hero.framecount = 0;
                    else if (hero.framecount == 70)
                        hero.framecount = 30;
                    if (hero.position.Y >= currentHeight)
                    {
                        isInAir = false;
                        isGround = true;
                        isJumping = false;
                        isDirUp = false;

                    }
                }
            }
        }

        public void batRectangle()//방망이 충돌영역
        {
            // 좌우 공격 범위 지정
            Rectangle bat1 = new Rectangle((int)hero.position.X, (int)hero.position.Y-100, 100, 100);
            Rectangle bat2 = new Rectangle((int)hero.position.X+222, (int)hero.position.Y-100, 100, 100);
            // 무기위 위치
            bat1.Offset((int)hero.position.X, (int)hero.position.Y);
            bat2.Offset((int)hero.position.X, (int)hero.position.Y);

            // 적의 피격 범위 지정
            Rectangle pinkR = new Rectangle((int)Pink.position.X, (int)Pink.position.Y, 100, 200);
            //적의 위치
            pinkR.Offset((int)Pink.position.X, (int)Pink.position.Y);
            // 좌우 이동중이면
            if (hero.framecount == 60 || hero.framecount == 70)
            {   //만약 적과 몽둥이 범위가 겹치면
                if (bat1.Intersects(pinkR) || pinkR.Intersects(bat1))
                {   // 공격
                    Pink.position.X += 30;
                    hitnum++;
                }
                if (bat2.Intersects(pinkR) || pinkR.Intersects(bat2))
                {   //공격
                    Pink.position.X -= 30;
                    hitnum++;
                }
            }
        }
        public void UpdateCannonBalls()
        {
            foreach (GameObject ball in cannonBalls)
            {   // 캐논볼이 날아가는 중이면
                if (ball.alive)
                {
                    ball.position += ball.velocity;     //이동
                    if (!viewportRect.Contains(new Point(
                        (int)ball.position.X,
                        (int)ball.position.Y)))
                    {   //만약 화면 밖으로 이동하면 종료
                        ball.alive = false;
                        continue;
                    }
                    //적 위치와 피격범위
                    Rectangle pinkR = new Rectangle((int)Pink.position.X, (int)Pink.position.Y,100,200);
                    pinkR.Offset((int)Pink.position.X, (int)Pink.position.Y);
                    // 캐논볼을 맞으면 피격
                    Rectangle cannonBallRect = new Rectangle(
                        (int)ball.position.X,
                        (int)ball.position.Y,
                       100,
                      100);
                    if (ball.position.X == Pink.position.X)
                    {
                        hitnum++;
                    }
                    if (isRight == true)
                    {
                        if (cannonBallRect.Intersects(pinkR) || pinkR.Intersects(cannonBallRect))
                        {
                            Pink.position.X += 100;
                            hitnum++;
                            ball.alive = false;
                        }
                    }
                    else
                    {
                        if (cannonBallRect.Intersects(pinkR)||pinkR.Intersects(cannonBallRect))
                        {
                            Pink.position.X -= 100;
                            hitnum++;
                            ball.alive = false;
                        }

                    }
                }
            }
        }
    }
}
