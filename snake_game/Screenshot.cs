using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Drawing;

namespace snake_game
{
	public static class Screenshot
	{
		public static Texture2D TakeScreenshot(GraphicsDevice graphics)
		{
			int w, h;
			w = graphics.PresentationParameters.BackBufferWidth;
			h = graphics.PresentationParameters.BackBufferHeight;
			RenderTarget2D screenshot;
			screenshot = new RenderTarget2D(GraphicsDevice, w, h, false, SurfaceFormat.Bgra32, DepthFormat.None);
			graphics.SetRenderTarget(screenshot);
			// _lastUpdatedGameTime is a variable typed GameTime, used to record the time last updated and create a common time standard for some game components
			Draw(_lastUpdatedGameTime != null ? _lastUpdatedGameTime : new GameTime());
			graphics.Present();
			graphics.SetRenderTarget(null);
			return screenshot;
		}
		private static void Save(this Texture2D texture, ImageFormat imageFormat, Stream stream)
		{
			var width = texture.Width;
			var height = texture.Height;
			using (Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb))
			{
				IntPtr safePtr;
				BitmapData bitmapData;
				System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, width, height);

				texture.GetData(textureData);
				bitmapData = bitmap.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
				safePtr = bitmapData.Scan0;
				Marshal.Copy(textureData, 0, safePtr, textureData.Length);
				bitmap.UnlockBits(bitmapData);
				bitmap.Save(stream, imageFormat);

				textureData = null;
			}
			GC.Collect();
		}
	}
}
