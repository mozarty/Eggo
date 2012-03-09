package com.eggo.entities
{
	/**
	 * ...
	 * @author Salem
	 */
	import net.flashpunk.Entity;
	import net.flashpunk.graphics.Image;
	import net.flashpunk.utils.Input;
	import net.flashpunk.utils.Key;
	import net.flashpunk.graphics.Spritemap;
	import net.flashpunk.FP;
	
	public class Egg extends Entity
	{
		[Embed(source='../../../../asset/yo.png')]
		private const EGG:Class;
		
		public var egg:Spritemap = new Spritemap(EGG, 0, 0);
		public var time:Number = 0;
		public var updatetime:Number = 0.03;
		
		public function Egg()
		{
			// Here I set the hitbox width/height with the setHitbox function.		
			egg.scale = 0.2;
			setHitbox(egg.scaledWidth, egg.scaledHeight);
			type = "egg";
			
			//sprSwordguy.add("stand", [0, 1, 2, 3, 4, 5], 20, true);
			//sprSwordguy.add("run", [6, 7, 8, 9, 10, 11], 20, true);
			Input.define("run", Key.LEFT, Key.RIGHT);
			graphic = egg;
		
			//sprSwordguy.play("stand",true);
		}
		
		override public function update():void
		{
			time += FP.elapsed;
			if (time >= updatetime)
			{
				x -= 20;
				egg.centerOrigin();
				egg.angle += 20;
				time -= updatetime;
			}
		
		}
		
		public function destroy():void
		{
			FP.world.remove(this);
		}
	}

}