/****************************************************************************
Copyright (c) 2010-2012 cocos2d-x.org
Copyright (c) 2008-2010 Ricardo Quesada
Copyright (c) 2011 Zynga Inc.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
****************************************************************************/

namespace CocosSharp
{
    public class CCTransitionZoomFlipX : CCTransitionSceneOriented
    {
        float inDeltaZ, inAngleZ;
        float outDeltaZ, outAngleZ;


        #region Properties

        protected override CCFiniteTimeAction InSceneAction
        {
            get 
            { 
                return new CCSequence
                    (
                        new CCDelayTime (Duration / 2),
                        new CCSpawn
                        (
                            new CCOrbitCamera(Duration / 2, 1, 0, inAngleZ, inDeltaZ, 0, 0),
                            new CCScaleTo(Duration / 2, 1),
                            new CCShow()
                        )
                    );
            }
        }

        protected override CCFiniteTimeAction OutSceneAction
        {
            get 
            { 
                return new CCSequence(
                    new CCSpawn
                    (
                        new CCOrbitCamera(Duration / 2, 1, 0, outAngleZ, outDeltaZ, 0, 0),
                        new CCScaleTo(Duration / 2, 0.5f)
                    ),
                    new CCHide(),
                    new CCDelayTime (Duration / 2)
                );
            }
        }

        #endregion Properties


        #region Constructors

        public CCTransitionZoomFlipX (float duration, CCScene scene, CCTransitionOrientation orientation) 
            : base(duration, scene, orientation)
        {
        }

        #endregion Constructors


        protected override void InitialiseScenes()
        {
            base.InitialiseScenes();

            InSceneNodeContainer.Visible = false;
            InSceneNodeContainer.Scale = 0.5f;

            InSceneNodeContainer.IgnoreAnchorPointForPosition = true;
            OutSceneNodeContainer.IgnoreAnchorPointForPosition = true;

            InSceneNodeContainer.AnchorPoint = new CCPoint(0.5f, 0.5f);
            OutSceneNodeContainer.AnchorPoint = new CCPoint(0.5f, 0.5f);

            if (Orientation == CCTransitionOrientation.RightOver)
            {
                inDeltaZ = 90;
                inAngleZ = 270;
                outDeltaZ = 90;
                outAngleZ = 0;
            }
            else
            {
                inDeltaZ = -90;
                inAngleZ = 90;
                outDeltaZ = -90;
                outAngleZ = 0;
            }
        }

    }
}