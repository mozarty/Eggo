package com.eggo.worlds
{
	import it.svelto.graphic.importers.SpriteBuddy;
	import it.svelto.graphic.flashpunk.FlexiSpriteMap;
	import net.flashpunk.Entity;
	import net.flashpunk.graphics.Image;
	import net.flashpunk.World;
	import com.eggo.entities.Player;
	import com.eggo.entities.Egg;
	import flash.events.TimerEvent;
	import flash.utils.Timer;
	
	public class GamePlay extends World
	{
		private var playerSprite:Player = new Player();
		
		function GamePlay()
		{
			add(playerSprite);
			// adding enimes generator
			var myTimer:Timer = new Timer(1000); // 1 second
			myTimer.addEventListener(TimerEvent.TIMER, runMany);
			myTimer.start();
		}
		
		public function runMany(event:TimerEvent):void
		{
			
			var egg:Egg = new Egg();
			egg.y = 170;
			egg.x = Main.Width;
			add(egg);
		
		}
		
		
	}
}