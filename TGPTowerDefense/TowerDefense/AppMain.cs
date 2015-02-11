using System;
using System.Collections.Generic;
using System.IO;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;

using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;
using Sce.PlayStation.HighLevel.UI;
using Sce.PlayStation.Core.Audio;




namespace TowerDefense
{
	public struct turretStats 
	{
		public float damage;
		public float fireRate;
		
	};
	public class AppMain
	{
		private static Sce.PlayStation.HighLevel.GameEngine2D.Scene 	gameScene;
		private static Sce.PlayStation.HighLevel.UI.Scene 				uiScene;
		
		private static TextureInfo										bgTex;
		private static SpriteUV											bgSprite;
		

		
		private static bool												quitGame;
		private static int												screenH, screenW;
		private static int[]											mapData;
		
		private static List<Space>										grid;
		private static List<Space>										wayTiles;
		private static List<Space>										decoTiles;
		
		private static List<Bullet>										bullets;
		private static List<Enemy>										enemies;
		private static List<Turret>										turrets;
		
		private static Random											rand;
		private static Turret											turr;
		
		public static void Main (string[] args)
		{
			quitGame = false;
			Initialize ();
			
			//Game Loop
			while (!quitGame) 
			{
				Update ();
				
				Director.Instance.Update();
				UISystem.Update(Touch.GetData(0));
				Director.Instance.Render();
				UISystem.Render();
				
				Director.Instance.GL.Context.SwapBuffers();
				Director.Instance.PostSwap();
			}
		}

		public static void Initialize ()
		{
			Director.Initialize ();
			UISystem.Initialize(Director.Instance.GL.Context);
			
			//Set game scene
			gameScene = new Sce.PlayStation.HighLevel.GameEngine2D.Scene();
			gameScene.Camera.SetViewFromViewport();
			
			//Set the ui scene.
			uiScene = new Sce.PlayStation.HighLevel.UI.Scene();
			
			//Setup Panel
			Panel panel  = new Panel();
			panel.Width  = Director.Instance.GL.Context.GetViewport().Width;
			panel.Height = Director.Instance.GL.Context.GetViewport().Height;
			screenH = (int)panel.Height;
			screenW = (int)panel.Width;
			
			//add things
			load (1);
//			bgTex = new TextureInfo("Application/graphics/TESTBG.png");
//			bgSprite = new SpriteUV(bgTex);
//			bgSprite.Quad.S = bgTex.TextureSizef;
//			bgSprite.Position = new Vector2(10,0);
//			gameScene.AddChild(bgSprite);
			
			Vector2 ePos = new Vector2(0.0f, 0.0f);
			
			grid = new List<Space>();
			wayTiles = new List<Space>();
			decoTiles = new List<Space>();
			
			bullets = new List<Bullet>();
			enemies = new List<Enemy>();
			turrets = new List<Turret>();
			int typeCount = 0;
			bool enemySpawn = false;
			
			
			
			
			for(int i = 17; i > 0; i--)
			{
				Vector2 pos = new Vector2(10,0);
				for(int j = 0; j < 27; j++)
				{
					pos = new Vector2(j*32, i*32);
					Space temp = new Space(gameScene, pos, mapData[typeCount]);
					if(mapData[typeCount] == 19 && enemySpawn == false)
					{
						enemySpawn = true;
						ePos = new Vector2(pos.X + 32.0f, pos.Y - 50.0f);
					}
					typeCount++;
					if(temp.getType() == 20 || temp.getType() == 21 || temp.getType() == 22 || temp.getType() == 23)
					{
						wayTiles.Add(temp);
					}
					else if (temp.getType() != 10)
					{
						decoTiles.Add(temp);
					} else 
					{
						grid.Add (temp);
					}
				}
			}
			Vector2 turrPos = new Vector2(400.0f, 400.0f);
			Turret tempTurr = new Turret(gameScene, turrPos, 1);
			turrets.Add(tempTurr);
			
			turrPos = new Vector2(800.0f, 200.0f);
			tempTurr = new Turret(gameScene, turrPos, 2);
			turrets.Add(tempTurr);
			
			turrPos = new Vector2(250.0f, 250.0f);
			tempTurr = new Turret(gameScene, turrPos, 3);
			turrets.Add(tempTurr);
			
			
	
				
				
			rand = new Random();
			for(int i = 0; i < 3; i++)
			{
				ePos.X += 5.0f;
				ePos.Y -= 5.0f;
				Enemy ene = new Enemy(gameScene, ePos, 2, rand);
				enemies.Add(ene);
			}
			
			ePos.X -= 70.0f;
			ePos.Y -= 90.0f;
			turr = new Turret(gameScene, ePos, 47);
			uiScene.RootWidget.AddChildLast(panel);
			
			UISystem.SetScene(uiScene);
			
			
			//Run the scene.
			Director.Instance.RunWithScene(gameScene, true);
		}

