using System;
using System.Drawing;
using GXPEngine.Core;

namespace GXPEngine
{
	/// <summary>
	/// The Canvas object can be used for drawing 2D visuals at runtime.
	/// </summary>
	public class Canvas : Sprite
	{
		protected Graphics _graphics;
		protected bool _invalidate = false;

		//------------------------------------------------------------------------------------------------------------------------
		//														Canvas()
		//------------------------------------------------------------------------------------------------------------------------

		/// <summary>
		/// Initializes a new instance of the Canvas class.
		/// It is a regular GameObject that can be added to any displaylist via commands such as AddChild.
		/// It contains a <a href="http://msdn.microsoft.com/en-us/library/system.drawing.graphics(v=vs.110).aspx">System.Drawing.Graphics</a> component.
		/// </summary>
		/// <param name='width'>
		/// Width of the canvas in pixels.
		/// </param>
		/// <param name='height'>
		/// Height of the canvas in pixels.
		/// </param>
		public Canvas (int width, int height, bool addCollider=true) : this(new Bitmap (width, height), addCollider)
		{
			name = width + "x" + height;
		}

		public Canvas (System.Drawing.Bitmap bitmap, bool addCollider=true) : base (bitmap,addCollider)
		{
			_graphics = Graphics.FromImage(bitmap);
			_invalidate = true;
		}

		public Canvas(string filename, bool addCollider=true):base(filename, false, addCollider)
		{
			_graphics = Graphics.FromImage(texture.bitmap);
			_invalidate = true;
		}


		//------------------------------------------------------------------------------------------------------------------------
		//														graphics
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Returns the graphics component. This interface provides tools to draw on the sprite.
		/// See: <a href="http://msdn.microsoft.com/en-us/library/system.drawing.graphics(v=vs.110).aspx">System.Drawing.Graphics</a>
		/// </summary>
		public Graphics graphics {
			get { 
				_invalidate = true;
				return _graphics; 
			}
		}

		//------------------------------------------------------------------------------------------------------------------------
		//														Render()
		//------------------------------------------------------------------------------------------------------------------------
		override protected void RenderSelf(GLContext glContext) {
			if (_invalidate) {
				_texture.UpdateGLTexture ();
				_invalidate = false;
			}

			base.RenderSelf (glContext);
		}

		//------------------------------------------------------------------------------------------------------------------------
		//														alpha
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Draws a Sprite onto this Canvas.
		/// It will ignore color and alpha, but it will use position, rotation, scale, origin, current frame.
		/// </summary>
		/// <param name='sprite'>
		/// The Sprite that should be drawn.
		/// </param>
		private PointF[] destPoints = new PointF[3];
		public void DrawSprite(Sprite sprite) {
			float wd = sprite.texture.width;
			float ht = sprite.texture.height;
			Vector2[] corners = sprite.GetExtents();
			destPoints[0] = new PointF(corners[0].x, corners[0].y);
			destPoints[1] = new PointF(corners[1].x, corners[1].y);
			destPoints[2] = new PointF(corners[3].x, corners[3].y);
			var uvs = sprite.GetUVs();
			graphics.DrawImage(
				sprite.texture.bitmap, 
				destPoints, 
				new RectangleF(uvs[0]*wd,uvs[1]*ht,(uvs[2]-uvs[0])*wd,(uvs[5]-uvs[1])*ht), 
				GraphicsUnit.Pixel
			);
		}

		// Called by the garbage collector
		~Canvas() {
			_graphics.Dispose();
		}
	}
}

