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

        // Content
        //Texture2D cubeTexture;
        BasicEffect cubeEffect;

        //Testing block as a camera target
        Block cameraTarget = new Block(new Vector3(1.2f, 1.2f, 1.2f), new Vector3(0f,0f,0f), 0);

        
        List<Block> cubes = new List<Block>();


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
            TargetElapsedTime = TimeSpan.FromTicks(333333);
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
            for (int i = 0; i<8; i++)
            {
                cubes.Add(new Block(new Vector3(1f,1f,1f), new Vector3(i,0,0), 0)); //X - Grass
                cubes.Add(new Block(new Vector3(1f,1f,1f), new Vector3(0,i,0), 1)); //Y - Dirt
                cubes.Add(new Block(new Vector3(1f,1f,1f), new Vector3(0,0,i), 2)); //Z -Stone

                cubes[0].Size = new Vector3(2.0f, 2.0f, 2.0f);
                cubes[0].Position = new Vector3(-0.5f, -0.5f, -0.5f);

                for (int j = 0; j<8; j++)
                {
                    for (int k = 0; k < 8; k++) ;
                    //cubes.Add(new Block(new Vector3(1f,1f,1f), new Vector3(i,j,k), i % 2));
                    
                }
            }

            cameraTarget.Color = new Color(1.0f, 0.0f, 0.0f);
            

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //cubeTexture = Content.Load<Texture2D>("CubetacularTexture");
            aspectRatio = GraphicsDevice.Viewport.AspectRatio;
            cubeEffect = new BasicEffect(GraphicsDevice);

            camera = new ArcBallCamera(aspectRatio, new Vector3(0.0f,0.0f,0f));
            camera.MoveCameraRight(0);
            camera.MoveCameraForward(0);

            
            //UpdateViewMatrix();

            Mouse.SetPosition(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height/2);
            originalMouseState = Mouse.GetState();

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

            Console.WriteLine("Lk@: " + camera.LookAt);
            cameraTarget.Position = camera.LookAt - new Vector3(0.5f, 0.5f, 0.5f);
            cameraTarget.setDirty();
            Console.WriteLine("Pos: " + cameraTarget.Position);
            base.Update(gameTime);
            Console.WriteLine("Pitch: " + camera.Pitch);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            cubeEffect.View = camera.ViewMatrix;

            // Set the Projection matrix which defines how we see the scene (Field of view)
            cubeEffect.Projection = camera.ProjectionMatrix;

            // Enable textures on the Cube Effect. this is necessary to texture the model
            cubeEffect.TextureEnabled = false;
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

                foreach( Block c in cubes)
                {
                    c.RenderToDevice(GraphicsDevice);
                }

                cameraTarget.RenderToDevice(GraphicsDevice);
            }

            base.Draw(gameTime);
        }
    }
}
