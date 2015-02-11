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
	public class Bullet
	{
		private SpriteUV 				sprite;
		private TextureInfo				spriteTex;
		private float					damage, speed;
		private Vector2 				position;
		private Vector2					direction;
		private int						width, height;
		private bool					dead;
		
		public Bullet (Scene scene, Vector2 pos, Vector2 dir)
		{
			spriteTex = new TextureInfo("Application/graphics/BULLET.png");
			sprite = new SpriteUV(spriteTex);
			sprite.Position = pos;
			position = pos;
			sprite.Quad.S 	= spriteTex.TextureSizef;
			dead = false;
			//Set sprites center to the middle of the sprite
			sprite.CenterSprite(TRS.Local.Center);
			
			//Rotate sprite to face the (already normalized fomr the turret) direction
			sprite.Rotation = dir;
			
			width = sprite.TextureInfo.Texture.Width;
			height = sprite.TextureInfo.Texture.Height;
			direction = dir;
			speed = 5.0f;
			damage = 40.0f;
			scene.AddChild(sprite);
		}
		
		
		public void Update()
		{
			if(!dead)
			{
				position -= direction * speed;
				sprite.Position = position;
			}

		}
		
		public void setDead(bool p)
		{
			dead = p;
			if(dead)
			{
				position = new Vector2(1000.0f, 1000.0f);
				sprite.Visible = false;
			}
		}
		
		public bool getDead()
		{
			return dead;
		}
		
		public Vector2 getPos()
		{
			return position;
		}
		
		public int getWidth()
		{
			return sprite.TextureInfo.Texture.Width;
		}
		
		public int getHeight()
		{
			return sprite.TextureInfo.Texture.Height;
		}
		
		public float getDamage()
		{
			return damage;
		}
		
		public void Dispose()
		{
			spriteTex.Dispose();
			sprite = null;
		}
		
		
		
	}
}