		public static void Update ()
		{
			

				foreach(Turret t in turrets)
				{
					t.Update(0);
					if(t.getLockOn())
					{
						for(int j = 0; j < enemies.Count; j++)
						{
							if(enemies[j].getID() == t.getID())
							{
								t.RotateToEnemy(enemies[j].GetCenter());
							}
						}
						
					} else {
						for(int i = 0; i < enemies.Count; i++)
						{
							if(enemies[i].getID() == 0 && t.getLockOn() == false)
							{
								enemies[i].setID(t.getID());
								t.setLockOn(true);
							}
						}
					}
					if(t.fireCheck() == true)
					{
						Bullet bu = new Bullet(gameScene, t.getPos(), t.getDirection());
						bullets.Add (bu);
					}
				}
				
			for(int i = 0; i < enemies.Count; i++)
			{
				enemies[i].Update(0);
			}

			
			
			BulletUpdate ();
			GridUpdate();
			
			
		}
		
		public static void BulletUpdate()
		{
			for(int j = 0; j < bullets.Count; j++)
			{
				bullets[j].Update ();
				Vector2 bPos = bullets[j].getPos ();
				int bW = bullets[j].getWidth();
				int bH = bullets[j].getHeight();
				
				for(int i = 0; i < enemies.Count; i++)
				{
					if(!enemies[i].getDead () && !bullets[j].getDead())
					{
						if(bPos.X < enemies[i].getPos().X + enemies[i].getWidth() && bPos.X + bW > enemies[i].getPos ().X &&
						   bPos.Y < enemies[i].getPos().Y + enemies[i].getHeight() && bPos.X + bH > enemies[i].getPos().Y )
						{
							enemies[i].takeDamage(bullets[j].getDamage());
							
							bullets[j].setDead(true);
							bullets.RemoveAt(j);
							resetLockOns();
							break;
						}
					}
				}
			}
		}
		
