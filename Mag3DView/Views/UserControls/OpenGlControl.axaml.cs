using Avalonia.Controls;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System;

namespace Mag3DView.Views.UserControls
{
    public partial class OpenGlControl : UserControl
    {
        private int _vertexBufferObject;
        private int _vertexArrayObject;
        private int _shaderProgram;

        private readonly float[] _pointsVertex =
        {
            -0.5f, -0.5f, 0.0f, // Bottom-left vertex
             0.5f, -0.5f, 0.0f, // Bottom-right vertex
             0.0f,  0.5f, 0.0f  // Top vertex
        };

        private GameWindow _gameWindow;

        public OpenGlControl()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            // Create OpenGL window with settings
            var gameWindowSettings = GameWindowSettings.Default;
            var nativeWindowSettings = new NativeWindowSettings()
            {
                Size = new OpenTK.Mathematics.Vector2i(800, 450), // Use Vector2i for size
                Title = "OpenTK Window"
            };

            _gameWindow = new GameWindow(gameWindowSettings, nativeWindowSettings);
            _gameWindow.MakeCurrent(); // Make it current

            InitializeOpenGL();
            _gameWindow.RenderFrame += OnRenderFrame; // Subscribe to render event
            _gameWindow.Run(); // Start the rendering loop
        }

        private void InitializeOpenGL()
        {
            GL.ClearColor(0f, 0f, 0f, 1f); // Set clear color
            InitializeBuffers();
        }

        private void InitializeBuffers()
        {
            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _pointsVertex.Length * sizeof(float), _pointsVertex, BufferUsageHint.StaticDraw);

            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            _shaderProgram = CreateShaderProgram();
            GL.UseProgram(_shaderProgram);

            GL.Enable(EnableCap.DepthTest);
        }

        private int CreateShaderProgram()
        {
            var vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, @"
                #version 330 core
                layout (location = 0) in vec3 aPosition;
                void main()
                {
                    gl_Position = vec4(aPosition, 1.0);
                }
            ");
            GL.CompileShader(vertexShader);

            var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, @"
                #version 330 core
                out vec4 FragColor;
                void main()
                {
                    FragColor = vec4(1.0, 0.0, 0.0, 1.0); // Red color
                }
            ");
            GL.CompileShader(fragmentShader);

            var shaderProgram = GL.CreateProgram();
            GL.AttachShader(shaderProgram, vertexShader);
            GL.AttachShader(shaderProgram, fragmentShader);
            GL.LinkProgram(shaderProgram);

            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);

            return shaderProgram;
        }

        private void OnRenderFrame(FrameEventArgs e) // Updated the method signature
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.UseProgram(_shaderProgram);
            GL.BindVertexArray(_vertexArrayObject);
            GL.DrawArrays(PrimitiveType.Points, 0, 3);
            _gameWindow.SwapBuffers(); // Swap the buffers
        }

        protected override void OnDetachedFromVisualTree(Avalonia.VisualTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromVisualTree(e);
            GL.DeleteBuffer(_vertexBufferObject);
            GL.DeleteVertexArray(_vertexArrayObject);
            GL.DeleteProgram(_shaderProgram);
            _gameWindow.Dispose(); // Dispose the OpenGL window
        }
    }
}
