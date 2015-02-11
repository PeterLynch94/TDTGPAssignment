using System;
using System.Collections.Generic;


using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;


using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;



namespace TowerDefense
{
	
	
	public class Turret
	{
		private SpriteUV 				sprite;
		private TextureInfo				spriteTex;
		private turretStats				stats;
		private int						level, spentGold, ID;
		private Vector2					position, direction;
		private int 					width, height;
		private float 					fireTimer;
		private bool 					canFire, lockedOn;


		
		public Turret (Scene scene, Vector2 pos, int id)
		{
			spriteTex = new TextureInfo("Application/graphics/TURRET.png");
			sprite = new SpriteUV(spriteTex);
			
			
			
			stats = new turretStats();
			setStats (0);
			sprite.Position = pos;
			position = pos;
			level = 0;
			sprite.Quad.S = spriteTex.TextureSizef;
			width = sprite.TextureInfo.Texture.Width;
			height = sprite.TextureInfo.Texture.Height;
			ID = id;
			lockedOn = false;
			scene.AddChild(sprite);
		}
		
		
		public void Update(float t)
		{
			if(!canFire)
			{
				fireTimer += 0.2f;
				
				if(fireTimer > stats.fireRate)
				{
					canFire = true;
				}
			}
		}
		
		public turretStats getStats()
		{
			return stats;
		}
		
		public void setStats(int level)
		{
			if(level == 0)
			{
				stats.damage = 10;
				stats.fireRate = 15;
				level = 0;
			}
		}
		
		public bool fireCheck()
		{
			if(canFire)
			{
				canFire = false;
				fireTimer = 0;
				return true;
			} else {
				return false;
			}
		}
		
		public bool RotateToEnemy(Vector2 enemyPos)
		{
			//Set sprite centre for rotation to its center
			sprite.CenterSprite(TRS.Local.Center);
			
			direction = GetCenter() - enemyPos;
			

			
			sprite.RotationNormalize = direction;

			return true;
			
		}
		
		public Vector2 getDirection()
		{
			return direction.Normalize();
		}
		
		public int getLevel()
		{
			return level;
		}
		
		public Vector2 getPos()
		{
			return position;
		}
		
		public void setLockOn(bool lockon)
		{
			lockedOn = lockon;
		}
		
		public bool getLockOn()
		{
			return lockedOn;
		}
		
		
		public int getID()
		{
			return ID;
		}
		
		public void setID(int id)
		{
			ID = id;
		}
		
		
		private Vector2 GetCenter()
		{
			Vector2 ret;
			ret.X = position.X+(width*0.5f);
			ret.Y = position.Y+(height*0.5f);
			return ret;
		}
		
		
		

		
		
	}
}

