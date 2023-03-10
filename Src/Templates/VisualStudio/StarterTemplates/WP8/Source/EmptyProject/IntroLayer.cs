using System;
using CocosSharp;
using Microsoft.Xna.Framework;

namespace EmptyProject
{
	public class IntroLayer : CCLayerColor
	{
		public IntroLayer () 
		{

			// create and initialize a Label
			var label = new CCLabelTtf("Hello CocosSharp", "MarkerFelt", 22);

			// position the label on the center of the screen
			label.Position = CCDirector.SharedDirector.WinSize.Center;

			// add the label as a child to this Layer
			AddChild(label);

			// setup our color for the background
			Color = new CCColor3B (CCColor4B.Blue);
			Opacity = 255;

		}

		public static CCScene Scene 
		{
			get {
				// 'scene' is an autorelease object.
				var scene = new CCScene();

				// 'layer' is an autorelease object.
				var layer = new IntroLayer();

				// add layer as a child to scene
				scene.AddChild(layer);

				// return the scene
				return scene;

			}

		}

	}
}

