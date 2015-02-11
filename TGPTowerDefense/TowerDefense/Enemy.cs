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
	public class Enemy
	{
		
		private SpriteUV 				sprite;
		private TextureInfo				spriteTex;
		private Vector2 				pos;
		private Vector2 				turnPos;
		private int 					dir, newDir, gold, colTimer, colTimerMax, spW, spH, ID;
		private float					health;
		private float					speed;
		private bool					turning, dead;
		
		
		public Enemy (Scene scene, Vector2 p, int d, Random rand)
		{
			spriteTex = new TextureInfo("Application/graphics/FOE.png");
			sprite = new SpriteUV(spriteTex);
			sprite.Position = p;
			sprite.Quad.S 	= spriteTex.TextureSizef;
			spH = sprite.TextureInfo.Texture.Height;
			spW = sprite.TextureInfo.Texture.Width;
			health = 100.0f;
			speed = 1.0f;
			dir = d;
			gold = 50;
			dead = false;
			colTimerMax = rand.Next(32, 64);
			colTimer = colTimerMax;
			turning = false;
			ID = 0;
			scene.AddChild(sprite);

			
		}
		
		public void Update(float t)
		{
			if(!dead)
			{
				pos = sprite.Position;
				if(health <= 0)
				{
					Dispose();
				}
				if(dir == 0)
				{
					pos = new Vector2(pos.X  -= speed, pos.Y);
				}else if (dir == 1)
				{
					pos = new Vector2(pos.X  += speed, pos.Y);
				}else if (dir == 2)
				{
					pos = new Vector2(pos.X, pos.Y -= speed);
				}else if (dir == 3)
				{
					pos = new Vector2(pos.X, pos.Y+= speed);
				}
				if(turning)
				{
					checkTurn ();
				}
				
				sprite.Position = new Vector2(pos.X, pos.Y);
				if(health < 0.0f)
				{
					setDead(true);
				}
			}
			
			
		}
		
		private void setDead(bool p)
		{
			dead = p;
			if(dead)
			{
				//sprite.Position = new Vector2(1000.0f, 1000.0f);
				sprite.Visible = false;
				
			}
		}
		
		
		private void checkTurn()
		{
			if(dir == 0)
			{
				if(pos.X <= turnPos.X - colTimerMax)
				{
					dir = newDir;
					turning = false;
				}
			} else if (dir == 1)
			{
				if(pos.X >= turnPos.X + colTimerMax)
				{
					dir = newDir;
					turning = false;
				}
			}else if (dir == 2)
			{
				if(pos.Y <= turnPos.Y - colTimerMax)
				{
					dir = newDir;
					turning = false;
				}
				
			}
		}
		
		public void randDelay(Random rand)
		{
			colTimerMax = rand.Next (32, 64);
			turnPos = pos;
			
		}

		
		public void setDirection(int d)
		{
			turning = true;
			newDir = d;
		}
		
		public SpriteUV getSprite()
		{
			return sprite;
		}
		
		public void takeDamage(float damage)
		{
			health -= damage;
		}
		
		public Vector2 getPos()
		{
			return pos;
		}
		
		public int getHeight()
		{
			return spH;
		}
		
		public int getWidth()
		{
			return spW;
		}

		
		
		public bool getDead()
		{
			return dead;
		}
		public Vector2 GetCenter()
		{
			Vector2 ret;
			ret.X = pos.X + getWidth()*0.5f;
			ret.Y = pos.Y + getHeight()*0.5f;
			return ret;
		}
		
		public void Dispose()
		{
			spriteTex.Dispose();
			
		}
		
		public int getID()
		{
			return ID;
		}
		
		public void setID(int id)
		{
			ID = id;
		}
		
	}
}

