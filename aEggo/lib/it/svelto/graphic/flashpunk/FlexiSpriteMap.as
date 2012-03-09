package it.svelto.graphic.flashpunk
{
	import flash.geom.Rectangle;
	import it.svelto.graphic.MetaFrame;
	import it.svelto.graphic.MetaSprite;
	import it.svelto.graphic.MetaSpriteAnimation;
	import net.flashpunk.FP;
	import it.svelto.graphic.flashpunk.ImageEx;
	
	public class FlexiSpriteMap extends ImageEx
	{
		/**
		 * If the animation has stopped.
		 */
		public var complete:Boolean = true;
		
		/**
		 * Optional callback function for animation end.
		 */
		public var animationEndcallback:Function;
		
		public var frameChangedCallback:Function;
		
		/**
		 * Animation speed factor, alter this to speed up/slow down all animations.
		 */
		public var rate:Number = 1;
		
		function FlexiSpriteMap(source:*, metaSprite:MetaSprite)
		{
			_rect = metaSprite.getBoundingRect();
			
			super(source, _rect);
			
			for each (var metaAnimation:MetaSpriteAnimation in metaSprite.metaAnimations)
				add(metaAnimation);
				
			active = true;
		}
		
		private function add(metaAnimation:MetaSpriteAnimation):void
		{
			if (_anims[metaAnimation.name]) throw new Error("Cannot have multiple animations with the same name");
			
			_anims[metaAnimation.name] = metaAnimation;
		}
		
		/**
		 * Updates the spritemap's buffer.
		 */
		override public function updateBuffer(clearBefore:Boolean = false):void 
		{
			if (_frame == null) return; 
			
			var rect:Rectangle = _frame.sourceRect;
			
			// get position of the current frame
			_rect.x = rect.x;
			_rect.y = rect.y;
			_rect.width = rect.width;
			_rect.height = rect.height;
			
			if (_flipped) _rect.x = _source.width - (_rect.width + _rect.x);
			
			// update the buffer
			super.updateBuffer(clearBefore);
		}
		
		/** @private Updates the animation. */
		override public function update():void 
		{
			if (_anim && !complete)
			{
				_timer += (FP.timeInFrames ? _frame.frameRate : _frame.frameRate * FP.elapsed) * rate;
				
				if (_timer >= 1)
				{
					while (_timer >= 1)
					{
						_timer --;
						_index ++;

						if (_index == _anim.frames)
						{
							if (_anim.loops)
							{
								_index = 0;
								if (animationEndcallback != null) animationEndcallback();
							}
							else
							{
								_index = _anim.frames - 1;
								complete = true;
								if (animationEndcallback != null) animationEndcallback();
								break;
							}
						}	
					}
					
					_frame = (_anim.getFrame(_index));
					
					x =  _frame.offset.x;
					y =  _frame.offset.y;
				
					if (frameChangedCallback != null) frameChangedCallback(_frame);
					
					updateBuffer();
				}
			}
		}
		
		/**
		 * Plays an animation.
		 * @param	name		Name of the animation to play.
		 * @param	reset		If the animation should force-restart if it is already playing.
		 * @param	frame		Frame of the animation to start from, if restarted.
		 * @return	Anim object representing the played animation.
		 */
		public function play(name:String = "", reset:Boolean = false, frame:int = 0, frameCallback:Function = null, animationCallback:Function = null):MetaSpriteAnimation
		{			
			animationEndcallback = animationCallback;
			frameChangedCallback = frameCallback;
			
			if (!reset && _anim && _anim.name == name) return _anim;
			_anim = _anims[name];
			if (!_anim)
			{
				_frame = null;
				_index = 0;
				complete = true;
				updateBuffer();
				return null;
			}
			_index = 0;
			_timer = 0;
			_frame = _anim.getFrame(frame % _anim.frames);
			x =  _frame.offset.x;
			y =  _frame.offset.y;
			frameCallback(_frame);
					
			complete = false;
			updateBuffer();
			return _anim;
		}
		
		/**
		 * The currently playing animation Name.
		 */
		public function get currentAnim():String { return _anim ? _anim.name : ""; }
		
		/**
		 * The currently playing animation.
		 */
		public function get currentAnimObj():MetaSpriteAnimation { return _anim ; }
		
		// Spritemap information.
		/** @private */ protected var _frame:MetaFrame;
		/** @private */ private var _anims:Object = { };
		/** @private */ private var _anim:MetaSpriteAnimation;
		/** @private */ protected var _rect:Rectangle;
		/** @private */ private var _timer:Number = 0;
		/** @private */ private var _index:uint;
	}
}