		public static void resetLockOns()
		{
			foreach(Turret t in turrets)
			{
				t.setLockOn(false);
			}
			foreach(Enemy e in enemies)
			{
				e.setID(0);
			}
		}
		
		
		public static void GridUpdate()
		{
			var touchT = Touch.GetData(0).ToArray();
			int touchX = -100;
			int touchY = -100;
			if(touchT.Length > 0 && touchT[0].Status == TouchStatus.Up)
			{
				touchX = (int)((touchT[0].X + 0.5f) * screenW);
				touchY = screenH-(int)((touchT[0].Y + 0.5f) * screenH);
				//Grid selection code
				foreach(Space s in grid)
				{
					int sW = s.getWidth();
					int sH = s.getHeight();
					int sX = s.getX ();
					int sY = s.getY ();
					int sT = s.getType();

					if(touchX <= (sW + sX) && touchX >= sX && touchY <= (sH + sY) && touchY >= sY)
					{
						s.setSelected();
						Vector2 pos = new Vector2(sX, sY);
						foreach(Space h in grid)
						{
							
							if(h.getX() != pos.X || h.getY() != pos.Y)
							{
								h.unSelect();	
							}
						
						}
					}
				
				}
			}
			
			foreach(Space w in wayTiles)
			{
				foreach(Enemy en in enemies)
				{
					if(!en.getDead ())
					{
						int wayW = w.getWWidth();
						int wayH = w.getWHeight();
						int wayX = w.getWX ();
						int wayY = w.getWY ();
						
						float enX = en.getPos().X;
						float enY = en.getPos().Y;
						int enH = en.getHeight();
						int enW = en.getWidth();
						if(enX <= (wayW + wayX) && (enX + enW) >= wayX && enY <= (wayH + wayY) && (enY + enH) >= wayY)
						{
							en.setDirection(w.getWayDir());
							en.randDelay(rand);
						}
					}
				}
				
			}
		}
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		public static void load(int lv)
		{
			
			
			/*Map editing key
			 * 10 = Empty space for turrets
			 * 11 = Vertical Wall |
			 * 12 = Horizontal wall _
			 * 13 = r corner
			 * 14 = L corner
			 * 15 = inverse R corner -|
			 * 16 = inverse L corner _|
			 * 19 = Standard ground tile for enemies
			 * (20-23 all include 19's graphic)
			 * 20 = Move left waypoint
			 * 21 = Move right waypoint
			 * 22 = move down waypoint
			 * 23 = move up waypoint
			 */
			
			
			mapData = new int[459] 
			{10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,11,19,19,19,11,10,10,10,
			 10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,11,19,19,19,11,10,10,10,
			 10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,11,19,19,19,11,10,10,10,
			 10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,11,19,19,19,11,10,10,10,
			 10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,11,19,19,19,11,10,10,10,
			 10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,11,19,19,19,11,10,10,10,
			 10,10,10,10,10,10,10,10,10,13,12,12,12,12,12,12,12,12,12,16,20,20,20,11,10,10,10,
			 10,10,10,10,10,10,10,10,10,11,19,19,19,22,19,19,19,19,19,19,19,19,19,11,10,10,10,
			 10,10,10,10,10,10,10,10,10,11,19,19,19,22,19,19,19,19,19,19,19,19,19,11,10,10,10,
			 10,10,10,10,10,10,10,10,10,11,19,19,19,22,19,19,19,19,19,19,19,19,19,11,10,10,10,
			 10,10,10,10,10,10,10,10,10,11,19,19,19,13,12,12,12,12,12,12,12,12,12,16,10,10,10,
			 10,10,10,10,10,10,10,10,10,11,19,19,19,11,10,10,10,10,10,10,10,10,10,10,10,10,10,
			 10,10,10,10,10,10,10,10,10,11,21,21,21,14,12,12,12,12,12,12,12,12,12,15,10,10,10,
			 10,10,10,10,10,10,10,10,10,11,19,19,19,19,19,19,19,19,22,19,19,19,19,11,10,10,10,
			 10,10,10,10,10,10,10,10,10,11,19,19,19,19,19,19,19,19,22,19,19,19,19,11,10,10,10,
			 10,10,10,10,10,10,10,10,10,11,19,19,19,19,19,19,19,19,22,19,19,19,19,11,10,10,10,
			 10,10,10,10,10,10,10,10,10,14,12,12,12,12,12,12,12,12,12,15,19,19,19,11,10,10,10};
		
//			string path = "Application/levels/level" + lv.ToString() + ".txt";
//		            using (System.IO.FileStream hStream = System.IO.File.OpenRead(@path))
//					{
//		                if (hStream != null) 
//						{
//		                    long size = hStream.Length;
//			                byte[] buffer = new byte[size];
//			                hStream.Read(buffer, 10, (int)size);
//							int x = sizeof(Int32);
//					//17*27 =459
//							mapData = new int[459];
//			                Int32 sum=0;
//			                for(int i=0; i<459; i++)
//			                {
//			                    Buffer.BlockCopy(buffer, sizeof(Int32)*i, mapData, sizeof(Int32)*i,  sizeof(Int32));
//			                    sum+=mapData[i];
//			                }
//		                }
//						hStream.Close();
//					}
		}
	}
}
