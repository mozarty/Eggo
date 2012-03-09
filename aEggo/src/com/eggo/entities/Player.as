package com.eggo.entities
{
	import it.svelto.graphic.MetaFrame;
	import net.flashpunk.Entity;
	import net.flashpunk.utils.Draw;
	import net.flashpunk.utils.Key;
	import net.flashpunk.utils.Input;
	import it.svelto.graphic.flashpunk.FlexiSpriteMap;
	import it.svelto.graphic.importers.SpriteBuddy;
	import it.svelto.graphic.flashpunk.FlexiSpriteMap;
	
	//TODO split jump to up and down
	public class Player extends Entity
	{
		[Embed(source="../../../../asset/wolverine_sheet.png")]
		private var characterImage:Class;
		
		[Embed(source="../../../../asset/wolvie.xml")]
		private var boxXml:Class;
		
		private var sprite:FlexiSpriteMap;
		public var spriteSheet:SpriteBuddy;
		public var dir:Boolean = false;
		public var attackMode:String = "attack2";
		public var isjumping:Boolean = false;
		public var jumpframes:int = 6;
		public var attackFrames:int = 7;
		
		function Player()
		{
			spriteSheet = new SpriteBuddy(boxXml.data as XML);
			sprite = new FlexiSpriteMap(characterImage, spriteSheet.metaSprite);
			graphic = sprite;
			
			Input.define("run", Key.LEFT, Key.RIGHT);
			Input.define("attack", Key.SPACE);
			Input.define("jump", Key.UP);
			x = 50;
			y = 150;
		}
		
		override public function added():void
		{
			sprite.play("stand", true, 0, setNewHitBox);
		}
		
		override public function render():void
		{
			super.render();
			
			Draw.hitbox(this);
		}
		
		override public function update():void
		{
			
			var egg:Egg = collide("egg", x, y) as Egg;
			
			if (egg)
			{
				// Player is colliding with an "egg" type.
				if (sprite.currentAnim == attackMode)
				{
					trace("good");
					egg.destroy();
				}
				else
				{
					trace("bad");
						//egg.destroy();
				}
			}
			
			if (Input.released("run"))
			{
				sprite.play("stand", true, 0, setNewHitBox);
			}
			
			if (Input.check(Key.LEFT))
			{
				if (!isjumping)
				{
					sprite.flipped = true;
					dir = true;
					sprite.play("run", false, 0, setNewHitBox);
				}
				x -= 15;
			}
			if (Input.check(Key.RIGHT))
			{
				if (!isjumping)
				{
					sprite.flipped = false;
					dir = false;
					sprite.play("run", false, 0, setNewHitBox);
				}
				x += 15;
			}
			if (Input.pressed("jump"))
			{
				sprite.flipped = dir;
				sprite.play("jump", true, 0, setNewHitBox);
				sprite.currentAnimObj.loops = false;
				isjumping = true;
				frameNumber = 1;
			}
			if (Input.pressed("attack"))
			{
				sprite.flipped = dir;
				sprite.play(attackMode, true, 0, setNewHitBox, attackFinished);
				sprite.currentAnimObj.loops = false;
			}
			if (Input.check(Key.DOWN))
			{
				//sprSwordguy.play("run");
				//y += 5;
			}
		
		}
		
		public var frameNumber:int = 1;
		
		private function attackFinished():void
		{
			sprite.play("stand", true, 0, setNewHitBox);
		}
		
		private function setNewHitBox(frame:MetaFrame):void
		{
			if (sprite.currentAnim == attackMode)
			{
				frameNumber++;
				if (frameNumber > attackFrames)
				{
					frameNumber = 1;
					sprite.play("stand", true, 0, setNewHitBox);
				}
			}
			else if (isjumping)
			{
				
				//trace(frameNumber)
				if (frameNumber <= ((int)(jumpframes / 2)))
				{
					//trace("up")
					y -= 30;
				}
				else
				{
					//trace("down")
					y += 30;
				}
				frameNumber++;
				
				if (frameNumber > jumpframes)
				{
					isjumping = false;
					frameNumber = 1;
					sprite.play("stand", true, 0, setNewHitBox);
				}
			}
			setHitbox(frame.sourceRect.width, frame.sourceRect.height, -frame.offset.x, -frame.offset.y);
		}
	}
}