package it.svelto.graphic
{
	import flash.geom.Rectangle;
	
	public class MetaSpriteAnimation
	{
		private var _name:String;
		private var _bitmapSource:String;
		private var _transparentColour:uint;
		private var _frames:Vector.<MetaFrame>;
		private var _animationRect:Rectangle;
		private var isLoop:Boolean=true;
		
		function MetaSpriteAnimation(name:String)
		{
			_name = name;
			
			_frames = new Vector.<MetaFrame>;
			
			_animationRect = new Rectangle;
		}
		
		public function set bitmapSource(bitmapPath:String):void
		{
			_bitmapSource = bitmapPath;
		}
		
		public function set transparentColour(colour:int):void
		{
			_transparentColour = colour;
		}
		
		public function set loops(isloop:Boolean):void
		{
			isLoop = isloop;
		}
		
		public function get name():String
		{
			return _name;
		}
		
		public function get frames():uint
		{
			return _frames.length;
		}
		
		public function get loops():Boolean
		{
			return isLoop;
		}
		
		public function addFrame(metaFrame:MetaFrame):void
		{
			_frames.push(metaFrame);
			
			_animationRect = _animationRect.union(metaFrame.spriteRect);
		}
		
		public function getFrame(frame:int):MetaFrame
		{
			return _frames[frame];
		}
		
		public function getBoundingRect():Rectangle
		{
			return _animationRect;
		}
	}
}