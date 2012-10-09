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

        // Create a cube with a size of 1 (all dimensions) at the origin
        Block cube = new Block(new Vector3(1, 1, 1), Vector3.Zero, 0);
        Block cube2 = new Block(new Vector3(1, 1, 1), new Vector3(1.0f, 0.1f, 0.1f), 1);

        List<Block> cubes = new List<Block>();


        // Position related variables
        Vector3 cameraPosition = new Vector3(80, 30, -50);
        float leftRightRotation = MathHelper.PiOver2;
        float upDownRotation = -MathHelper.Pi / 10.0f;
        const float rotationSpeed = 0.003f;
        const float moveSpeed = 0.8f;
        Matrix viewMatrix;
        MouseState originalMouseState;
        
        //Vector3 modelPosition = Vector3.Zero;
        
        //float XRotation = 0.0f;
        //float YRotation = 0.0f;
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
                for (int j = 0; j<8; j++)
                {
                    for ( int k = 0; k<8; k++)
                    cubes.Add(new Block(new Vector3(1f,1f,1f), new Vector3(i,j,k), k % 3));

                }
            }

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
            UpdateViewMatrix();

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

            Vector3 moveVector = new Vector3(0,0,0);

            if (state.IsKeyDown(Keys.Left))
                moveVector += new Vector3(-1,0,0);
            if (state.IsKeyDown(Keys.Right))
                moveVector += new Vector3(1, 0, 0);
            if (state.IsKeyDown(Keys.Up))
                moveVector += new Vector3(0, 0, -1);
            if (state.IsKeyDown(Keys.Down))
                moveVector += new Vector3(0, 0, 1);

            AddToCameraPosition(moveVector);

            MouseState currentMouseState = Mouse.GetState();
            if (currentMouseState != originalMouseState)
            {
                float xDiff = currentMouseState.X - originalMouseState.X;
                float yDiff = currentMouseState.Y - originalMouseState.Y;
                leftRightRotation -= rotationSpeed * xDiff;
                upDownRotation += rotationSpeed * yDiff; //Invert this sign for "normal" mouse input.
                Mouse.SetPosition(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
                UpdateViewMatrix();
            }

            base.Update(gameTime);
        }

        private void AddToCameraPosition(Vector3 moveVector)
        {
            Matrix cameraRotation = Matrix.CreateRotationX(upDownRotation) * Matrix.CreateRotationY(leftRightRotation);
            Vector3 rotatedVector = Vector3.Transform(moveVector, cameraRotation);
            cameraPosition += moveSpeed * rotatedVector;
            UpdateViewMatrix();

        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Set the World matrix which defines the position of the cube
            //cubeEffect.World = Matrix.CreateRotationY(MathHelper.ToRadians(XRotation)) *
               // Matrix.CreateRotationX(MathHelper.ToRadians(YRotation)) * Matrix.CreateTranslation(modelPosition);

            // Set the View matrix which defines the camera and what it's looking at
            //cubeEffect.View = Matrix.CreateLookAt(cameraPosition, modelPosition, Vector3.Up);
            cubeEffect.View = viewMatrix;

            // Set the Projection matrix which defines how we see the scene (Field of view)
            cubeEffect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1.0f, 1000.0f);

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

            //cubeEffect.Parameters["xLightDirection"].SetValue(lightDirection);
            //cubeEffect.Parameters["xAmbient"].SetValue(0.1f); 
            //cubeEffect.Parameters["xEnableLighting"].SetValue(true);       

            // apply the effect and render the cube
            foreach (EffectPass pass in cubeEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                //cube.RenderToDevice(GraphicsDevice);
                //cube2.RenderToDevice(GraphicsDevice);

                foreach( Block c in cubes)
                {
                    c.RenderToDevice(GraphicsDevice);
                }
            }

            base.Draw(gameTime);
        }

        private void UpdateViewMatrix()
        {
            Matrix cameraRotation = Matrix.CreateRotationX(upDownRotation) * Matrix.CreateRotationY(leftRightRotation);

            Vector3 cameraOriginalTarget = new Vector3(0,0,-1);
            Vector3 cameraRotatedTarget = Vector3.Transform(cameraOriginalTarget, cameraRotation);
            Vector3 cameraFinalTarget = cameraPosition + cameraRotatedTarget;

            Vector3 cameraOriginalUpVector = new Vector3(0, 1, 0);
            Vector3 cameraRotatedUpVector = Vector3.Transform(cameraOriginalUpVector, cameraRotation);

            viewMatrix = Matrix.CreateLookAt(cameraPosition, cameraFinalTarget, cameraRotatedUpVector);

        }
    }
}
