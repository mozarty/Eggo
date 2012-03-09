package it.svelto.graphic
{
	import flash.geom.Point;
	import flash.geom.Rectangle;
	
	public class MetaFrame
	{
		private var _sourceRect:Rectangle;
		private var _spriteRect:Rectangle;
		private var _duration:uint;
		private var _offset:Point;
							
		function MetaFrame(rect:Rectangle)
		{
			_duration = 1;
			_offset = new Point(0, 0);
			_sourceRect = rect;
			_spriteRect = rect.clone();
			_spriteRect.x = 0;
			_spriteRect.y = 0;
		}
		
		public function set duration(time:uint):void
		{
			_duration = time;
		}
		
		public function get offset():Point
		{
			return _offset;
		}
		
		public function set offset(point:Point):void
		{
			_offset = point;
		}
		
		public function get sourceRect():Rectangle
		{
			return _sourceRect;
		}
		
		public function get spriteRect():Rectangle
		{
			return _spriteRect;
		}
		
		public function get frameRate():Number
		{
			return 1000.0 / _duration;
		}
	}
}