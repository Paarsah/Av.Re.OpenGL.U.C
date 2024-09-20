using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace Mag3DView.Views
{
    public class OpenGLView : GameWindow
    {
        private int _vertexBufferObject;
        private int _vertexArrayObject;
        private int _shaderProgram;

        // Single point vertex at (0.0, 0.0, 0.0)
        private readonly float[] _pointVertex = { 0.0f, 0.0f, 0.0f };
        private readonly float[] _pointsVertex =
        {
            -0.5f, -0.5f, 0.0f, // Bottom-left vertex
             0.5f, -0.5f, 0.0f, // Bottom-right vertex
             0.0f,  0.5f, 0.0f  // Top vertex
        };
        public OpenGLView()
            : base(GameWindowSettings.Default, new NativeWindowSettings()
            {
                Size = new Vector2i(800, 450),
                Title = "OpenTK Window"
            })
        {
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            // Gray background
            GL.ClearColor(0.5f, 0.5f, 0.5f, 1.0f);

            // Setup Vertex Array Object (VAO) and Vertex Buffer Object (VBO)
            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _pointsVertex.Length * sizeof(float), _pointsVertex, BufferUsageHint.StaticDraw);

            // Create Vertex Array Object
            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            // Compile shaders
            _shaderProgram = CreateShaderProgram();
            GL.UseProgram(_shaderProgram);

            GL.Enable(EnableCap.DepthTest); // Enable depth testing

            // Set point size (bigger to make it more visible)
            GL.PointSize(10.0f); // Adjust size as needed
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.UseProgram(_shaderProgram);
            GL.BindVertexArray(_vertexArrayObject);

            // Draw the point
            GL.DrawArrays(PrimitiveType.Points, 0, 3); // Draw 3 point

            SwapBuffers();
        }

        private int CreateShaderProgram()
        {
            // Compile vertex shader
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

            // Compile fragment shader
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

            // Link shaders into a program
            var shaderProgram = GL.CreateProgram();
            GL.AttachShader(shaderProgram, vertexShader);
            GL.AttachShader(shaderProgram, fragmentShader);
            GL.LinkProgram(shaderProgram);

            // Clean up shaders (they are no longer needed once linked)
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);

            return shaderProgram;
        }

        protected override void OnUnload()
        {
            base.OnUnload();

            // Cleanup resources
            GL.DeleteBuffer(_vertexBufferObject);
            GL.DeleteVertexArray(_vertexArrayObject);
            GL.DeleteProgram(_shaderProgram);
        }
    }
}
