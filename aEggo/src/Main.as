package
{
	import flash.display.Sprite;
	import flash.events.Event;
	import net.flashpunk.Engine;
	import net.flashpunk.FP;
	import com.eggo.worlds.GamePlay;
	
	public class Main extends Engine
	{
		
		public static var Width:int = 800;
		public static var Height:int = 600;
		
		public function Main()
		{
			
			super(Width, Height, 20, false);
			FP.world = new GamePlay;
			// Make the background color full red.
			FP.screen.color = 0xFFFFFF;
		}
		
		override public function init():void
		{
			trace("FlashPunk has started successfully!");
		}
	}
}