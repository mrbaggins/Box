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

namespace Box
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont debugFont;

        int frameRate = 0;
        int frameCounter = 0;
        TimeSpan elapsedTime = TimeSpan.Zero;

        // Content
        //Texture2D cubeTexture;
        BasicEffect cubeEffect;

        //Testing block as a camera target
        Block cameraTarget = new Block(new Vector3(1.2f, 1.2f, 1.2f), new Vector3(0f,0f,0f), 0);

        
        List<Block> cubes = new List<Block>();
        Chunk c;


        // Position related variables
        float leftRightRotation = 0;//MathHelper.PiOver2;
        float upDownRotation = 0;//-MathHelper.Pi / 10.0f;
        const float rotationSpeed = 0.003f;
        const float moveSpeed = 0.8f;
        MouseState originalMouseState;
        int lastScrollValue = 0;

        
        ArcBallCamera camera;
        int zoom = 100;

        float aspectRatio = 0.0f;

        public Game1()
        {
            
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // Frame rate is 30 fps by default for Windows Phone.
            //TargetElapsedTime = TimeSpan.FromTicks(333333);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            //Dummy chunk for testing
            c = new Chunk();

            //Camera Target block (Covers Camera.LookAt coordinates)
            cameraTarget.Color = new Color(1.0f, 0.0f, 0.0f);

            //Set up some stuff for graphics
            aspectRatio = GraphicsDevice.Viewport.AspectRatio;
            cubeEffect = new BasicEffect(GraphicsDevice);
            graphics.SynchronizeWithVerticalRetrace = false;    //Turns off VSync
            this.IsFixedTimeStep = true;                        //Make sure it's fixed timestep. 
            //this.TargetElapsedTime = new TimeSpan(6666);      //Sets the fixed timestep
            graphics.ApplyChanges();                            //Actually sticks the above changes
            
            camera = new ArcBallCamera(aspectRatio, new Vector3(0.0f, 0.0f, 0f));
            //camera.MoveCameraRight(0);
            //camera.MoveCameraForward(0);

            //Set up the mouse
            Mouse.SetPosition(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
            originalMouseState = Mouse.GetState();
            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Create a font for drawing text to the screen with the Spritebatch
            debugFont = Content.Load<SpriteFont>("DebugFont");
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {

            #region keyboard controls
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            KeyboardState state = Keyboard.GetState();
            if(state.IsKeyDown(Keys.Escape))
                this.Exit();

            if (state.IsKeyDown(Keys.A))
                camera.MoveCameraRight(-1);
            if (state.IsKeyDown(Keys.D))
                camera.MoveCameraRight(1);
            if (state.IsKeyDown(Keys.W))
                camera.MoveCameraForward(1);
            if (state.IsKeyDown(Keys.S))
                camera.MoveCameraForward(-1);
            if (state.IsKeyDown(Keys.Space))
                camera.MoveCameraUp(1);
            if (state.IsKeyDown(Keys.LeftShift))
                camera.MoveCameraUp(-1);
            #endregion 
            #region mouse controls
            MouseState currentMouseState = Mouse.GetState();
            if (lastScrollValue > currentMouseState.ScrollWheelValue)
                zoom += 5;
            else if (lastScrollValue < currentMouseState.ScrollWheelValue)
                zoom -= 5;

            if (zoom < camera.MinZoom)
                zoom = 0;

            lastScrollValue = currentMouseState.ScrollWheelValue;

            if (currentMouseState != originalMouseState)
            {
                float xDiff = currentMouseState.X - originalMouseState.X;
                float yDiff = currentMouseState.Y - originalMouseState.Y;
                leftRightRotation -= rotationSpeed * xDiff;
                upDownRotation += rotationSpeed * yDiff; //Invert this sign for "normal" mouse input.
                upDownRotation = MathHelper.Clamp(upDownRotation, camera.MinPitch, camera.MaxPitch);
                
                camera.Pitch = upDownRotation;
                camera.Yaw = leftRightRotation;
               
                camera.Zoom = zoom;

                Mouse.SetPosition(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
            }
            #endregion

            cameraTarget.Position = camera.LookAt - new Vector3(0.5f, 0.5f, 0.5f); //Can this be removed to somewhere else?
            cameraTarget.setDirty();

            //Framerate counter
            elapsedTime += gameTime.ElapsedGameTime;

            if (elapsedTime > TimeSpan.FromSeconds(1))
            {
                elapsedTime -= TimeSpan.FromSeconds(1);
                frameRate = frameCounter;
                frameCounter = 0;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            cubeEffect.View = camera.ViewMatrix;                //View Matrix does what?
            cubeEffect.Projection = camera.ProjectionMatrix;    //Projection is the field of view

            cubeEffect.TextureEnabled = false;                  //turn ooff the textures in the effect shader
            //cubeEffect.Texture = cubeTexture;

            // Enable some pretty lights
            cubeEffect.EnableDefaultLighting();
            cubeEffect.VertexColorEnabled = true;
            Vector3 lightDirection = new Vector3(1.0f, -1.0f, -1.0f);
            lightDirection.Normalize();
            cubeEffect.LightingEnabled = true;
            cubeEffect.AmbientLightColor = new Vector3(0.1f,0.1f,0.1f);

            // apply the effect and render the cube
            foreach (EffectPass pass in cubeEffect.CurrentTechnique.Passes)
            {
                pass.Apply();

                #region cube data
                VertexBuffer vb = new VertexBuffer(GraphicsDevice, VertexPositionColorNormal.VertexDeclaration, c.VertexList.Count, BufferUsage.WriteOnly);
                IndexBuffer ib = new IndexBuffer(GraphicsDevice, typeof(int), c.IndexList.Count, BufferUsage.WriteOnly);

                VertexPositionColorNormal[] vl = c.VertexList.ToArray();
                int[] il = c.IndexList.ToArray();
                
                vb.SetData<VertexPositionColorNormal>(vl);
                ib.SetData<int>(il);
                Console.WriteLine(GraphicsDevice.Indices);

                GraphicsDevice.SetVertexBuffer(vb);
                GraphicsDevice.Indices = ib;

                GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vb.VertexCount, 0, c.IndexList.Count/3);
                #endregion

                //Draw the camera target (camera.lookat)
                cameraTarget.RenderToDevice(GraphicsDevice);
            }

            #region Debug text drawing
            frameCounter++;

            string fps = string.Format("fps: {0}", frameRate);

            spriteBatch.Begin();

            spriteBatch.DrawString(debugFont, fps, new Vector2(33, 33), Color.Black);
            spriteBatch.DrawString(debugFont, fps, new Vector2(32, 32), Color.White);

            spriteBatch.End();
            #endregion

            //Reset graphics device states for drawing in 3D
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            base.Draw(gameTime);
        }
    }
}
