

using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpBox2D.Common;
using SharpBox2D.TestBed.Testbed.Framework.j2d;

namespace SharpBox2D.TestBed.Framework.j2d
{


    public class MonoGameTestbedError : TestbedErrorHandler
    {
        public void serializationError(Exception e, string message)
        {
            Debug.WriteLine(message);
        }
    }

/**
 * The entry point for the testbed application
 * 
 * @author Daniel Murphy
 */

    public static class TestbedMain
    {
        private static MonoGameDebugDraw _debugDraw;
        // private static final Logger log = LoggerFactory.getLogger(TestbedMain.class);

        public static void Main(string[] args)
        {
            // try {
            // UIManager.setLookAndFeel("com.sun.java.swing.plaf.nimbus.NimbusLookAndFeel");
            // } catch (Exception e) {
            // log.warn("Could not set the look and feel to nimbus.  "
            // + "Hopefully you're on a mac so the window isn't ugly as crap.");
            // }
            TestbedModel model = new TestbedModel();
            TestbedController controller = new TestbedController(model, UpdateBehavior.UPDATE_CALLED,
                MouseBehavior.NORMAL, new MonoGameTestbedError());

            using (var game = new TestBedGame(model))
            {
               
                game.Run();
            }

            //        ;
            //        TestPanelJ2D panel = new TestPanelJ2D(model, controller);
            //        model.setPanel(panel);
            //        model.setDebugDraw(new DebugDrawJ2D(panel, true));
            //        TestList.populateModel(model);

            //        JFrame testbed = new JFrame();
            //        testbed.setTitle("JBox2D Testbed");
            //        testbed.setLayout(new BorderLayout());
            //        TestbedSidePanel side = new TestbedSidePanel(model, controller);
            //        testbed.add((Component) panel, "Center");
            //        testbed.add(new JScrollPane(side), "East");
            //        testbed.pack();
            //        testbed.setVisible(true);
            //        testbed.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
            //        System.out.
            //        println(System.getProperty("java.home"));

            //        SwingUtilities.invokeLater(new Runnable()
            //        {
            //            @Override
            //        public void run() {
            //controller.playTest(0);
            //controller.start();
            //        }
            //    }
            //    )
            //        ;
            //    }
        }
    }

    public class TestBedGame : Game, TestbedPanel
    {
        private TestbedController _controller;
        private MonoGameDebugDraw _debugDraw;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private TestbedModel _model;
        private SpriteFont spriteFont;
        private BasicEffect basicEffect;
        private KeyboardState oldState;
        private GamePadState oldGamePad;
        private int width;
        private int height;
        private int tw;
        private int th;
        private float viewZoom = 1f;
        private Vector2 viewCenter = new Vector2(0.0f, 20.0f);

        public TestBedGame(TestbedModel model)
        {
            _model = model;
            IsMouseVisible = true;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
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
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            //spriteFont = Content.Load<SpriteFont>("font");
            basicEffect = new BasicEffect(GraphicsDevice);
            basicEffect.VertexColorEnabled = true;

            MonoGameDebugDraw._batch = spriteBatch;
            MonoGameDebugDraw._device = GraphicsDevice;
            MonoGameDebugDraw._font = spriteFont;

            oldState = Keyboard.GetState();
            oldGamePad = GamePad.GetState(PlayerIndex.One);
            
       

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            _model.setPanel(this);
            _controller = new TestbedController(_model, UpdateBehavior.UPDATE_CALLED, MouseBehavior.NORMAL, new MonoGameTestbedError());
           
          
            _debugDraw = new MonoGameDebugDraw();
            _model.setDebugDraw(_debugDraw);
            TestList.populateModel(_model);

            _controller.playTest(7);
            _controller.start();

            Resize(GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight);

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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
            //Keyboard.GetState().IsKeyDown(Keys.Space)
            //    _controller.playTest(_controller.t);
            _controller.Update();
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            GraphicsDevice.Clear(Color.Black);

            basicEffect.Techniques[0].Passes[0].Apply();

            
            
            _debugDraw.FinishDrawShapes();

            
            spriteBatch.Begin();
            //_debugDraw.FinishDrawString();
            spriteBatch.End();

            
        }

        public void grabFocus()
        {
            
        }

        public bool render()
        {
            return true;
        }

        public void paintScreen()
        {
            
        }

        void Resize(int w, int h)
        {
            width = w;
            height = h;

            tw = GraphicsDevice.Viewport.Width;
            th = GraphicsDevice.Viewport.Height;
            int x = GraphicsDevice.Viewport.X;
            int y = GraphicsDevice.Viewport.Y;

            float ratio = (float)tw / (float)th;

            Vector2 extents = new Vector2(ratio * 25.0f, 25.0f);
            extents *= viewZoom;

            Vector2 lower = viewCenter - extents;
            Vector2 upper = viewCenter + extents;

            // L/R/B/T
            basicEffect.Projection = Matrix.CreateOrthographicOffCenter(lower.X, upper.X, lower.Y, upper.Y, -1, 1);
        }

        Vector2 ConvertScreenToWorld(int x, int y)
        {
            float u = x / (float)tw;
            float v = (th - y) / (float)th;

            float ratio = (float)tw / (float)th;
            Vector2 extents = new Vector2(ratio * 25.0f, 25.0f);
            extents *= viewZoom;

            Vector2 lower = viewCenter - extents;
            Vector2 upper = viewCenter + extents;

            Vector2 p = new Vector2();
            p.X = (1.0f - u) * lower.X + u * upper.X;
            p.Y = (1.0f - v) * lower.Y + v * upper.Y;
            return p;
        }
    }
}
