package it.svelto.graphic.flashpunk 
{
	import flash.geom.Rectangle;
	import net.flashpunk.graphics.Image;
	import net.flashpunk.FP;
	
	public class ImageEx extends Image
	{
		function ImageEx(source:*, clipRect:Rectangle = null) 
		{
			super(source, clipRect);
		}
		
		/**
		 * Updates the image buffer.
		 */
		override public function updateBuffer(clearBefore:Boolean = false):void
		{
			_bufferRect = _sourceRect.clone();
			_bufferRect.x = 0;
			_bufferRect.y = 0;
			
			super.updateBuffer(clearBefore);
		}
	}
}