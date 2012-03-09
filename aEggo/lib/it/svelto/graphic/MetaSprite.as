package it.svelto.graphic
{
	import flash.geom.Rectangle;
	public class MetaSprite
	{
		private var _name:String;
		private var _animations:Vector.<MetaSpriteAnimation>;
		private var _boundingRect:Rectangle;
		
		function MetaSprite(name:String)
		{
			_name = name;
			_animations = new Vector.<MetaSpriteAnimation>;
			_boundingRect = new Rectangle;
		}
		
		public function addMetaAnimation(metaAnimation:MetaSpriteAnimation):void
		{
			_animations.push(metaAnimation);
			
			_boundingRect = _boundingRect.union(metaAnimation.getBoundingRect());
		}
		
		public function get metaAnimations():Vector.<MetaSpriteAnimation>
		{
			return _animations;
		}
		
		public function getBoundingRect():Rectangle
		{
			return _boundingRect;
		}
	}
}