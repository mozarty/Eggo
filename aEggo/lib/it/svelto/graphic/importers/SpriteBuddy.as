package it.svelto.graphic.importers
{
	import flash.geom.Point;
	import flash.geom.Rectangle;
	import it.svelto.graphic.MetaFrame;
	import it.svelto.graphic.MetaSprite;
	import it.svelto.graphic.MetaSpriteAnimation;

	public class SpriteBuddy
	{
		private var _metaSprite:MetaSprite;
		private var _onProcessedCMD:Function;
		
		function SpriteBuddy(file:XML)
		{
			processSpritesData(file);
		}
		
		private function processSpritesData(xml:XML):void
		{
			var spritesList:XMLList = xml.sprites;
			
			for each (var spriteData:XML in spritesList)
				buildSprite(spriteData.children()[0]);
		}
		
		private function buildSprite(spriteData:XML):void
		{
			_metaSprite = new MetaSprite(spriteData.name());
			
			for each (var animationData:XML in spriteData.animations.children())
			{
				var metaAnimation:MetaSpriteAnimation = new MetaSpriteAnimation(animationData.name());
				
				metaAnimation.bitmapSource = spriteData.bitmapPath;
				metaAnimation.transparentColour = animationData.transparentColour;
				
				for each (var animationFrame:XML in animationData.layer1.children())
				{
					var rect:String = animationFrame.sourceRect;
					rect = rect.substring(5, rect.length - 1);
					rect = rect.replace(/\s/g, "");
					var values:Array = rect.split(",");
					
					var metaFrame:MetaFrame = new MetaFrame(new Rectangle(values[0], values[1], values[2]-values[0], values[3]-values[1]));
					metaFrame.duration = uint(animationFrame.duration);
					
					var point:String = animationFrame.offset;
					point = point.substring(6, point.length - 1);
					point = point.replace(/\s/g, "");
					values = point.split(",");
					
					metaFrame.offset = new Point(values[0], values[1]);
					
					metaAnimation.addFrame(metaFrame);
				}
				
				_metaSprite.addMetaAnimation(metaAnimation);
			}
		}
		
		public function get metaSprite():MetaSprite
		{
			return _metaSprite; 
		}
	}
}